using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    public int bumperForce = 800;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject == player)
        {
            player.GetComponent<Rigidbody2D>().AddExplosionForce(bumperForce, transform.position, 1);
        }
    }
}
