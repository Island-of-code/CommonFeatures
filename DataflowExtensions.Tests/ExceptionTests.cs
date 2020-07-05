using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace DataflowExtensions.Tests
{
    public class ExceptionTests
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void ErrorTest3()
        {
            var completedCommon = 0;
            var errorsCommon = 0;

            var actionProducerConsumer = new ActionBlockPerformer<int>();
            actionProducerConsumer.Dispose();

            actionProducerConsumer.ItemComplete += (sender, item) => { completedCommon++; };
            actionProducerConsumer.ItemFailed += (sender, item) => { errorsCommon++; };

            Assert.Throws<Exception>((() =>
            {
                actionProducerConsumer.Send(new PerfWorkItem<int>(i =>
                {

                }));

            }));
        }

        [Test]
        public void ErrorTest0()
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
                t.Failed += (o, y) => { errors++; exceptions.Add(y); };

                all.Add(t);
                put.Add(i);
                actionProducerConsumer.Send(t);
            }

            actionProducerConsumer.Wait(2000);

           
            foreach (var perfWorkItem in all)
            {
                Assert.IsNull(perfWorkItem.Error);
            }

           
            Assert.AreEqual(20, result.Count);
            Assert.AreEqual(20, execCount);
            Assert.AreEqual(0, errors);
            Assert.AreEqual(0, exceptions.Count);

            Assert.AreEqual(20, completedCommon);
            Assert.AreEqual(0, errorsCommon);

            Console.WriteLine($"{execCount} summ");
        }

        [Test]
        public void ErrorTest1()
        {
            var actionProducerConsumer = new ActionBlockPerformer<int>(4);

            var execCount = 0;
            var errors = 0;

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
                    throw new Exception("Asd faf la la l a");
                    result.Add(o);
                })
                {
                    Argument = i
                };

                t.Completed += (o, y) => { execCount++; };
                t.Failed += (o, y) => { errors++; exceptions.Add(y); };

                all.Add(t);
                put.Add(i);
                actionProducerConsumer.Send(t);
            }

            actionProducerConsumer.Wait(2000);

            for (var i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(put[i], result[i]);
            }

            foreach (var perfWorkItem in all)
            {
                Assert.IsNotNull(perfWorkItem.Error);
            }

            for (var i = 0; i < exceptions.Count; i++)
            {
                Assert.IsTrue(exceptions[i].Message.Contains("Asd faf la la l a"));
            }

            Assert.AreEqual(0, result.Count);
            Assert.AreEqual(0, execCount);
            Assert.AreEqual(20, errors);
            Assert.AreEqual(20, exceptions.Count);
            
            Console.WriteLine($"{execCount} summ");
        }

        [Test]
        public void ErrorTest2()
        {
            var blockPerformer = new ActionBlockPerformer<int>(1);

            var completedCommon = 0;
            var errorsCommon = 0;

            blockPerformer.ItemComplete += (sender, item) => { completedCommon++; };
            blockPerformer.ItemFailed += (sender, item) => { errorsCommon++; };

            var execCount = 0;
            var errors = 0;

            var random = new Random(1);

            var all = new List<PerfWorkItem<int>>();

            var put = new List<int>();
            var result = new List<int>();
            var exceptions = new List<Exception>();

            for (var i = 0; i < 20; i++)
            {
                var t = new PerfWorkItem<int>(o =>
                {
                    Thread.Sleep(random.Next(1, 500));
                    Console.WriteLine($"{o} run");
                    throw new Exception("Asd faf la la l a");
                    result.Add(o);
                })
                {
                    Argument = i
                };

                t.Completed += (o, y) => { execCount++; };
                t.Failed += (o, y) => { errors++; exceptions.Add(y); };
                all.Add(t);
                put.Add(i);
                blockPerformer.Send(t);
            }

            blockPerformer.Wait(7000);

            for (var i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(put[i], result[i]);
            }

            for (var i = 0; i < exceptions.Count; i++)
            {
                Assert.IsTrue(exceptions[i].Message.Contains("Asd faf la la l a"));
            }

            foreach (var perfWorkItem in all)
            {
                Assert.IsNotNull(perfWorkItem.Error);
            }

            Assert.AreEqual(0, result.Count);
            Assert.AreEqual(0, execCount);
            Assert.AreEqual(20, errors);
            Assert.AreEqual(20, exceptions.Count);

            Assert.AreEqual(0, completedCommon);
            Assert.AreEqual(20, errorsCommon);

            Console.WriteLine($"{execCount} summ");
        }

    }
}