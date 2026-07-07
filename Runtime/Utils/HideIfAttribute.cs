using System;
using UnityEngine;

namespace CatAnnaDev.Utils
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class HideIfAttribute : PropertyAttribute
    {
        public readonly string ConditionMember;

        public HideIfAttribute(string conditionMember)
        {
            ConditionMember = conditionMember;
        }
    }
}
