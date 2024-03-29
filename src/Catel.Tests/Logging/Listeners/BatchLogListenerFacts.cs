﻿namespace Catel.Tests.Logging.Listeners
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Catel.Logging;
    using NUnit.Framework;

    public class BatchLogListenerFacts
    {
        [TestFixture]
        public class TheWriteBatchAsyncMethod
        {
            private class CustomBatchLogListener : BatchLogListenerBase
            {
                private int _flushCounter;

                public CustomBatchLogListener()
                    : base(TimeSpan.FromSeconds(1))
                {
                    FlushWhenBatchIsEmpty = true;
                }

                public int FlushCounter
                {
                    get
                    {
                        lock (this)
                        {
                            return _flushCounter;
                        }
                    }
                }

                protected override async Task WriteBatchAsync(List<LogBatchEntry> batchEntries)
                {
                    lock (this)
                    {
                        _flushCounter++;
                    }

                    // Note: must be longer than the interval in the batch log listener
                    await Task.Delay(2000);
                }
            }

            [Explicit]
            [TestCase(1000, 0)]
            [TestCase(2400, 1)]
            [TestCase(4400, 2)]
            public async Task TestMultipleBatchesAsync(int millisecondsToWait, int expectedFlushCount)
            {
                var listener = new CustomBatchLogListener();

                await Task.Delay(millisecondsToWait);

                Assert.That(listener.FlushCounter, Is.EqualTo(expectedFlushCount), "The timer should have awaited the calls");
            }
        }
    }
}
