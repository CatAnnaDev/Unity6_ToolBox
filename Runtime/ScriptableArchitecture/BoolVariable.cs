using UnityEngine;

namespace CatAnnaDev.ScriptableArchitecture
{
    [CreateAssetMenu(menuName = "CatAnnaDev/Variables/Bool", fileName = "BoolVariable")]
    public sealed class BoolVariable : Variable<bool>
    {
        public void Toggle()
        {
            SetValue(!Value);
        }

        public static implicit operator bool(BoolVariable variable)
        {
            return variable != null && variable.Value;
        }
    }
}
