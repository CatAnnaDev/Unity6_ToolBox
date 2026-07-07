using System;
using UnityEngine;

namespace CatAnnaDev.Utils
{
    public enum InfoBoxType
    {
        None,
        Info,
        Warning,
        Error
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public sealed class InfoBoxAttribute : PropertyAttribute
    {
        public readonly string Text;
        public readonly InfoBoxType Type;

        public InfoBoxAttribute(string text)
        {
            Text = text;
            Type = InfoBoxType.Info;
        }

        public InfoBoxAttribute(string text, InfoBoxType type)
        {
            Text = text;
            Type = type;
        }
    }
}
