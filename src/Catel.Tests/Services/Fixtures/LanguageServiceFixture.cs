﻿namespace Catel.Tests.Services.Fixtures
{
    using System.Collections.Generic;
    using System.Globalization;
    using Catel.Services;

    public class LanguageServiceFixture : LanguageService
    {
        public readonly Dictionary<string, string> _values = new Dictionary<string, string>();

        public void RegisterValue(string resourceName, string value)
        {
            _values[resourceName] = value;
        }

        public override string GetString(ILanguageSource languageSource, string resourceName, CultureInfo cultureInfo)
        {
            return _values[resourceName];
        }
    }
}