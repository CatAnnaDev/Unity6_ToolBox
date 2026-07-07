using System;
using UnityEngine;

namespace CatAnnaDev.Utils
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class ReadOnlyAttribute : PropertyAttribute
    {
        public readonly bool OnlyWhilePlaying;

        public ReadOnlyAttribute()
        {
            OnlyWhilePlaying = false;
        }

        public ReadOnlyAttribute(bool onlyWhilePlaying)
        {
            OnlyWhilePlaying = onlyWhilePlaying;
        }
    }
}
