using UnityEngine;

namespace CatAnnaDev.ScriptableArchitecture
{
    [CreateAssetMenu(menuName = "CatAnnaDev/Variables/Vector2", fileName = "Vector2Variable")]
    public sealed class Vector2Variable : Variable<Vector2>
    {
        public static implicit operator Vector2(Vector2Variable variable)
        {
            return variable == null ? Vector2.zero : variable.Value;
        }
    }
}
