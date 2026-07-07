using System.Text;
using UnityEngine;

namespace CatAnnaDev.Utils
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool HasValue(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static string Truncate(this string value, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            {
                return value;
            }
            if (maxLength <= suffix.Length)
            {
                return value.Substring(0, maxLength);
            }
            return string.Concat(value.Substring(0, maxLength - suffix.Length), suffix);
        }

        public static string ToTitleCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            StringBuilder builder = new StringBuilder(value.Length);
            bool newWord = true;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (char.IsWhiteSpace(c) || c == '_' || c == '-')
                {
                    newWord = true;
                    builder.Append(' ');
                }
                else if (newWord)
                {
                    builder.Append(char.ToUpperInvariant(c));
                    newWord = false;
                }
                else
                {
                    builder.Append(char.ToLowerInvariant(c));
                }
            }
            return builder.ToString();
        }

        public static string NicifyName(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            StringBuilder builder = new StringBuilder(value.Length + 8);
            int start = 0;
            if (value.Length > 1 && value[0] == 'm' && value[1] == '_')
            {
                start = 2;
            }
            else if (value.Length > 0 && value[0] == '_')
            {
                start = 1;
            }
            bool first = true;
            for (int i = start; i < value.Length; i++)
            {
                char c = value[i];
                if (first)
                {
                    builder.Append(char.ToUpperInvariant(c));
                    first = false;
                    continue;
                }
                if (char.IsUpper(c) && i > start && !char.IsUpper(value[i - 1]))
                {
                    builder.Append(' ');
                }
                builder.Append(c);
            }
            return builder.ToString();
        }

        public static string StripRichText(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            StringBuilder builder = new StringBuilder(value.Length);
            bool insideTag = false;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (c == '<')
                {
                    insideTag = true;
                }
                else if (c == '>')
                {
                    insideTag = false;
                }
                else if (!insideTag)
                {
                    builder.Append(c);
                }
            }
            return builder.ToString();
        }

        public static string Colorize(this string value, Color color)
        {
            return string.Concat("<color=#", ColorUtility.ToHtmlStringRGBA(color), ">", value, "</color>");
        }

        public static string Bold(this string value)
        {
            return string.Concat("<b>", value, "</b>");
        }

        public static string Italic(this string value)
        {
            return string.Concat("<i>", value, "</i>");
        }

        public static string Size(this string value, int size)
        {
            return string.Concat("<size=", size.ToString(), ">", value, "</size>");
        }

        public static string RemoveWhitespace(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            StringBuilder builder = new StringBuilder(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i]))
                {
                    builder.Append(value[i]);
                }
            }
            return builder.ToString();
        }

        public static string EnsurePrefix(this string value, string prefix)
        {
            if (string.IsNullOrEmpty(value) || value.StartsWith(prefix))
            {
                return value;
            }
            return string.Concat(prefix, value);
        }

        public static string EnsureSuffix(this string value, string suffix)
        {
            if (string.IsNullOrEmpty(value) || value.EndsWith(suffix))
            {
                return value;
            }
            return string.Concat(value, suffix);
        }
    }
}
