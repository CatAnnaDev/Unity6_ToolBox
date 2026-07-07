using UnityEngine;

namespace CatAnnaDev.ScriptableArchitecture
{
    [CreateAssetMenu(menuName = "CatAnnaDev/Variables/GameObject", fileName = "GameObjectVariable")]
    public sealed class GameObjectVariable : Variable<GameObject>
    {
        public bool HasValue
        {
            get { return Value != null; }
        }

        public static implicit operator GameObject(GameObjectVariable variable)
        {
            return variable == null ? null : variable.Value;
        }
    }
}
