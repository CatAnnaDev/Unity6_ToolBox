using UnityEngine;

namespace CatAnnaDev.ScriptableArchitecture
{
    [AddComponentMenu("CatAnnaDev/Scriptable Architecture/Runtime Set Member")]
    [DisallowMultipleComponent]
    public sealed class RuntimeSetMember : MonoBehaviour
    {
        [SerializeField] private GameObjectRuntimeSet gameObjectSet;
        [SerializeField] private TransformRuntimeSet transformSet;

        public GameObjectRuntimeSet GameObjectSet
        {
            get { return gameObjectSet; }
            set
            {
                if (gameObjectSet == value)
                {
                    return;
                }

                if (isActiveAndEnabled && gameObjectSet != null)
                {
                    gameObjectSet.Remove(gameObject);
                }

                gameObjectSet = value;

                if (isActiveAndEnabled && gameObjectSet != null)
                {
                    gameObjectSet.Add(gameObject);
                }
            }
        }

        public TransformRuntimeSet TransformSet
        {
            get { return transformSet; }
            set
            {
                if (transformSet == value)
                {
                    return;
                }

                if (isActiveAndEnabled && transformSet != null)
                {
                    transformSet.Remove(transform);
                }

                transformSet = value;

                if (isActiveAndEnabled && transformSet != null)
                {
                    transformSet.Add(transform);
                }
            }
        }

        private void OnEnable()
        {
            if (gameObjectSet != null)
            {
                gameObjectSet.Add(gameObject);
            }

            if (transformSet != null)
            {
                transformSet.Add(transform);
            }
        }

        private void OnDisable()
        {
            if (gameObjectSet != null)
            {
                gameObjectSet.Remove(gameObject);
            }

            if (transformSet != null)
            {
                transformSet.Remove(transform);
            }
        }
    }
}
