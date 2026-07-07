using System;
using UnityEngine;

namespace CatAnnaDev.ScriptableArchitecture
{
    [Serializable]
    public struct Vector3Reference
    {
        [SerializeField] private bool useConstant;
        [SerializeField] private Vector3 constantValue;
        [SerializeField] private Vector3Variable variable;

        public Vector3Reference(Vector3 value)
        {
            useConstant = true;
            constantValue = value;
            variable = null;
        }

        public Vector3Reference(Vector3Variable source)
        {
            useConstant = false;
            constantValue = Vector3.zero;
            variable = source;
        }

        public bool UseConstant
        {
            get { return useConstant; }
            set { useConstant = value; }
        }

        public Vector3Variable Variable
        {
            get { return variable; }
            set { variable = value; }
        }

        public Vector3 Value
        {
            get { return useConstant || variable == null ? constantValue : variable.Value; }
        }

        public void SetValue(Vector3 value)
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

        public static implicit operator Vector3(Vector3Reference reference)
        {
            return reference.Value;
        }
    }
}
