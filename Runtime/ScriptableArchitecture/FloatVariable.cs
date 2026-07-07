using UnityEngine;

namespace CatAnnaDev.ScriptableArchitecture
{
    [CreateAssetMenu(menuName = "CatAnnaDev/Variables/Float", fileName = "FloatVariable")]
    public sealed class FloatVariable : Variable<float>
    {
        public void Add(float amount)
        {
            SetValue(Value + amount);
        }

        public static implicit operator float(FloatVariable variable)
        {
            return variable == null ? 0f : variable.Value;
        }
    }
}
