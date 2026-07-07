using System;
using UnityEngine;

namespace CatAnnaDev.Utils
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class ShowIfAttribute : PropertyAttribute
    {
        public readonly string ConditionMember;
        public readonly bool Invert;

        public ShowIfAttribute(string conditionMember)
        {
            ConditionMember = conditionMember;
            Invert = false;
        }

        public ShowIfAttribute(string conditionMember, bool invert)
        {
            ConditionMember = conditionMember;
            Invert = invert;
        }
    }
}
