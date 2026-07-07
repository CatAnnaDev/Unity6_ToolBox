using System;
using UnityEngine;

namespace CatAnnaDev.Utils
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class TitleAttribute : PropertyAttribute
    {
        public readonly string Text;
        public readonly bool DrawSeparator;

        public TitleAttribute(string text)
        {
            Text = text;
            DrawSeparator = true;
        }

        public TitleAttribute(string text, bool drawSeparator)
        {
            Text = text;
            DrawSeparator = drawSeparator;
        }
    }
}
