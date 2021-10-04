using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManger : MonoBehaviour
{
    public static int currentSceneNumber;

    public void NextLevel (int _levelNumber)
    {
        currentSceneNumber = _levelNumber;
        SceneManager.LoadScene( _levelNumber);
    }
    
    public void Restart ()
    {
        var currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
