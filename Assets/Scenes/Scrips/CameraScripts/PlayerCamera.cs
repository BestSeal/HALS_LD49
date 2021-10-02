using UnityEngine;

namespace Scenes.Scrips.CameraScripts
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float followSpeed;
        [SerializeField] private Vector3 cameraOffset;

        private void LateUpdate()
        {
            var destinationPosition = playerTransform.position + cameraOffset;
            transform.position = Vector3.Lerp(transform.position, destinationPosition, followSpeed);
        }
    }
}