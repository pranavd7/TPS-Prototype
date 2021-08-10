using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;

    [SerializeField] GameObject PausedUI;
    [SerializeField] GameObject PlayStateUI;
    [SerializeField] GameObject GameOverUI;
    [SerializeField] GameObject LevelCompletedUI;
    [SerializeField] Button backButton;
    [SerializeField] TMPro.TMP_Text waveText;
    [SerializeField] Animator transition;
    [SerializeField] Button resumeButton;
    [SerializeField] Slider volumeSlider;
    [SerializeField] CinemachineFreeLook thirdPersonCam;
    [SerializeField] CinemachineVirtualCamera firstPersonCam;
    public enum gameStates { Playing, Paused, Death, GameOver, LevelCompleted };
    public gameStates gameState;
    public bool isRobot = false;
    public bool isCompleted = false;
    public bool isAlive = true;
    public bool wave1finished = false;
    public bool paused;

    bool setting = false;
    float transitionTime = 0.4f;

    private void Awake()
    {
        if (gm == null)
            gm = gameObject.GetComponent<GameManager>();
        paused = false;
        PausedUI.SetActive(false);
        GameOverUI.SetActive(false);
        LevelCompletedUI.SetActive(false);

        isCompleted = PlayerPrefs.GetInt("completed", 0) == 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            ScreenCapture.CaptureScreenshot("game.png");
            //Debug.Log("ss done");
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            PlayerPrefs.DeleteAll();
        }

        //Debug.Log(gameState);
        switch (gameState)
        {
            case gameStates.Playing:
                if (isCompleted)
                {
                    waveText.enabled = true;
                    waveText.text = "Wave: " + EnemySpawner.waveNumber;
                }
                else
                {
                    waveText.enabled = false;
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGame();
                }
                if (!isAlive)
                    gameState = gameStates.Death;
                if (wave1finished)
                {
                    gameState = gameStates.LevelCompleted;
                }
                break;
            case gameStates.Paused:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (setting)
                    {
                        backButton.onClick.Invoke();
                    }
                    else
                        Resume();
                }
                break;
            case gameStates.LevelCompleted:
                //isCompleted = true;
                EndGame();
                break;
            case gameStates.Death:
                thirdPersonCam.m_Follow = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0);
                thirdPersonCam.m_LookAt = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0);
                thirdPersonCam.m_XAxis.m_InputAxisValue = 0;
                thirdPersonCam.m_YAxis.m_InputAxisValue = 0;
                thirdPersonCam.m_XAxis.m_InputAxisName = "";
                thirdPersonCam.m_YAxis.m_InputAxisName = "";
                firstPersonCam.enabled = false;
                //firstPersonCam.GetComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisName = "";
                //firstPersonCam.GetComponent<CinemachinePOV>().m_VerticalAxis.m_InputAxisName = "";
                EndGame();
                break;
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        PlayStateUI.SetActive(false);
        PausedUI.SetActive(true);
        resumeButton.Select();
        gameState = gameStates.Paused;
        paused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Resume()
    {
        Time.timeScale = 1;
        PausedUI.SetActive(false);
        GameOverUI.SetActive(false);
        LevelCompletedUI.SetActive(false);
        PlayStateUI.SetActive(true);
        gameState = gameStates.Playing;
        paused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void EndGame()
    {
        PlayStateUI.SetActive(false);
        if (wave1finished && isAlive)
        {
            LevelCompletedUI.SetActive(true);
            //Time.timeScale = 0;
        }
        else
            GameOverUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartWaves()
    {
        isCompleted = true;
        wave1finished = false;
        Resume();
    }

    public void ToggleSettings()
    {
        setting = !setting;
        if (setting)
            volumeSlider.Select();
        else
            resumeButton.Select();
    }
    public void LoadScene(string levelToLoad)
    {
        StartCoroutine(LoadLevelTransition(levelToLoad));
    }

    IEnumerator LoadLevelTransition(string levelToLoad)
    {
        transition.SetTrigger("fade");
        Time.timeScale = 1;
        yield return new WaitForSeconds(transitionTime);

        // load the specified level
        SceneManager.LoadScene(levelToLoad);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        if (isCompleted)
            PlayerPrefs.SetInt("completed", 1);
        else
            PlayerPrefs.SetInt("completed", 0);

        PlayerPrefs.Save();
        //PlayerPrefs.DeleteAll();
    }
}
