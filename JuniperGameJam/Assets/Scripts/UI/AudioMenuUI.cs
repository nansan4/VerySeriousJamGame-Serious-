using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMenuUI : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider ambienceSlider;
    [SerializeField] private Slider playerSFXSlider;
    [SerializeField] private Slider gameSFXSlider;

    [Header("Music/Ambience")]
    [SerializeField] private AudioMixer musicMixer;

    [Header("Player/Game SFX")]
    [SerializeField] private AudioMixer sfxMixer;

    [Header("UI Vars")]
    [SerializeField] private Button backButton;
    [SerializeField] private OptionsUI optionsUI;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GraphicRaycaster graphicRaycaster;

    private const string MUSIC_KEY = "MusicVolume";
    private const string AMBIENCE_KEY = "AmbienceVolume";
    private const string PLAYER_SFX_KEY = "PlayerSFXVolume";
    private const string GAME_SFX_KEY = "GameSFXVolume";

    private void Start()
    {
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        ambienceSlider.onValueChanged.AddListener(SetAmbienceVolume);
        playerSFXSlider.onValueChanged.AddListener(SetPlayerSFXVolume);
        gameSFXSlider.onValueChanged.AddListener(SetGameSFXVolume);

        backButton.onClick.AddListener(Hide);

        LoadSettings();
    }

    #region Save / Load

    private void LoadSettings()
    {
        musicSlider.value =
            PlayerPrefs.GetFloat(MUSIC_KEY, 0.3229164f);

        ambienceSlider.value =
            PlayerPrefs.GetFloat(AMBIENCE_KEY, 1f);

        playerSFXSlider.value =
            PlayerPrefs.GetFloat(PLAYER_SFX_KEY, 0.78125f);

        gameSFXSlider.value =
            PlayerPrefs.GetFloat(GAME_SFX_KEY, 1f);
    }

    #endregion

    #region Slider Funcs

    private void SetMusicVolume(float value)
    {
        float db = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;

        musicMixer.SetFloat("MusicVolume", db);
        PlayerPrefs.SetFloat(MUSIC_KEY, value);
    }

    private void SetAmbienceVolume(float value)
    {
        float db = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;

        musicMixer.SetFloat("AmbienceVolume", db);
        PlayerPrefs.SetFloat(AMBIENCE_KEY, value);
    }

    private void SetPlayerSFXVolume(float value)
    {
        float db = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;

        sfxMixer.SetFloat("PlayerSFXVolume", db);
        sfxMixer.SetFloat("CollisionSFXVolume", db);

        PlayerPrefs.SetFloat(PLAYER_SFX_KEY, value);
    }

    private void SetGameSFXVolume(float value)
    {
        float db = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;

        sfxMixer.SetFloat("GameSFXVolume", db);

        PlayerPrefs.SetFloat(GAME_SFX_KEY, value);
    }

    #endregion

    #region UI Funcs

    public void Show()
    {
        canvas.enabled = true;
        graphicRaycaster.enabled = true;
    }

    public void Hide()
    {
        PlayerPrefs.Save();

        canvas.enabled = false;
        graphicRaycaster.enabled = false;

        optionsUI.Show();
    }

    #endregion
}