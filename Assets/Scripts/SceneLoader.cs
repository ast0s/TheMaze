using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void StartNewGame()
    {
        Debug.Log("New game scene loaded");
        SceneManager.LoadScene("GameScene");
    }
    public void ContinueGame()
    {
        Debug.Log("Previous game scene loaded");
        //SceneManager.LoadScene("GameScene");
    }
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
