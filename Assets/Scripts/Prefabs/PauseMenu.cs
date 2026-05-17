using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject optionsPanel;
    public GameObject confirmExitPanel;

    public Slider volumeSlider;

    private bool isPaused = false;

    private void Start()
    {
        pausePanel.SetActive(false);

        float volume = PlayerPrefs.GetFloat("VOLUME", 1f);
        volumeSlider.value = volume;

        AudioListener.volume = volume;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
            return;


        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (confirmExitPanel.activeSelf)
            {
                CloseExitConfirm();
            }
            else if (optionsPanel.activeSelf)
            {
                CloseOptions();
            }
            else if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        optionsPanel.SetActive(false);
        confirmExitPanel.SetActive(false);

        Time.timeScale = 0f;

        isPaused = true;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);

        Time.timeScale = 1f;

        isPaused = false;
    }

    public void OpenOptions()
    {
        CloseAllSubMenus();

        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }

    public void OpenExitConfirm()
    {
        CloseAllSubMenus();

        confirmExitPanel.SetActive(true);
    }

    public void CloseExitConfirm()
    {
        confirmExitPanel.SetActive(false);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }

    public void ChangeVolume(float value)
    {
        AudioListener.volume = value;

        PlayerPrefs.SetFloat("VOLUME", value);
        PlayerPrefs.Save();
    }

    public void DeletePlayerData()
    {
        PlayerPrefs.DeleteAll();
    }

    public void CloseAllSubMenus()
    {
        optionsPanel.SetActive(false);

        confirmExitPanel.SetActive(false);
    }
}
