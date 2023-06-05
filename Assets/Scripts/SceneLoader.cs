using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static bool GameIsPaused = false;
    [SerializeField] private GameObject _pauseUI;
    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private GameObject _settingsMenuUI;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void ResumeGame()
    {
        Debug.Log("The game continues");
        _settingsMenuUI.SetActive(false);
        _pauseMenuUI.SetActive(true);
        _pauseUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    public void PauseGame()
    {
        Debug.Log("Game paused");
        Time.timeScale = 0f;
        GameIsPaused = true;
        _pauseUI.SetActive(true);
    }
    
    public void StartNewGame()
    {
        Debug.Log("New game started");
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene("GameScene");
    }
    public void ContinueGame()
    {
        Debug.Log("Previous game continued");
        Time.timeScale = 1f;
        GameIsPaused = false;
        //SceneManager.LoadScene("GameScene");
    }
    public void QuitToMenu()
    {
        Debug.Log("Quitting to main menu");
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene("MainMenuScene");
    }
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
