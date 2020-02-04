using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    /*----------------------------------------*/
    //             object                     //
    //          CameraScript                  //
    //                                        //
    //   Handles the camera and how it works  //
    /*----------------------------------------*/



    //------------------------//
    //       Variables        //
    //------------------------//

    enum CameraModes{ CenterLerp, MarioLike};
    Camera activeCamera;

    [SerializeField]
    [Tooltip("CenterLerp : Super Meat Boy Style\r\nMarioLike : BorderCaps")]
    CameraModes CameraMode;

    [SerializeField]
    [Tooltip("Target of the Camera")]
    GameObject target;

    [SerializeField]
    [Tooltip("Target of the Camera")]
    float lerpForce;

    float targetX ,targetY ,cameraX ,cameraY;

    //------------------------//
    //       Functions        //
    //------------------------//

    // Start is called before the first frame update
    void Start()
    {
        activeCamera = GetComponent<Camera>();
        float targetX = target.transform.position.x;
        float targetY = target.transform.position.y;
        float cameraX = activeCamera.transform.position.x;
        float cameraY = activeCamera.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        switch(CameraMode)
        {
            case CameraModes.CenterLerp : CenterLerpUpdate(); break;
            case CameraModes.MarioLike: MarioLikeUpdate(); break;
            default: break;
        }
    }

    void CenterLerpUpdate()
    {
        float targetX = target.transform.position.x;
        float targetY = target.transform.position.y;
        float cameraX = activeCamera.transform.position.x;
        float cameraY = activeCamera.transform.position.y;

        activeCamera.transform.position = new Vector3 (Mathf.Lerp(cameraX,targetX,lerpForce*Time.deltaTime), Mathf.Lerp(cameraY, targetY, lerpForce*Time.deltaTime), activeCamera.transform.position.z);
    }
    void MarioLikeUpdate()
    {
        float targetX = target.transform.position.x;
        float targetY = target.transform.position.y;
        float cameraX = activeCamera.transform.position.x;
        float cameraY = activeCamera.transform.position.y;


    }

}
