using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    enum UiState { Title, Main }
    enum ControlsState { Classic, Invert }
    UiState uiState = UiState.Title;
    ControlsState controlsState = ControlsState.Classic;

    [SerializeField]
    GameObject uiTitle, uiMain;

    [SerializeField]
    Button firstSelected;

    [SerializeField]
    EventSystem eS;

    private SceneManager sc;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("fromLevel") != 0)
        {
            uiState = UiState.Main;
            uiTitle.SetActive(false);
            uiMain.SetActive(true);
            firstSelected.Select();
            PlayerPrefs.SetInt("fromLevel", 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(uiState)
        {
            case UiState.Title:
                if(Input.anyKeyDown)
                {
                    uiState = UiState.Main;
                    uiTitle.SetActive(false);
                    uiMain.SetActive(true);
                    firstSelected.Select();
                }
                break;
            case UiState.Main:
                break;
            default:
                break;
        }

    }

    public void LevelSelector(int level)
    {
        switch(level)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                SceneManager.LoadScene("TestScene");
                break;
            default:
                
                break;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("fromLevel", 0);
    }
}
