﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FastDateTimeFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests
{
    using System;
    using System.Threading.Tasks;
    using Catel.Threading;
    using NUnit.Framework;

    [TestFixture]
    public class FastDateTimeFacts
    {
        [Test]
        public async Task ReturnsCorrectDateTime_Now()
        {
            await ValidateDateTimeRetrieval(() => DateTime.Now, () => FastDateTime.Now);
        }

        [Test]
        public async Task ReturnsCorrectDateTime_UtcNow()
        {
            await ValidateDateTimeRetrieval(() => DateTime.UtcNow, () => FastDateTime.UtcNow);
        }

        private async Task ValidateDateTimeRetrieval(Func<DateTime> normal, Func<DateTime> fast)
        {
            var normalNow1 = normal();
            var fastNow1 = fast();

            var delta1 = fastNow1 - normalNow1;
            if (delta1 < TimeSpan.Zero)
            {
                delta1 = TimeSpan.FromMilliseconds(delta1.TotalMilliseconds * -1);
            }

            Assert.IsTrue(delta1.TotalMilliseconds < 15);

            await TaskShim.Delay(5000);

            var normalNow2 = normal();
            var fastNow2 = fast();

            var delta2 = fastNow2 - normalNow2;
            if (delta2 < TimeSpan.Zero)
            {
                delta2 = TimeSpan.FromMilliseconds(delta2.TotalMilliseconds * -1);
            }

            Assert.IsTrue(delta2.TotalMilliseconds < 15);
        }
    }
}