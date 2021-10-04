using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManger : MonoBehaviour
{
    public static int currentSceneNumber;

    public void NextLevel ()
    {
        SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void Restart ()
    {
        var currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
