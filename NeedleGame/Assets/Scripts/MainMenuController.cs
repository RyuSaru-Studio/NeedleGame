using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject startMenuPanel;
    public GameObject levelMenuPanel;
    public GameObject settingsMenuPanel;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioClip startMenuMusic;
    public AudioClip levelMenuMusic;

    private GameObject previousPanel;

    private void Start()
    {
        if (PlayerPrefs.GetInt("OpenLevelMenuOnStart", 0) == 1)
        {
            PlayerPrefs.SetInt("OpenLevelMenuOnStart", 0);
            ShowLevelMenu();
        }
        else
        {
            ShowStartMenu();
        }
    }

    public void ShowStartMenu()
    {
        startMenuPanel.SetActive(true);
        levelMenuPanel.SetActive(false);
        settingsMenuPanel.SetActive(false);

        PlayMusic(startMenuMusic);
    }

    public void ShowLevelMenu()
    {
        startMenuPanel.SetActive(false);
        levelMenuPanel.SetActive(true);
        settingsMenuPanel.SetActive(false);

        PlayMusic(levelMenuMusic);
    }

    public void OpenSettingsFromStartMenu()
    {
        previousPanel = startMenuPanel;

        startMenuPanel.SetActive(false);
        levelMenuPanel.SetActive(false);
        settingsMenuPanel.SetActive(true);
    }

    public void OpenSettingsFromLevelMenu()
    {
        previousPanel = levelMenuPanel;

        startMenuPanel.SetActive(false);
        levelMenuPanel.SetActive(false);
        settingsMenuPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsMenuPanel.SetActive(false);

        if (previousPanel != null)
        {
            previousPanel.SetActive(true);
        }
        else
        {
            ShowStartMenu();
        }
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    private void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null)
            return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }
}