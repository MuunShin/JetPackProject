using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBack : MonoBehaviour
{
    GameObject player;
    CheckPoint parentCheck;
    BoxCollider2D col;
    bool noCollide;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        parentCheck = GetComponentInParent<CheckPoint>();
        col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NoCollide()
    {
        col.enabled = false;
    }

    public void NoCheckOnce()
    {
        noCollide = true;
    }

    public void Reset()
    {
        noCollide = false;
        col.enabled = true;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.gameObject == player)
        {

            if (noCollide)
                noCollide = false;
            else
                parentCheck.checkBack();
        }
    }
}
