﻿namespace Catel
{
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    public static partial class StringExtensions
    {
        /// <summary>
        /// The slug regex.
        /// </summary>
        public static readonly Regex SlugRegex = new Regex(@"[^A-Za-z0-9_.]+");

        /// <summary>
        /// The white space regex.
        /// </summary>
        public static readonly Regex WhiteSpaceRegex = new Regex(@"[\s]+");

        /// <summary>
        /// Gets the slug of the specific input string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="spaceReplacement">The space replacement.</param>
        /// <param name="dotReplacement">The dot replacement.</param>
        /// <param name="makeLowercase">if set to <c>true</c>, make the slug lower case.</param>
        /// <returns>
        /// The slug based on the input.
        /// </returns>
        public static string GetSlug(this string input, string spaceReplacement = "", string dotReplacement = "",
            bool makeLowercase = true)
        {
            Argument.IsNotNullOrWhitespace("input", input);

            input = input.RemoveDiacritics();

            var output = WhiteSpaceRegex.Replace(input, spaceReplacement);
            output = SlugRegex.Replace(output, string.Empty);

            if (makeLowercase)
            {
                output = output.ToLowerInvariant();
            }

            output = output.Replace(".", dotReplacement);

            return output;
        }

        /// <summary>
        /// Removes the diacritics (special characters) from the string.
        /// </summary>
        /// <param name="value">The value.</param>
        public static string RemoveDiacritics(this string value)
        {
            var normalizedString = value.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
