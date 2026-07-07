using UnityEngine;

namespace CatAnnaDev.ScriptableArchitecture
{
    [CreateAssetMenu(menuName = "CatAnnaDev/Variables/Int", fileName = "IntVariable")]
    public sealed class IntVariable : Variable<int>
    {
        public void Add(int amount)
        {
            SetValue(Value + amount);
        }

        public static implicit operator int(IntVariable variable)
        {
            return variable == null ? 0 : variable.Value;
        }
    }
}
