using System;
using UnityEngine;

namespace CatAnnaDev.ScriptableArchitecture
{
    [Serializable]
    public struct BoolReference
    {
        [SerializeField] private bool useConstant;
        [SerializeField] private bool constantValue;
        [SerializeField] private BoolVariable variable;

        public BoolReference(bool value)
        {
            useConstant = true;
            constantValue = value;
            variable = null;
        }

        public BoolReference(BoolVariable source)
        {
            useConstant = false;
            constantValue = false;
            variable = source;
        }

        public bool UseConstant
        {
            get { return useConstant; }
            set { useConstant = value; }
        }

        public BoolVariable Variable
        {
            get { return variable; }
            set { variable = value; }
        }

        public bool Value
        {
            get { return useConstant || variable == null ? constantValue : variable.Value; }
        }

        public void SetValue(bool value)
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

        public static implicit operator bool(BoolReference reference)
        {
            return reference.Value;
        }
    }
}
