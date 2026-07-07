using System;
using UnityEngine;

namespace CatAnnaDev.ScriptableArchitecture
{
    [Serializable]
    public struct IntReference
    {
        [SerializeField] private bool useConstant;
        [SerializeField] private int constantValue;
        [SerializeField] private IntVariable variable;

        public IntReference(int value)
        {
            useConstant = true;
            constantValue = value;
            variable = null;
        }

        public IntReference(IntVariable source)
        {
            useConstant = false;
            constantValue = 0;
            variable = source;
        }

        public bool UseConstant
        {
            get { return useConstant; }
            set { useConstant = value; }
        }

        public IntVariable Variable
        {
            get { return variable; }
            set { variable = value; }
        }

        public int Value
        {
            get { return useConstant || variable == null ? constantValue : variable.Value; }
        }

        public void SetValue(int value)
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

        public static implicit operator int(IntReference reference)
        {
            return reference.Value;
        }
    }
}
