using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    //[SerializeField] sceneManger script;
    [SerializeField] GameObject UI;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject end;
    GameObject gameobject;
    [SerializeField] float timestart = 60;
    [SerializeField] TextMeshProUGUI timertext;
    // Start is called before the first frame update
    void Start()
    {
        timertext.text = timestart.ToString();
        Time.timeScale = 1f;
        /*menu.SetActive(true);
        menu.SetActive(false);*/
    }

    // Update is called once per frame
    void Update()
    {
        timestart -= Time.deltaTime;
        timertext.text = Mathf.Round(timestart).ToString();
        if (timertext.text == "0"){
            Time.timeScale = 0f;
            menu.SetActive(false);
            UI.SetActive(false);
            end.SetActive(true);
        }
    }
}
