using UnityEngine;
using UnityEngine.UI;

public class HintClose : MonoBehaviour
{
    [SerializeField] private Image hintToClose;
    
    public void CloseHint()
    {
        hintToClose.gameObject.SetActive(false);
    }
}
