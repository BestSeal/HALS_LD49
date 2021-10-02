using UnityEngine;

namespace Tests.TestPlatformSettings
{
    [CreateAssetMenu(fileName = "PlatformStage", menuName = "ScriptableObjects/PlatformStage", order = 1)]
    public class PlatformStage : ScriptableObject
    {
        public float criticalWeightUntilProceedToNextStage;
        public float stageRotation;
    }
}
