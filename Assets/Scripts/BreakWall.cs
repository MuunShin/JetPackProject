using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakWall : MonoBehaviour
{

    public void Destruction()
    {
        Destroy(transform.gameObject);
    }
}

