using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public enum FinishType { Tour, Piste}

    [SerializeField]
    public FinishType raceMode;

    [Header("Tour")]
    [Tooltip("Nombre de tours")]
    [SerializeField]
    public float lapAmount;

    [Header("Liste CheckPoint")]
    [Tooltip("Checkpoints necessaire pour valider un tour")]
    [SerializeField]
    public CheckPoint[] checkPointArray;

    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void Finish()
    {

    }

    public void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject == player)
        {
            switch (raceMode)
            {
                case FinishType.Piste:
                    Finish();
                    break;
                case FinishType.Tour: 

                    foreach (CheckPoint ch in checkPointArray)
                    {
                        if(ch.State != CheckPoint.CheckPointState.Validated)
                        {

                        }
                    }

                    break;
                default: break;
            }

        }
    }
}
