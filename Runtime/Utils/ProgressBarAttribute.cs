using System;
using UnityEngine;

namespace CatAnnaDev.Utils
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class ProgressBarAttribute : PropertyAttribute
    {
        public readonly string MaxValueMember;
        public readonly float MaxValue;
        public readonly string Label;
        public readonly float ColorR;
        public readonly float ColorG;
        public readonly float ColorB;

        public ProgressBarAttribute(float maxValue)
        {
            MaxValue = maxValue;
            MaxValueMember = null;
            Label = null;
            ColorR = 0.26f;
            ColorG = 0.60f;
            ColorB = 0.88f;
        }

        public ProgressBarAttribute(string maxValueMember)
        {
            MaxValueMember = maxValueMember;
            MaxValue = 1f;
            Label = null;
            ColorR = 0.26f;
            ColorG = 0.60f;
            ColorB = 0.88f;
        }

        public ProgressBarAttribute(float maxValue, string label)
        {
            MaxValue = maxValue;
            MaxValueMember = null;
            Label = label;
            ColorR = 0.26f;
            ColorG = 0.60f;
            ColorB = 0.88f;
        }

        public Color BarColor
        {
            get { return new Color(ColorR, ColorG, ColorB, 1f); }
        }
    }
}
