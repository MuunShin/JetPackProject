using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Tooltip("LevelNb")]
    public int actualLevel;

    [Header("Gameplay Elements")]
    [Tooltip("Player")]
    [SerializeField]
    JetPackPlayer Player;

    [Tooltip("Finish Line")]
    [SerializeField]
    FinishLine finishLine;

    [Header("Timer & UI")]
    [Tooltip("Text where tiemer goes")]
    [SerializeField]
    Text timerRace;

    [Tooltip("Text where tiemer goes")]
    [SerializeField]
    Text timerLaps;

    [Tooltip("Text where tiemer goes")]
    [SerializeField]
    Text lapCount;

    [Tooltip("Text where tiemer goes")]
    [SerializeField]
    Text pos, timeFinishB, lapFinishB, timeFinishNoB, lapFinishNoB, name;

    [Tooltip("Text where tiemer goes")]
    [SerializeField]
    InputField inputName;

    [Tooltip("Text where tiemer goes")]
    [SerializeField]
    LeaderBoardManager _LBManager;

    [Tooltip("Text where tiemer goes")]
    [SerializeField]
    Animator CanvasAnim;

    [Tooltip("Text where tiemer goes")]
    [SerializeField]
    AudioSource CountDown;

    [SerializeField]
    Button okSelected,homeSelected, pauseSeclected;

    [SerializeField]
    GameObject pauseMenu;

    [SerializeField]
    EventSystem eS;


    [Header("Sound")]
    [SerializeField]
    AudioSource sound;
    [SerializeField]
    AudioSource uiSound;

    [SerializeField]
    AudioClip mapTheme, finish, selctUi, pressedUi;

    float chronoSec, chronoMin, chronoCentiSec, lapSec, lapMin, lapCentiSec, timer, lapTime, bestLapTime, bLapMin, bLapSec, bLapCenti;
    string chronoMinS, chronoSecS, chronoCentiS, lapMinS, lapSecS, lapCentiS, bLapTime, lapDisplay;
    bool finished = false;
    public bool startup = true;
    bool realStartup,paused;
    int countdown = 0;
    ArrayList lapTimerAL;

    // Start is called before the first frame update
    void Start()
    {


        timerRace.text = "00:00:00";
        timerLaps.text = "1 : 00:00:00";
        bestLapTime = 99999999;
        lapTimerAL = new ArrayList();
        if (finishLine.raceMode == FinishLine.FinishType.Tour)
        {
            lapTimerAL.Add(finishLine.actualLap + " : " + GetTimeLap());
            lapCount.text = finishLine.actualLap + "/" + finishLine.lapAmount;

        }
        _LBManager.ReadScoreTxt(6);
    }



    // Update is called once per frame
    void Update()
    {
        if (startup)
        {
            StartupUpdate();
        }
        if (!finished && !startup)
        {
            if (Input.GetButtonDown("pauseButton"))
                paused = startPause();

            ChronoUpdate();
            timerRace.text = GetTime();
        }
  
        if (finishLine.raceMode == FinishLine.FinishType.Tour && !startup)
        {
            lapDisplay = "";
            foreach(string lap in lapTimerAL)
            {               
                lapDisplay += lap + "\n";
            }
            timerLaps.text = lapDisplay;
        }
    }

    public bool startPause()
    {
        if (Time.timeScale == 0f)
        {
            Player.Paused(false);
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
            return (false);
        }
        else
        {
            pauseSeclected.Select();
            pauseSeclected.OnSelect(null);

            Player.Paused(true);
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
            return (true);
        }
    }

    public void togglePause()
    {
        paused = startPause();
    }

    void StartupUpdate()
    {
        timer += Time.deltaTime;
        chronoSec = Mathf.RoundToInt(timer % 60);

        if (!realStartup)
        {

            if (chronoSec >= 3)
            {
                timer = 0;
                realStartup = true;
                Debug.Log("3");
                CanvasAnim.SetTrigger("CDTrigger");
                CountDown.Play();
            }
        }
        else
        {
            if (timer >= 1)
            {
                timer = 0;

                switch (countdown)
                {
                    case 0:
                        Debug.Log("2");
                        break;
                    case 1:
                        Debug.Log("1");
                        break;
                    case 2:
                        Debug.Log("GO");
                        startup = false;
                        Player.enabled = true;

                        sound.clip = mapTheme;
                        sound.loop = true;
                        sound.Play();

                        break;
                    default:

                        break;
                }
                countdown++;
            }
        }
    }

    void ChronoUpdate()
    {
        timer += Time.deltaTime;
        lapTime += Time.deltaTime;

        chronoMin = Mathf.Floor(timer / 60);
        chronoSec = Mathf.RoundToInt(timer % 60);
        chronoCentiSec = Mathf.RoundToInt((timer * 100)%99);

        lapMin = Mathf.Floor(lapTime / 60);
        lapSec = Mathf.RoundToInt(lapTime % 60);
        lapCentiSec = Mathf.RoundToInt((lapTime * 100) % 99);



        if (chronoMin < 10)
            chronoMinS = Mathf.Floor(chronoMin).ToString("00");
        else
            chronoMinS = Mathf.Floor(chronoMin).ToString();

        if (chronoSec <= 10)
            chronoSecS = chronoSec.ToString("00");
        else
            chronoSecS = chronoSec.ToString();

        if (chronoCentiSec <= 10)
            chronoCentiS = chronoCentiSec.ToString("00");
        else
            chronoCentiS = chronoCentiSec.ToString();

        //Lap chrono



        if (finishLine.raceMode == FinishLine.FinishType.Tour)
        {
            if (lapMin < 10)
                lapMinS = Mathf.Floor(lapMin).ToString("00");
            else
                lapMinS = Mathf.Floor(lapMin).ToString();

            if (lapSec <= 10)
                lapSecS = lapSec.ToString("00");
            else
                lapSecS = lapSec.ToString();

            if (lapCentiSec <= 10)
                lapCentiS = lapCentiSec.ToString("00");
            else
                lapCentiS = lapCentiSec.ToString();

            lapTimerAL[(int)(finishLine.actualLap - 1)] = finishLine.actualLap + " : " + GetTimeLap();
        }
    }

    public void FinishGame()
    {
        Player.enabled = false;
        CanvasAnim.SetTrigger("EndTrigger");

        sound.clip = finish;
        sound.loop = true;
        sound.Play();

        switch (_LBManager.Placed(chronoMin, chronoSec, chronoCentiSec, actualLevel))
        {
            case 1:
                pos.text = "1st";
                BestScoreUiUpdate();
                break;
            case 2:
                pos.text = "2nd";
                BestScoreUiUpdate();
                break;
            case 3:
                pos.text = "3rd";
                BestScoreUiUpdate();
                break;
            case 4:
                pos.text = "4th";
                BestScoreUiUpdate();
                break;
            case 5:
                pos.text = "5th";
                BestScoreUiUpdate();
                break;
            case 6:
                pos.text = "6th";
                BestScoreUiUpdate();
                break;
            default:
                CanvasAnim.SetTrigger("NoBestScore");
                lapFinishNoB.text = GetBestLap();
                timeFinishNoB.text = GetTime();
                homeSelected.Select();
                homeSelected.OnSelect(null);
                break;
        }

    }

    private void BestScoreUiUpdate()
    {
        okSelected.Select();
        okSelected.OnSelect(null);
        lapFinishB.text = GetBestLap();
        timeFinishB.text = GetTime();
        CanvasAnim.SetTrigger("BestScore");
    }

    public void ValidateInput()
    {
        _LBManager.AddScore(inputName.text,chronoMin,chronoSec,chronoCentiSec,bLapMin,bLapSec,bLapCenti,actualLevel);
        name.text = inputName.text;
        CanvasAnim.SetTrigger("BestScoreNext");

        homeSelected.Select();
        homeSelected.OnSelect(null);

    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void HomeScene()
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("fromLevel", actualLevel);
        SceneManager.LoadScene("MainMenu");
    }

    string GetTimeLap()
    {
        return lapMinS + ":" + lapSecS + ":" + lapCentiS;
    }

    string GetBestLap()
    {
        return bLapTime;
    }

    string GetTime()
    {
        return chronoMinS + ":" + chronoSecS + ":" + chronoCentiS;
    }

    public void LapCount()
    {
        if(bestLapTime > lapTime)
        {
            bestLapTime = lapTime;
            bLapMin = lapMin;
            bLapSec = lapSec;
            bLapCenti = lapCentiSec;
            bLapTime = GetTimeLap();
            Debug.Log(GetTimeLap());
            Debug.Log(GetBestLap());
        }
        lapTime = 0;
        lapTimerAL.Add(finishLine.actualLap + " : " + GetTimeLap());
        lapCount.text = finishLine.actualLap + "/" + finishLine.lapAmount;
        
    }

    public void LapCountFinish()
    {
        finished = true;

    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("fromLevel", 0);
    }
}
