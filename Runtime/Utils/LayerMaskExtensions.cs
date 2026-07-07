using UnityEngine;

namespace CatAnnaDev.Utils
{
    public static class LayerMaskExtensions
    {
        public static bool Contains(this LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) != 0;
        }

        public static bool ContainsGameObject(this LayerMask mask, GameObject gameObject)
        {
            return (mask.value & (1 << gameObject.layer)) != 0;
        }

        public static LayerMask Add(this LayerMask mask, int layer)
        {
            mask.value |= 1 << layer;
            return mask;
        }

        public static LayerMask Remove(this LayerMask mask, int layer)
        {
            mask.value &= ~(1 << layer);
            return mask;
        }

        public static LayerMask Toggle(this LayerMask mask, int layer)
        {
            mask.value ^= 1 << layer;
            return mask;
        }

        public static bool IsEverything(this LayerMask mask)
        {
            return mask.value == ~0;
        }

        public static bool IsNothing(this LayerMask mask)
        {
            return mask.value == 0;
        }

        public static int FirstLayer(this LayerMask mask)
        {
            int value = mask.value;
            if (value == 0)
            {
                return -1;
            }
            int index = 0;
            while ((value & 1) == 0)
            {
                value >>= 1;
                index++;
            }
            return index;
        }

        public static LayerMask Everything()
        {
            return ~0;
        }

        public static LayerMask Nothing()
        {
            return 0;
        }
    }
}
