using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public Vector2 vector;
    public float speed;
    public bool left;

    void Update()
    {
        if (left)
            transform.Translate(vector * speed);
        else
            transform.Translate(-vector * speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
            left = !left;
    }
}
