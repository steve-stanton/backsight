using System.Globalization;

namespace Backsight;

/// <summary>
/// Extension methods for instances of <see cref="String"/>
/// </summary>
public static class StringExtensions
{
    /// <param name="a">The string to be checked</param>
    extension(string a)
    {
        /// <summary>
        /// Checks whether two strings are equal,
        /// using <see cref="StringComparison.InvariantCultureIgnoreCase"/>
        /// </summary>
        /// <param name="b">The string to compare with (could be null)</param>
        /// <returns>True if the strings are equal (ignoring case)</returns>
        public bool EqualsIgnoreCase(string? b)
        {
            return String.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Checks whether a string starts with a specified substring,
        /// using <see cref="StringComparison.InvariantCultureIgnoreCase"/>
        /// </summary>
        /// <param name="prefix">The string to look for at the start of <paramref name="a"/></param>
        /// <returns>True if <paramref name="a"/> begins with
        /// <paramref name="prefix"/> (ignoring case). False if there is
        /// no match, or either string is null or empty.</returns>
        public bool StartsWithIgnoreCase(string prefix)
        {
            if (String.IsNullOrEmpty(a) || String.IsNullOrEmpty(prefix))
                return false;
            else
                return a.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Produces a string that does not contains unnecessary plurals.
        /// </summary>
        /// <param name="pluralPrefix">The marker that precedes characters that may need to be stripped.</param>
        /// <returns>A potentially modified string with any unnecessary characters removed.</returns>
        /// <remarks>
        /// This is a matter of looking for a word that contains <paramref name="pluralPrefix"/>,
        /// then looking for a previous word that can be converted into an integer. If that
        /// number is "1", the letter(s) following the marker will be stripped out.
        /// </remarks>
        public string TrimExtras(char pluralPrefix = '`')
        {
            if (String.IsNullOrEmpty(a))
                return a;

            string[] words = a.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                int pp = words[i].IndexOf(pluralPrefix);

                if (pp > 0)
                {
                    bool strip = false;

                    for (int j = i - 1; j >= 0; j--)
                    {
                        // TODO: Handle case where the previous word starts with something like a bracket -- (1 item`s) 
                        if (Int32.TryParse(words[j], out int number))
                        {
                            strip = number == 1;
                            break;
                        }
                    }

                    if (strip)
                        words[i] = words[i][..pp];
                    else
                        words[i] = words[i][..pp] + words[i][(pp + 1)..];
                }
            }

            return String.Join(" ", words);
        }

        /// <summary>
        /// Parses a UTC timestamp.
        /// </summary>
        /// <returns>The corresponding UTC timestamp.</returns>
        public DateTime ParseUtc()
        {
            return DateTime.ParseExact(a, "o",
                CultureInfo.InvariantCulture).ToUniversalTime();
        }
    }
}
