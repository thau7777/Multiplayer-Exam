using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private bool isPaused;

    private void Start()
    {
        // Hide pause panel & cursor when game starts
        pausePanel.SetActive(false);
        if (SceneManager.GetActiveScene().buildIndex == 2)
            SetCursorVisible(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                ShowPauseMenu();
        }
    }

    private void ShowPauseMenu()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        SetCursorVisible(true);
    }

    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);

        if(SceneManager.GetActiveScene().buildIndex == 2)
            SetCursorVisible(false);
    }

    private void SetCursorVisible(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
