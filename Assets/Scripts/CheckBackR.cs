using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBackR : MonoBehaviour
{

    CheckPoint parentCheck;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        parentCheck = GetComponentInParent<CheckPoint>();
    }
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject == player)
        {

            parentCheck.checkBackR();
        }
    }
}
