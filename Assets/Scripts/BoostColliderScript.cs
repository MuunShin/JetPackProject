using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostColliderScript : MonoBehaviour
{
    bool alreadyNear = false;
    List<GameObject> currentCollisions = new List<GameObject>();
    private void OnTriggerEnter2D(Collider2D collision)
    {

        

        if (collision.gameObject.CompareTag("Wall"))
        {
            if (alreadyNear == false)
            {
                alreadyNear = true;
                GetComponentInParent<JetPackPlayer>().BoostChargeStart();
            }

            currentCollisions.Add(collision.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Wall"))
        {
            currentCollisions.Remove(collision.gameObject);

            if (currentCollisions.Count == 0)
            {
                GetComponentInParent<JetPackPlayer>().BoostChargeStop();
                alreadyNear = false;
            }
        }
    }
}
