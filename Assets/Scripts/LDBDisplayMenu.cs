using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LDBDisplayMenu : MonoBehaviour
{
    [SerializeField]
    LeaderBoardManager _LDBManager;

    GameObject first ,second ,third ,fourth ,fifth ,sixth ;

    ArrayList _ArraylistLeaderBoard;
    void Start()
    {
        first  = this.transform.Find("1st").gameObject;
        second = this.transform.Find("2nd").gameObject;
        third  = this.transform.Find("3rd").gameObject;
        fourth = this.transform.Find("4th").gameObject;
        fifth  = this.transform.Find("5th").gameObject;
        sixth  = this.transform.Find("6th").gameObject;
    }

    public void UpdateLDB(int level)
    {
        _ArraylistLeaderBoard = _LDBManager.ReadScoreTxt(level);

        UpdateLine(first, 1);
        UpdateLine(second, 2);
        UpdateLine(third, 3);
        UpdateLine(fourth, 4);
        UpdateLine(fifth, 5);
        UpdateLine(sixth, 6);
    }

    private void UpdateLine(GameObject line, int placement)
    {
        ArrayList list = (ArrayList)_ArraylistLeaderBoard[placement - 1];
        line.transform.Find("NAME").GetComponent<Text>().text = (string)list[0];
        float min     = float.Parse((string)list[1]);
        float sec     = float.Parse((string)list[2]);
        float cent    = float.Parse((string)list[3]);
        float lapMin  = float.Parse((string)list[4]);
        float lapSec  = float.Parse((string)list[5]);
        float lapCent = float.Parse((string)list[6]);

        string time = min + ":" + sec.ToString("00") + ":" + cent.ToString("00");
        string timeLap = lapMin + ":" + lapSec.ToString("00") + ":" + lapCent.ToString("00");

        line.transform.Find("BestLap").GetComponent<Text>().text = timeLap;
        line.transform.Find("Time").GetComponent<Text>().text = time;

        switch(placement)
        {
            case 1: 
                line.transform.Find("Pos").GetComponent<Text>().text = "1st"; 
                break;
            case 2:
                line.transform.Find("Pos").GetComponent<Text>().text = "2nd";
                break;
            case 3:
                line.transform.Find("Pos").GetComponent<Text>().text = "3rd";
                break;
            case 4:
                line.transform.Find("Pos").GetComponent<Text>().text = "4th";
                break;
            case 5:
                line.transform.Find("Pos").GetComponent<Text>().text = "5th";
                break;
            case 6:
                line.transform.Find("Pos").GetComponent<Text>().text = "6th";
                break;

            default:
                line.transform.Find("Pos").GetComponent<Text>().text = "NaN";
                break;
                
        }
        
    }

    public string fetchFirstTime(int level)
    {
        _ArraylistLeaderBoard = _LDBManager.ReadScoreTxt(level);
        ArrayList list = (ArrayList)_ArraylistLeaderBoard[0];
        float min = float.Parse((string)list[1]);
        float sec = float.Parse((string)list[2]);
        float cent = float.Parse((string)list[3]);

        string timeReturn = min + ":" + sec.ToString("00") + ":" + cent.ToString("00");

        return timeReturn;
    }
    public string fetchFirstName (int level)
    {
        _ArraylistLeaderBoard = _LDBManager.ReadScoreTxt(level);
        ArrayList list = (ArrayList)_ArraylistLeaderBoard[0];
        return (string)list[0];
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
