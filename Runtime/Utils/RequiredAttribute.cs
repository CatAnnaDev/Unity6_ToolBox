using System;
using UnityEngine;

namespace CatAnnaDev.Utils
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class RequiredAttribute : PropertyAttribute
    {
        public readonly string Message;

        public RequiredAttribute()
        {
            Message = null;
        }

        public RequiredAttribute(string message)
        {
            Message = message;
        }
    }
}
