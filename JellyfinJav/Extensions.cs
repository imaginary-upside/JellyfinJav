#pragma warning disable SA1600

namespace JellyfinJav
{
    public static class Extensions
    {
        public static string Trim(this string source, string value)
        {
            return source.TrimStart(value).TrimEnd(value);
        }

        public static string TrimStart(this string source, string value)
        {
            if (value == null || !source.StartsWith(value))
            {
                return source;
            }

            return source[value.Length..];
        }

        public static string TrimEnd(this string source, string value)
        {
            if (value == null || !source.EndsWith(value))
            {
                return source;
            }

            return source.Remove(source.LastIndexOf(value));
        }
    }
}