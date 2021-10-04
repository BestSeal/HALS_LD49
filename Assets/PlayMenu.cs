using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayMenu : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image timer;
    
    [SerializeField] private GameObject victoryMessage;
    [SerializeField] private GameObject defeatMessage;

    public void Start()
    {
        victoryMessage.SetActive(false);
        defeatMessage.SetActive(false);
    }

    public void TimeUp()
    {
        HideGameplayUI();
        
        victoryMessage.SetActive(true);
    }

    public void Defeat()
    {
        HideGameplayUI();
        
        defeatMessage.SetActive(true);
    }

    public void HideGameplayUI()
    {
        slider.gameObject.SetActive(false);
        timer.gameObject.SetActive(false);
    }
}
