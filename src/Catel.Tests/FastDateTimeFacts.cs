﻿namespace Catel.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class FastDateTimeFacts
    {
        [Test]
        public async Task ReturnsCorrectDateTime_NowAsync()
        {
            await ValidateDateTimeRetrievalAsync(() => DateTime.Now, () => FastDateTime.Now);
        }

        [Test]
        public async Task ReturnsCorrectDateTime_UtcNowAsync()
        {
            await ValidateDateTimeRetrievalAsync(() => DateTime.UtcNow, () => FastDateTime.UtcNow);
        }

        private async Task ValidateDateTimeRetrievalAsync(Func<DateTime> normal, Func<DateTime> fast)
        {
            var normalNow1 = normal();
            var fastNow1 = fast();

            var delta1 = fastNow1 - normalNow1;
            if (delta1 < TimeSpan.Zero)
            {
                delta1 = TimeSpan.FromMilliseconds(delta1.TotalMilliseconds * -1);
            }

            Assert.That(delta1.TotalMilliseconds < 15, Is.True);

            await Task.Delay(5000);

            var normalNow2 = normal();
            var fastNow2 = fast();

            var delta2 = fastNow2 - normalNow2;
            if (delta2 < TimeSpan.Zero)
            {
                delta2 = TimeSpan.FromMilliseconds(delta2.TotalMilliseconds * -1);
            }

            Assert.That(delta2.TotalMilliseconds < 15, Is.True);
        }
    }
}
