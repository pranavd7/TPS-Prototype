using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] TMPro.TMP_Dropdown resolutionDropdown;
    [SerializeField] Button backButtonSettings;
    [SerializeField] Button backButtonControls;
    [SerializeField] Animator transition;

    bool setting = false;
    bool controls = false;
    float transitionTime = 0.4f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (setting)
            {
                backButtonSettings.onClick.Invoke();
            }
            if (controls)
            {
                backButtonControls.onClick.Invoke();
            }
        }

        if (Input.GetKeyDown(KeyCode.F12))
        {
            ScreenCapture.CaptureScreenshot("Menu.png");
            //Debug.Log("ss done");
        }

    }
    public void LoadLevel(string levelToLoad)
    {
        StartCoroutine(LoadLevelTransition(levelToLoad));
    }

    IEnumerator LoadLevelTransition(string levelToLoad)
    {
        transition.SetTrigger("fade");
        yield return new WaitForSeconds(transitionTime);

        // load the specified level
        SceneManager.LoadScene(levelToLoad);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleSettings()
    {
        setting = !setting;
        if (setting)
            resolutionDropdown.Select();
        else
            startButton.Select();
    }
    public void ToggleControls()
    {
        controls = !controls;
        if (controls)
            backButtonControls.Select();
        else
            startButton.Select();
    }
}
