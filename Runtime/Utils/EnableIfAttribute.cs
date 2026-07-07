using System;
using UnityEngine;

namespace CatAnnaDev.Utils
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class EnableIfAttribute : PropertyAttribute
    {
        public readonly string ConditionMember;
        public readonly bool Invert;

        public EnableIfAttribute(string conditionMember)
        {
            ConditionMember = conditionMember;
            Invert = false;
        }

        public EnableIfAttribute(string conditionMember, bool invert)
        {
            ConditionMember = conditionMember;
            Invert = invert;
        }
    }
}
