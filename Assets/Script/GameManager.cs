using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float thoiGianChoPhepVeDich = 2000f;
    public bool winGame = false;
    public bool ketThucGame = false;
    private static GameManager instance;
    public GameObject gameOverObject;
    public GameObject winGameObject;
    public GameObject timeGameObject;
    [SerializeField]
    private float thoiGianHoiQuaCheckPoint = 120f;

    [Header("Âm thanh")]
    [SerializeField] private AudioSource engineSound; // Âm thanh động cơ
    [SerializeField] private AudioSource brakeSound;  // Âm thanh phanh
    [SerializeField] private AudioSource backgroundMusic; // Âm thanh nhạc nền

    private bool isSoundEnabled = true;
    private bool isBackgroundMusicEnabled = true;
    public bool isGamePaused = false;

    [Header("UI Controls")]
    [SerializeField] private Button optionButton; // Nút Option
    [SerializeField] private GameObject pauseMenu; // Panel Pause Menu
    [SerializeField] private Toggle backgroundToggle; // Toggle cho Background Song
    [SerializeField] private Toggle soundToggle; // Toggle cho Sound
    [SerializeField] private Button resumeButton; // Nút Tiếp tục chơi
    [SerializeField] private Button restartButton; // Nút Chơi lại
    [SerializeField] private Button exitButton; // Nút Exit

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameManager>();
                if (instance == null)
                {
                    GameObject gameManagerGameObject = new GameObject("GameManager");
                    instance = gameManagerGameObject.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    void Start()
    {
        // Kiểm tra và thiết lập âm thanh
        if (engineSound == null || brakeSound == null || backgroundMusic == null)
        {
            Debug.LogWarning("⚠️ Vui lòng gán AudioSource cho engineSound, brakeSound và backgroundMusic trong Inspector.");
        }
        else
        {
            engineSound.loop = true;
            engineSound.volume = 0.1f;
            engineSound.Play();

            brakeSound.loop = false;
            brakeSound.volume = 0f;

            backgroundMusic.loop = true;
            backgroundMusic.volume = 0.3f;
            backgroundMusic.Play();
        }

        // Gán sự kiện cho các nút và toggle
        if (optionButton != null) optionButton.onClick.AddListener(ShowPauseMenu);
        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (exitButton != null) exitButton.onClick.AddListener(ExitGame);
        if (backgroundToggle != null) backgroundToggle.onValueChanged.AddListener(ToggleBackgroundMusic);
        if (soundToggle != null) soundToggle.onValueChanged.AddListener(ToggleSound);

        // Đặt trạng thái ban đầu cho Toggle
        if (backgroundToggle != null) backgroundToggle.isOn = isBackgroundMusicEnabled;
        if (soundToggle != null) soundToggle.isOn = isSoundEnabled;

        // Ẩn Pause Menu ban đầu
        if (pauseMenu != null) pauseMenu.SetActive(false);
    }

    void Update()
    {
        if (!ketThucGame && !isGamePaused)
        {
            thoiGianChoPhepVeDich -= Time.deltaTime;
            //Debug.Log(thoiGianChoPhepVeDich);
            if (thoiGianChoPhepVeDich <= 0)
            {
                timeGameObject.SetActive(false);
                gameOverObject.SetActive(true);
                KetThucGame();
            }
        }
        if (winGame && !isGamePaused)
        {
            timeGameObject.SetActive(false);
            winGameObject.SetActive(true);
        }
    }

    public void KetThucGame()
    {
        ketThucGame = true;
    }

    public void QuaCheckPoint()
    {
        if (!ketThucGame)
        {
            thoiGianChoPhepVeDich = thoiGianHoiQuaCheckPoint;
        }
    }

    public void QuaWinPoint()
    {
        if (!ketThucGame)
        {
            winGame = true;
        }
    }

    public void PlayerCaught()
    {
        timeGameObject.SetActive(false);
        gameOverObject.SetActive(true);
        KetThucGame();
    }

    private void ShowPauseMenu()
    {
        if (pauseMenu != null)
        {
            isGamePaused = true;
            Time.timeScale = 0f; // Tạm dừng game
            if (backgroundMusic != null) backgroundMusic.Pause(); // Tạm dừng nhạc
            pauseMenu.SetActive(true);
        }
    }

    private void ResumeGame()
    {
        if (pauseMenu != null)
        {
            isGamePaused = false;
            Time.timeScale = 1f; // Tiếp tục game
            if (backgroundMusic != null && isBackgroundMusicEnabled) backgroundMusic.UnPause(); // Tiếp tục nhạc nếu bật
            pauseMenu.SetActive(false);
        }
    }

    private void RestartGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Tải lại scene hiện tại
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Dừng trong Editor
#else
        Application.Quit(); // Thoát game khi build
#endif
    }

    private void ToggleBackgroundMusic(bool value)
    {
        isBackgroundMusicEnabled = value;
        if (backgroundMusic != null)
        {
            backgroundMusic.mute = !isBackgroundMusicEnabled;
            if (isGamePaused && isBackgroundMusicEnabled) backgroundMusic.UnPause();
            else if (!isBackgroundMusicEnabled) backgroundMusic.Pause();
        }
    }

    private void ToggleSound(bool value)
    {
        isSoundEnabled = value;
        if (engineSound != null) engineSound.mute = !isSoundEnabled;
        if (brakeSound != null) brakeSound.mute = !isSoundEnabled;
    }
}