using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SoundUi : MonoBehaviour
{
    bool selected = false;
    Button thisB;
    AudioSource son;
    [SerializeField]
    AudioClip select, pressed;

    // Start is called before the first frame update
    void Start()
    {
        son = GetComponent<AudioSource>();
        thisB = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject && !selected)
        {
            son.clip = select;
            son.Play();
            thisB.onClick.AddListener(TaskOnClick);
            selected = true;
        }
        else if (EventSystem.current.currentSelectedGameObject != gameObject && selected)
        {
            selected = false;
        }
    }
    void TaskOnClick()
    {
        son.clip = pressed;
        son.Play();
    }
    
}
