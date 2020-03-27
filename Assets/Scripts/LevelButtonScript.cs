using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelButtonScript : MonoBehaviour
{
    [SerializeField]
    int level;
    [SerializeField]
    LDBDisplayMenu _LDBDisplayMenu;

    Text nom, temps;

    private void Start()
    {
        nom = transform.Find("Nom").GetComponent<Text>();
        temps = transform.Find("Time").GetComponent<Text>();

        nom.text = _LDBDisplayMenu.fetchFirstName(level);
        temps.text = _LDBDisplayMenu.fetchFirstTime(level);
    }

    bool selected;
    public void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject && !selected)
        {
            Debug.Log("HEY");
            _LDBDisplayMenu.UpdateLDB(level);
            selected = true;
        }
        else if(EventSystem.current.currentSelectedGameObject != gameObject && selected)
        {
            selected = false;
        }
    }
   
}