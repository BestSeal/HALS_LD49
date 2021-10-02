using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    IEnumerator ShakeCamera(float shakeDuration, float shakeMagnitude)
    {
        var initialPos = transform.localPosition;

        float elapsedTime = 0f;
        
        while (elapsedTime < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.localPosition = new Vector3(x, y, initialPos.z);

            yield return null;
        }
    }
}
