using System;
using UnityEngine;

namespace CatAnnaDev.Utils
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class MinMaxSliderAttribute : PropertyAttribute
    {
        public readonly float Min;
        public readonly float Max;
        public readonly bool ShowValues;

        public MinMaxSliderAttribute(float min, float max)
        {
            Min = min;
            Max = max;
            ShowValues = true;
        }

        public MinMaxSliderAttribute(float min, float max, bool showValues)
        {
            Min = min;
            Max = max;
            ShowValues = showValues;
        }
    }
}
