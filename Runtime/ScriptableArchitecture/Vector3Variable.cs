using UnityEngine;

namespace CatAnnaDev.ScriptableArchitecture
{
    [CreateAssetMenu(menuName = "CatAnnaDev/Variables/Vector3", fileName = "Vector3Variable")]
    public sealed class Vector3Variable : Variable<Vector3>
    {
        public static implicit operator Vector3(Vector3Variable variable)
        {
            return variable == null ? Vector3.zero : variable.Value;
        }
    }
}
