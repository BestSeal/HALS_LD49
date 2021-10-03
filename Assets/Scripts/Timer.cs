using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    GameObject gameobject;
    public float timestart = 60;
    public Text timertext;
    // Start is called before the first frame update
    void Start()
    {
        timertext.text = timestart.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        timestart -= Time.deltaTime;
        timertext.text = Mathf.Round(timestart).ToString();
    }
}
