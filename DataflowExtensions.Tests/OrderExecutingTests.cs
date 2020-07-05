using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

namespace DataflowExtensions.Tests
{
    public class OrderExecutingTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void OrderExecutionTest1()
        {
            var actionProducerConsumer = new ActionBlockPerformer<int>(1);

            var completedCommon = 0;
            var errorsCommon = 0;

            actionProducerConsumer.ItemComplete += (sender, item) => { completedCommon++; };
            actionProducerConsumer.ItemFailed += (sender, item) => { errorsCommon++; };

            var execCount = 0;

            var random = new Random(1);

            var put = new List<int>();
            var result = new List<int>();

            for (var i = 0; i < 100; i++)
            {
                var t = new PerfWorkItem<int>(o =>
                {
                    //Thread.Sleep(random.Next(1, 5000));
                    Console.WriteLine($"{o} run");
                    result.Add(o);
                })
                {
                    Argument = i
                };

                t.Completed += (o, y) => { execCount++; };

                put.Add(i);
                actionProducerConsumer.Send(t);
            }

            actionProducerConsumer.Wait(1000);

            for (var i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(put[i], result[i]);
            }

            Assert.AreEqual(100, result.Count);
            Assert.AreEqual(100, execCount);

            Assert.AreEqual(100, completedCommon);
            Assert.AreEqual(0, errorsCommon);

            Console.WriteLine($"{execCount} summ");
        }

        [Test]
        public void OrderExecutionTest2()
        {
            var actionProducerConsumer = new ActionBlockPerformer<int>(1);

            var completedCommon = 0;
            var errorsCommon = 0;

            actionProducerConsumer.ItemComplete += (sender, item) => { completedCommon++; };
            actionProducerConsumer.ItemFailed += (sender, item) => { errorsCommon++; };

            var execCount = 0;

            var random = new Random(1);

            var put = new List<int>();
            var result = new List<int>();

            for (var i = 0; i < 30; i++)
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

                put.Add(i);
                actionProducerConsumer.Send(t);
            }

            actionProducerConsumer.Wait(10000);

            for (var i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(put[i], result[i]);
            }

            Assert.AreEqual(30, result.Count);
            Assert.AreEqual(30, execCount);

            Assert.AreEqual(30, completedCommon);
            Assert.AreEqual(0, errorsCommon);

            Console.WriteLine($"{execCount} summ");
        }

        [Test]
        public void OrderExecutionTest3()
        {
            var actionProducerConsumer = new ActionBlockPerformer<int>(1);

            var execCount = 0;

            var random = new Random(1);

            var put = new List<int>();
            var result = new List<int>();

            for (var i = 0; i < 10; i++)
            {
                var t = new PerfWorkItem<int>(o =>
                {
                    Thread.Sleep(random.Next(1, 2000));
                    Console.WriteLine($"{o} run");
                    result.Add(o);
                })
                {
                    Argument = i
                };

                t.Completed += (o, y) => { execCount++; };

                put.Add(i);
                actionProducerConsumer.Send(t);
            }

            actionProducerConsumer.Wait(10000);

            for (var i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(put[i], result[i]);
            }

           
            Assert.AreEqual(10, result.Count);
            Assert.AreEqual(10, execCount);
        }

        [Test]
        public void OrderExecutionTest4()
        {
            var actionProducerConsumer = new ActionBlockPerformer<int>(1);

            var completedCommon = 0;
            var errorsCommon = 0;

            actionProducerConsumer.ItemComplete += (sender, item) => { completedCommon++; };
            actionProducerConsumer.ItemFailed += (sender, item) => { errorsCommon++; };

            var execCount = 0;

            var random = new Random(1);

            var put = new List<int>();
            var result = new List<int>();

            for (var i = 0; i < 10; i++)
            {
                var t = new PerfWorkItem<int>(o =>
                {
                    Thread.Sleep(random.Next(1, 2000));
                    Console.WriteLine($"{o} run");
                    result.Add(o);
                })
                {
                    Argument = i
                };

                t.Completed += (o, y) => { execCount++; };

                put.Add(i);
                actionProducerConsumer.Send(t);
            }

            actionProducerConsumer.Wait(10000);

            for (var i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(put[i], result[i]);
            }

            Assert.AreEqual(10, result.Count);
            Assert.AreEqual(10, execCount);

            for (var i = 0; i < 10; i++)
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

                put.Add(i);
                actionProducerConsumer.Send(t);
            }

            actionProducerConsumer.Wait(5000);

            for (var i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(put[i], result[i]);
            }

            Assert.AreEqual(20, completedCommon);
            Assert.AreEqual(0, errorsCommon);

            Assert.AreEqual(20, result.Count);
            Assert.AreEqual(20, execCount);
        }

    }
}