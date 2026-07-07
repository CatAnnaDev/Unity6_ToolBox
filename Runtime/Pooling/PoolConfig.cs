using UnityEngine;

namespace CatAnnaDev.Pooling
{
    [CreateAssetMenu(menuName = "CatAnnaDev/Pooling/Pool Config", fileName = "PoolConfig")]
    public sealed class PoolConfig : ScriptableObject
    {
        [SerializeField] GameObject prefab;
        [SerializeField, Min(0)] int prewarmCount;
        [SerializeField] ExpandPolicy expandPolicy = ExpandPolicy.Default;
        [SerializeField, Min(0)] int maxSize;
        [SerializeField] bool cullExcess;
        [SerializeField, Min(0f)] float cullCheckInterval = 5f;
        [SerializeField] bool persistAcrossScenes = true;
        [SerializeField] string id;
        [SerializeField] string category;

        public GameObject Prefab => prefab;
        public int PrewarmCount => prewarmCount;
        public ExpandPolicy ExpandPolicy => expandPolicy;
        public int MaxSize => maxSize;
        public bool CullExcess => cullExcess;
        public float CullCheckInterval => cullCheckInterval;
        public bool PersistAcrossScenes => persistAcrossScenes;
        public string Id => id;
        public string Category => category;

        public bool IsValid => prefab != null;
    }
}
