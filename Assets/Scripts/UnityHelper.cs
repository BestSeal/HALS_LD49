using UnityEngine;

namespace Tests
{
    public static class UnityHelper
    {
        public static bool IsObjectInLayerMask(GameObject gameObject, LayerMask layerMask)
            => layerMask == (layerMask | (1 << gameObject.layer));
        
    }
}