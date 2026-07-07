using System;
using UnityEngine;

namespace CatAnnaDev.ScriptableArchitecture
{
    [Serializable]
    public struct FloatReference
    {
        [SerializeField] private bool useConstant;
        [SerializeField] private float constantValue;
        [SerializeField] private FloatVariable variable;

        public FloatReference(float value)
        {
            useConstant = true;
            constantValue = value;
            variable = null;
        }

        public FloatReference(FloatVariable source)
        {
            useConstant = false;
            constantValue = 0f;
            variable = source;
        }

        public bool UseConstant
        {
            get { return useConstant; }
            set { useConstant = value; }
        }

        public FloatVariable Variable
        {
            get { return variable; }
            set { variable = value; }
        }

        public float Value
        {
            get { return useConstant || variable == null ? constantValue : variable.Value; }
        }

        public void SetValue(float value)
        {
            if (!useConstant && variable != null)
            {
                variable.SetValue(value);
            }
            else
            {
                constantValue = value;
            }
        }

        public static implicit operator float(FloatReference reference)
        {
            return reference.Value;
        }
    }
}
