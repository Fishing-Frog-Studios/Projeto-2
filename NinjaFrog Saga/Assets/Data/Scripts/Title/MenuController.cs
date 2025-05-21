using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class MenuManagement : MonoBehaviour
{
    [Header("Menus")]
    public string newGameScene;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject options;

    [Header("Áudio")]
    public Slider volumeSlider;
    public AudioSource volumeAudio;

    [Header("Tela")]
    public TextMeshProUGUI isFullscreenOn;
    public TextMeshProUGUI resolutionValue;
    public static List<string> resolutionList = new List<string>();

    private Resolution[] availableResolutions;
    private int currentResolutionIndex = 0;

    void Start()
    {
        mainMenu.SetActive(true);
        options.SetActive(false);

        LoadResolutions();
        LoadInitialSettings();
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene(newGameScene);
    }

    public void OpenOptions()
    {
        mainMenu.SetActive(false);
        options.SetActive(true);
    }

    public void CloseOptions()
    {
        mainMenu.SetActive(true);
        options.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void LoadResolutions()
    {
        availableResolutions = Screen.resolutions;
        resolutionList.Clear();

        int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", -1);
        currentResolutionIndex = 0;

        for (int i = 0; i < availableResolutions.Length; i++)
        {
            string option = availableResolutions[i].width + " x " + availableResolutions[i].height;
            resolutionList.Add(option);

            if (availableResolutions[i].width == Screen.width &&
                availableResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        if (savedResolutionIndex >= 0 && savedResolutionIndex < availableResolutions.Length)
        {
            currentResolutionIndex = savedResolutionIndex;
        }

        resolutionValue.text = resolutionList[currentResolutionIndex];
        ChangeResolution(currentResolutionIndex);
    }

    private void LoadInitialSettings()
    {
        // Volume
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        volumeSlider.value = savedVolume;
        VolumeController();

        // Fullscreen
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        Screen.fullScreen = isFullscreen;
        isFullscreenOn.text = isFullscreen ? "Ativado" : "Desativado";
    }

    public void ChangeResolution(int selectedIndex)
    {
        if (selectedIndex >= 0 && selectedIndex < availableResolutions.Length)
        {
            Resolution res = availableResolutions[selectedIndex];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
            currentResolutionIndex = selectedIndex;
            resolutionValue.text = resolutionList[currentResolutionIndex];

            PlayerPrefs.SetInt("ResolutionIndex", currentResolutionIndex);
            PlayerPrefs.Save();
        }
    }

    public void AddResolution()
    {
        if (currentResolutionIndex < availableResolutions.Length - 1)
        {
            currentResolutionIndex++;
            ChangeResolution(currentResolutionIndex);
        }
    }

    public void RemoveResolution()
    {
        if (currentResolutionIndex > 0)
        {
            currentResolutionIndex--;
            ChangeResolution(currentResolutionIndex);
        }
    }

    public void SetFullscreen()
    {
        Screen.fullScreen = true;
        isFullscreenOn.text = "Ativado";

        PlayerPrefs.SetInt("Fullscreen", 1);
        PlayerPrefs.Save();
    }

    public void ExitFullscreen()
    {
        Screen.fullScreen = false;
        isFullscreenOn.text = "Desativado";

        PlayerPrefs.SetInt("Fullscreen", 0);
        PlayerPrefs.Save();
    }

    public void VolumeController()
    {
        float volume = volumeSlider.value;
        AudioListener.volume = volume;

        if (volumeAudio != null)
        {
            volumeAudio.volume = volume;
        }

        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.Save();
    }
}
