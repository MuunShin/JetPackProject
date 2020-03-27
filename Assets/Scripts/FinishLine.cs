using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public enum FinishType { Tour, Piste}

    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    public FinishType raceMode;

    [Header("Tour")]
    [Tooltip("Nombre de tours")]
    [SerializeField]
    public float lapAmount;

    public float actualLap = 1;

    [Header("Liste CheckPoint")]
    [Tooltip("Checkpoints necessaire pour valider un tour")]
    [SerializeField]
    public CheckPoint[] checkPointArray;

    GameObject player;
    AudioSource sound;

    [SerializeField]
    AudioClip lap, finish;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        sound = transform.Find("AudioLap").GetComponent<AudioSource>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void Finish()
    {
        if (raceMode == FinishType.Tour)
            gameManager.LapCountFinish();

        gameManager.FinishGame();
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("zbub");
        if (!gameManager.startup)
        {
            if (collider.gameObject == player)
            {
                switch (raceMode)
                {
                    case FinishType.Piste:

                        Finish();
                        break;

                    case FinishType.Tour:

                        bool validate = true;

                        foreach (CheckPoint ch in checkPointArray)
                        {
                            Debug.Log("Pas tout les check");
                            if (ch.State != CheckPoint.CheckPointState.Validated)
                            {
                                validate = false;
                            }
                        }

                        if (validate)
                        {

                            if (lapAmount <= actualLap)
                            {
                                sound.clip = finish;
                                sound.Play();
                                Finish();
                            }
                            else
                            {

                                actualLap++;

                                sound.clip = lap;
                                sound.Play();

                                foreach (CheckPoint ch in checkPointArray)
                                {
                                    ch.State = CheckPoint.CheckPointState.Unchecked;
                                }

                                gameManager.LapCount();
                            }

                        }
                        break;

                    default:

                        break;
                }

            }
        }
    }
}
