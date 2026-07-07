using UnityEngine;

namespace CatAnnaDev.ScriptableArchitecture
{
    [CreateAssetMenu(menuName = "CatAnnaDev/Variables/String", fileName = "StringVariable")]
    public sealed class StringVariable : Variable<string>
    {
        public void Append(string suffix)
        {
            SetValue(Value + suffix);
        }

        public static implicit operator string(StringVariable variable)
        {
            return variable == null ? null : variable.Value;
        }
    }
}
