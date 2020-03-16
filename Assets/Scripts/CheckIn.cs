using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIn : MonoBehaviour
{
    GameObject player;
    CheckPoint parentCheck;
    BoxCollider2D col;
    bool noCollide;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        parentCheck = GetComponentInParent<CheckPoint>();
        col = GetComponent<BoxCollider2D>();
    }

    public void NoCheckOnce()
    {
        noCollide = true;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject == player)
        {

            if (noCollide)
                noCollide = false;
            else
                parentCheck.checkIn();
        }
    }
}

