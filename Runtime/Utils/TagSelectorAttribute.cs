using System;
using UnityEngine;

namespace CatAnnaDev.Utils
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class TagSelectorAttribute : PropertyAttribute
    {
        public readonly bool IncludeUntagged;

        public TagSelectorAttribute()
        {
            IncludeUntagged = true;
        }

        public TagSelectorAttribute(bool includeUntagged)
        {
            IncludeUntagged = includeUntagged;
        }
    }
}
