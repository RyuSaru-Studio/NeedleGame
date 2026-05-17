using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelUIController : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject pauseMenuPanel;
    public GameObject pauseMainContent;
    public GameObject pauseSettingsContent;

    [Header("Endscreen")]
    public GameObject endscreenPanel;

    [Header("Scene Names")]
    public string mainMenuSceneName = "StartMenu";
    public string nextLevelName = "Level2";

    [Header("Audio")]
    public AudioSource ambienceSourceOne;
    public AudioSource ambienceSourceTwo;
    public AudioSource endscreenMusicSource;

    public AudioClip ambienceClipOne;
    public AudioClip ambienceClipTwo;
    public AudioClip endscreenMusicClip;

    private bool isPaused = false;
    private bool levelEnded = false;

    private void Start()
    {
        Time.timeScale = 1f;

        pauseMenuPanel.SetActive(false);
        endscreenPanel.SetActive(false);

        pauseMainContent.SetActive(true);
        pauseSettingsContent.SetActive(false);

        StartLevelAmbience();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !levelEnded)
        {
            if (!isPaused)
            {
                OpenPauseMenu();
            }
            else
            {
                if (pauseSettingsContent.activeSelf)
                {
                    CloseSettingsToPauseMenu();
                }
                else
                {
                    ResumeGame();
                }
            }
        }
    }

    public void OpenPauseMenu()
    {
        isPaused = true;
        Time.timeScale = 0f;

        pauseMenuPanel.SetActive(true);
        endscreenPanel.SetActive(false);

        pauseMainContent.SetActive(true);
        pauseSettingsContent.SetActive(false);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        pauseMenuPanel.SetActive(false);

        pauseMainContent.SetActive(true);
        pauseSettingsContent.SetActive(false);
    }

    public void OpenSettingsInsidePauseMenu()
    {
        pauseMainContent.SetActive(false);
        pauseSettingsContent.SetActive(true);
    }

    public void CloseSettingsToPauseMenu()
    {
        pauseSettingsContent.SetActive(false);
        pauseMainContent.SetActive(true);
    }

    public void RestartLevel()
    {
        AudioManager.Instance?.PlayClick();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToLevelMenu()
    {
        AudioManager.Instance?.PlayClick();
        Time.timeScale = 1f;

        PlayerPrefs.SetInt("OpenLevelMenuOnStart", 1);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void CompleteLevel()
    {
        levelEnded = true;
        isPaused = false;
        Time.timeScale = 0f;

        pauseMenuPanel.SetActive(false);
        endscreenPanel.SetActive(true);

        StopLevelAmbience();
        PlayEndscreenMusic();
    }

    public void LoadNextLevel()
    {
        AudioManager.Instance?.PlayClick();
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextLevelName);
    }

    private void StartLevelAmbience()
    {
        PlayLoop(ambienceSourceOne, ambienceClipOne);
        PlayLoop(ambienceSourceTwo, ambienceClipTwo);
    }

    private void StopLevelAmbience()
    {
        if (ambienceSourceOne != null)
            ambienceSourceOne.Stop();

        if (ambienceSourceTwo != null)
            ambienceSourceTwo.Stop();
    }

    private void PlayEndscreenMusic()
    {
        PlayLoop(endscreenMusicSource, endscreenMusicClip);
    }

    private void PlayLoop(AudioSource source, AudioClip clip)
    {
        if (source == null || clip == null)
            return;

        source.clip = clip;
        source.loop = true;
        source.Play();
    }
}