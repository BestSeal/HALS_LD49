using Tests.TestScripts;
using UnityEngine;
using UnityEngine.Events;

public class FallZoneBehavior : MonoBehaviour
{
    [SerializeField] private UnityEvent playerFallEvent;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TestInputHandler>() != null)
        {
            playerFallEvent.Invoke();
        }
    }
}
