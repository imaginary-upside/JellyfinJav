namespace JellyfinJav
{
    /// <summary>A class for adding methods to the built in String class.</summary>
    public static class Extensions
    {
        /// <summary>Removes a value from the start and end of the string.</summary>
        /// <param name="source">The string to operate on.</param>
        /// <param name="value">The value to be trimmed.</param>
        /// <returns>A new string with the value trimmed.</returns>
        public static string Trim(this string source, string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return source;
            }

            return source.TrimStart(value).TrimEnd(value);
        }

        /// <summary>Removes a value from only the start of the string.</summary>
        /// <param name="source">The string to operate on.</param>
        /// <param name="value">The value to be trimmed.</param>
        /// <returns>A new string with the value trimmed.</returns>
        public static string TrimStart(this string source, string? value)
        {
            if (string.IsNullOrEmpty(value) || !source.StartsWith(value))
            {
                return source;
            }

            return source[value.Length..];
        }

        /// <summary>Removes a value from only the end of the string.</summary>
        /// <param name="source">The string to operate on.</param>
        /// <param name="value">The value to be trimmed.</param>
        /// <returns>A new string with the value trimmed.</returns>
        public static string TrimEnd(this string source, string? value)
        {
            if (string.IsNullOrEmpty(value) || !source.EndsWith(value))
            {
                return source;
            }

            return source.Remove(source.LastIndexOf(value));
        }
    }
}