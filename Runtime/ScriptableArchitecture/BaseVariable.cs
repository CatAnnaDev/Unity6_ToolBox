using System;
using System.Collections.Generic;
using UnityEngine;

namespace CatAnnaDev.ScriptableArchitecture
{
    public abstract class BaseVariable : ScriptableObject
    {
        [SerializeField] private bool resetOnPlay = true;

        public bool ResetOnPlay
        {
            get { return resetOnPlay; }
            set { resetOnPlay = value; }
        }

        public abstract Type ValueType { get; }

        public abstract object BoxedValue { get; set; }

        public abstract void ResetToInitial();
    }

    public abstract class Variable<T> : BaseVariable, ISerializationCallbackReceiver
    {
        [SerializeField] private T initialValue;
        [SerializeField] private T runtimeValue;

        public event Action<T> OnValueChanged;

        public T InitialValue
        {
            get { return initialValue; }
            set { initialValue = value; }
        }

        public T Value
        {
            get { return runtimeValue; }
            set { SetValue(value); }
        }

        public override Type ValueType
        {
            get { return typeof(T); }
        }

        public override object BoxedValue
        {
            get { return runtimeValue; }
            set { SetValue((T)value); }
        }

        public void SetValue(T newValue)
        {
            if (AreEqual(runtimeValue, newValue))
            {
                return;
            }

            runtimeValue = newValue;
            RaiseChanged();
        }

        public void SetValue(Variable<T> source)
        {
            if (source == null)
            {
                return;
            }

            SetValue(source.runtimeValue);
        }

        public void ForceSetValue(T newValue)
        {
            runtimeValue = newValue;
            RaiseChanged();
        }

        public void SetValueSilently(T newValue)
        {
            runtimeValue = newValue;
        }

        public override void ResetToInitial()
        {
            runtimeValue = initialValue;
        }

        protected void RaiseChanged()
        {
            Action<T> handler = OnValueChanged;
            if (handler != null)
            {
                handler.Invoke(runtimeValue);
            }
        }

        protected virtual bool AreEqual(T a, T b)
        {
            return EqualityComparer<T>.Default.Equals(a, b);
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (ResetOnPlay)
            {
                runtimeValue = initialValue;
            }
        }
    }
}
