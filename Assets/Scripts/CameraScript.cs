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

    JetPackPlayer targetPlayer;

    [SerializeField]
    [Tooltip("Target of the Camera")]
    float lerpForce;

    [SerializeField]
    [Tooltip("Offset for when your player is rising or falling")]
    float offsetCam;

    float targetX ,targetY ,cameraX ,cameraY, maxVeloY;

    //------------------------//
    //       Functions        //
    //------------------------//

    // Start is called before the first frame update
    void Start()
    {
        activeCamera = GetComponent<Camera>();
        targetPlayer = target.GetComponent<JetPackPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        targetX = target.transform.position.x;
        targetY = target.transform.position.y;
        cameraX = activeCamera.transform.position.x;
        cameraY = activeCamera.transform.position.y;

        switch (CameraMode)
        {
            case CameraModes.CenterLerp : CenterLerpUpdate(); break;
            case CameraModes.MarioLike: MarioLikeUpdate(); break;
            default: break;
        }
    }

    void CenterLerpUpdate()
    {
        

        if (targetPlayer.Rising())
        {

            Debug.Log(" Y+");
            targetY = targetY + targetPlayer.rbVelocityY() - (targetPlayer.speedCapY / offsetCam);
        }
        if (targetPlayer.Falling())
        {

            Debug.Log(" Gravity");
            targetY = targetY + targetPlayer.rbVelocityY() + (targetPlayer.floatCap / offsetCam);
        }

        activeCamera.transform.position = new Vector3 (Mathf.Lerp(cameraX,targetX,lerpForce*Time.deltaTime), Mathf.Lerp(cameraY, targetY, lerpForce*Time.deltaTime), activeCamera.transform.position.z);
    }
    void MarioLikeUpdate()
    {

    }

}
