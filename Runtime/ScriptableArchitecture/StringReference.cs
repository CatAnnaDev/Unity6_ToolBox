using System;
using UnityEngine;

namespace CatAnnaDev.ScriptableArchitecture
{
    [Serializable]
    public struct StringReference
    {
        [SerializeField] private bool useConstant;
        [SerializeField] private string constantValue;
        [SerializeField] private StringVariable variable;

        public StringReference(string value)
        {
            useConstant = true;
            constantValue = value;
            variable = null;
        }

        public StringReference(StringVariable source)
        {
            useConstant = false;
            constantValue = string.Empty;
            variable = source;
        }

        public bool UseConstant
        {
            get { return useConstant; }
            set { useConstant = value; }
        }

        public StringVariable Variable
        {
            get { return variable; }
            set { variable = value; }
        }

        public string Value
        {
            get { return useConstant || variable == null ? constantValue : variable.Value; }
        }

        public void SetValue(string value)
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

        public static implicit operator string(StringReference reference)
        {
            return reference.Value;
        }
    }
}
