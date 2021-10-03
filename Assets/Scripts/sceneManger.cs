using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManger : MonoBehaviour
{
    public void NextLevel (int _levelNumber){
        SceneManager.LoadScene( _levelNumber);
    }
}
