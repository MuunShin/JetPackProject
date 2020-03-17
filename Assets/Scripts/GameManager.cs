using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
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
    Animator CanvasAnim;

    float chronoSec, chronoMin, chronoCentiSec, lapSec, lapMin, lapCentiSec, timer, lapTime;
    string chronoMinS, chronoSecS, chronoCentiS, lapMinS, lapSecS, lapCentiS, lapDisplay;
    bool finished = false;
    public bool startup = true;
    bool realStartup;
    int countdown = 0;
    ArrayList lapTimerAL;

    // Start is called before the first frame update
    void Start()
    {
        
        lapTimerAL = new ArrayList();
        if (finishLine.raceMode == FinishLine.FinishType.Tour)
        {
            lapTimerAL.Add(finishLine.actualLap + " : " + GetTimeLap());
            lapCount.text = finishLine.actualLap + "/" + finishLine.lapAmount;

        }
    }


    // Update is called once per frame
    void Update()
    {
        if (startup)
        {
            StartupUpdate();
        }
        if (!finished && !startup)
            ChronoUpdate();

        timerRace.text = GetTime();
        if (finishLine.raceMode == FinishLine.FinishType.Tour)
        {
            lapDisplay = "";
            foreach(string lap in lapTimerAL)
            {               
                lapDisplay += lap + "\n";
            }
            timerLaps.text = lapDisplay;
        }
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
        Debug.Log("Finish");
        Debug.Log("afficher fin de jeu");

    }

    string GetTimeLap()
    {
        return lapMinS + ":" + lapSecS + ":" + lapCentiS;
    }

    string GetTime()
    {
        return chronoMinS + ":" + chronoSecS + ":" + chronoCentiS;
    }

    public void LapCount()
    {
        lapTime = 0;
        lapTimerAL.Add(finishLine.actualLap + " : " + GetTimeLap());
        lapCount.text = finishLine.actualLap + "/" + finishLine.lapAmount;
        
    }

    public void LapCountFinish()
    {
        finished = true;

    }
}
