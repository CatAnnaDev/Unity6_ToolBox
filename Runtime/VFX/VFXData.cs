using UnityEngine;
using CatAnnaDev.Utils;

namespace CatAnnaDev.VFX
{
    [CreateAssetMenu(menuName = "CatAnnaDev/VFX/VFX Data", fileName = "VFXData")]
    public sealed class VFXData : ScriptableObject
    {
        [Title("Prefab")]
        [Required]
        [SerializeField] private GameObject prefab;

        [Title("Placement")]
        [SerializeField] private bool followTarget;
        [SerializeField] private bool alignToNormal;
        [SerializeField] private Vector3 positionOffset = Vector3.zero;
        [SerializeField] private Vector3 eulerOffset = Vector3.zero;
        [SerializeField] private bool randomizeYaw;

        [Title("Transform")]
        [SerializeField] private float scale = 1f;

        [Title("Lifetime")]
        [InfoBox("Set a lifetime override to force a return after N seconds. Leave at 0 to let the effect finish on its own.")]
        [SerializeField] private float lifetimeOverride = 0f;

        [Title("Pooling")]
        [SerializeField] private int prewarmCount = 0;

        public GameObject Prefab => prefab;
        public bool FollowTarget => followTarget;
        public bool AlignToNormal => alignToNormal;
        public Vector3 PositionOffset => positionOffset;
        public Vector3 EulerOffset => eulerOffset;
        public bool RandomizeYaw => randomizeYaw;
        public float Scale => scale <= 0f ? 1f : scale;
        public float LifetimeOverride => lifetimeOverride < 0f ? 0f : lifetimeOverride;
        public int PrewarmCount => prewarmCount < 0 ? 0 : prewarmCount;

        public bool IsValid => prefab != null;
    }
}
