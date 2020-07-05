using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace DataflowExtensions.Tests
{
    public class CancelTests
    {
        [SetUp]
        public void Setup()
        {
        }
        
        [Test]
        public void Test0()
        {
            var actionProducerConsumer = new ActionBlockPerformer<int>(4);

            var execCount = 0;
            var errors = 0;
            var completedCommon = 0;
            var errorsCommon = 0;

            actionProducerConsumer.ItemComplete += (sender, item) => { completedCommon++; };
            actionProducerConsumer.ItemFailed += (sender, item) => { errorsCommon++; };

            var random = new Random(1);

            var put = new List<int>();
            var result = new List<int>();
            var exceptions = new List<Exception>();

            var all = new List<PerfWorkItem<int>>();

            for (var i = 0; i < 20; i++)
            {
                var t = new PerfWorkItem<int>(o =>
                {
                    Thread.Sleep(random.Next(1, 500));
                    Console.WriteLine($"{o} run");
                    result.Add(o);
                })
                {
                    Argument = i
                };

                t.Completed += (o, y) => { execCount++; };
                t.Failed += (o, y) =>
                {
                    errors++;
                    exceptions.Add(y);
                };

                t.IsCanceled = true;

                all.Add(t);
                put.Add(i);
                actionProducerConsumer.Send(t);
            }

            actionProducerConsumer.Wait(2000);


            foreach (var perfWorkItem in all)
            {
                Assert.IsNull(perfWorkItem.Error);
            }


            Assert.AreEqual(0, result.Count);
            Assert.AreEqual(0, execCount);
            Assert.AreEqual(0, errors);
            Assert.AreEqual(0, exceptions.Count);

            Assert.AreEqual(0, completedCommon);
            Assert.AreEqual(0, errorsCommon);

        }


        [Test]
        public void Test1()
        {
            var actionProducerConsumer = new ActionBlockPerformer<int>();

            var execCount = 0;
            var errors = 0;
            var completedCommon = 0;
            var errorsCommon = 0;

            actionProducerConsumer.ItemComplete += (sender, item) => { completedCommon++; };
            actionProducerConsumer.ItemFailed += (sender, item) => { errorsCommon++; };

            var random = new Random(1);

            var put = new List<int>();
            var result = new List<int>();
            var exceptions = new List<Exception>();

            var all = new List<PerfWorkItem<int>>();

            for (var i = 0; i < 20; i++)
            {
                var t = new PerfWorkItem<int>(o =>
                {
                    Thread.Sleep(random.Next(1, 500));
                    Console.WriteLine($"{o} run");
                    result.Add(o);
                })
                {
                    Argument = i
                };

                t.Completed += (o, y) => { execCount++; };
                t.Failed += (o, y) =>
                {
                    errors++;
                    exceptions.Add(y);
                };

                all.Add(t);
                put.Add(i);
                actionProducerConsumer.Send(t);
            }

            for (var i = 0; i < 15; i++)
            {
                all[i].IsCanceled = true;
            }

            actionProducerConsumer.Wait(3000);


            foreach (var perfWorkItem in all)
            {
                Assert.IsNull(perfWorkItem.Error);
            }


            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(5, execCount);
            Assert.AreEqual(0, errors);
            Assert.AreEqual(0, exceptions.Count);
            Assert.AreEqual(20, all.Count);

            Assert.AreEqual(5, completedCommon);
            Assert.AreEqual(0, errorsCommon);

        }
    }
}