using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LookAtCamera : MonoBehaviour
{

    Camera mainCamera;
    public bool scaleDependingOnDistance = false;
    public float sizeMin;
    public float sizeMax;

    public float value;

    public Image display;
    public GameObject None;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(mainCamera.transform.position);

        if (scaleDependingOnDistance)
        {
            float dist = Vector3.Distance(transform.position, mainCamera.transform.position);
            dist = Mathf.Clamp(dist, 5, 20);

            float size;
            size = Map(dist, 5, 20, 0, 1);
            size = Mathf.Lerp(sizeMin, sizeMax, size);

            transform.localScale = new Vector3(size, size, size);
        }
        display.fillAmount = value;

        if (value == -1)
        {
            None.SetActive(true);
        } else
        {
            None.SetActive(false);
        }
    }

    public float Map(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
