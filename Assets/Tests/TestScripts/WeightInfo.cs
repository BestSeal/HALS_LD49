using UnityEngine;

namespace Tests.TestScripts
{
    public class WeightInfo : MonoBehaviour
    {
        [SerializeField] private float weight;

        public float GetWeight => weight;
    }
}
