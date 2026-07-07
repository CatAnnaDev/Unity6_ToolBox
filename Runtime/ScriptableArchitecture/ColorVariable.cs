using UnityEngine;

namespace CatAnnaDev.ScriptableArchitecture
{
    [CreateAssetMenu(menuName = "CatAnnaDev/Variables/Color", fileName = "ColorVariable")]
    public sealed class ColorVariable : Variable<Color>
    {
        public static implicit operator Color(ColorVariable variable)
        {
            return variable == null ? Color.white : variable.Value;
        }
    }
}
