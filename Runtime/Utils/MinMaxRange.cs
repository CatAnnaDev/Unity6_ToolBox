using System;
using UnityEngine;

namespace CatAnnaDev.Utils
{
    [Serializable]
    public struct MinMaxRange
    {
        [SerializeField]
        private float min;

        [SerializeField]
        private float max;

        public MinMaxRange(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public float Min
        {
            get { return min; }
            set { min = value; }
        }

        public float Max
        {
            get { return max; }
            set { max = value; }
        }

        public float Length
        {
            get { return max - min; }
        }

        public float Center
        {
            get { return (min + max) * 0.5f; }
        }

        public float Random
        {
            get { return UnityEngine.Random.Range(min, max); }
        }

        public float Sample(ref RandomState state)
        {
            return state.NextFloat(min, max);
        }

        public float Lerp(float t)
        {
            return Mathf.Lerp(min, max, t);
        }

        public float InverseLerp(float value)
        {
            return Mathf.InverseLerp(min, max, value);
        }

        public float Clamp(float value)
        {
            return Mathf.Clamp(value, min, max);
        }

        public bool Contains(float value)
        {
            return value >= min && value <= max;
        }
    }

    [Serializable]
    public struct MinMaxRangeInt
    {
        [SerializeField]
        private int min;

        [SerializeField]
        private int max;

        public MinMaxRangeInt(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public int Min
        {
            get { return min; }
            set { min = value; }
        }

        public int Max
        {
            get { return max; }
            set { max = value; }
        }

        public int Length
        {
            get { return max - min; }
        }

        public int Random
        {
            get { return UnityEngine.Random.Range(min, max + 1); }
        }

        public int Sample(ref RandomState state)
        {
            return state.NextInt(min, max + 1);
        }

        public int Clamp(int value)
        {
            return Mathf.Clamp(value, min, max);
        }

        public bool Contains(int value)
        {
            return value >= min && value <= max;
        }
    }
}
