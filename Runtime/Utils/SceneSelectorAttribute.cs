using System;
using UnityEngine;

namespace CatAnnaDev.Utils
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class SceneSelectorAttribute : PropertyAttribute
    {
        public readonly bool OnlyBuildScenes;

        public SceneSelectorAttribute()
        {
            OnlyBuildScenes = true;
        }

        public SceneSelectorAttribute(bool onlyBuildScenes)
        {
            OnlyBuildScenes = onlyBuildScenes;
        }
    }
}
