using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class menu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    [SerializeField] InputAction pausebutton;

    // Update is called once per frame
    void Update()
    {
        if (pausebutton.triggered)
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume(){
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = true;
    }
    void Pause(){
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void credits(){
        Debug.Log("credits");
    }

    public void Exit(){
        Debug.Log("exit");
    }
}
