using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public enum CheckPointState { Unchecked, Reversed, Validated};

    CheckIn inBox; 
    CheckBack backBox;
    CheckBackR backBoxR;

    public CheckPointState State;

// Start is called before the first frame update
    void Start()
    {
        inBox = transform.GetChild(0).GetComponent<CheckIn>();
        backBox = transform.GetChild(1).GetComponent<CheckBack>();
        backBoxR = transform.GetChild(2).GetComponent<CheckBackR>();
    }

    public void checkIn()
    {
        switch (State)
        {
            case CheckPointState.Reversed:
                break;

            case CheckPointState.Unchecked:
                Debug.Log("Valid");
                State = CheckPointState.Validated;
                break;

            default: break;
        }
    }

    public void checkBackR()
    {
        Debug.Log("Back on track");
        State = CheckPointState.Unchecked;
        backBox.gameObject.SetActive(true);
        backBoxR.gameObject.SetActive(false);
    }

    public void checkBack()
    {
        switch (State)
        {
            case CheckPointState.Validated:
                break;

            case CheckPointState.Unchecked:
                Debug.Log("Wrong Way");
                State = CheckPointState.Reversed;
                backBox.gameObject.SetActive(false);
                backBoxR.gameObject.SetActive(true);
                break;

            default: break;
        }
    }
}
