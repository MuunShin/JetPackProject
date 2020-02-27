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
    float lerpForceY;

    [SerializeField]
    [Tooltip("Target of the Camera")]
    float lerpForceX;

    [SerializeField]
    [Tooltip("Target of the Camera")]
    float offSetX;

    [SerializeField]
    [Tooltip("Target of the Camera")]
    float deadZone;

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
            targetY = targetY + targetPlayer.RbVelocityY() - (targetPlayer.speedCapY / offsetCam);
        }
        if (targetPlayer.Falling())
        {

            Debug.Log(" Gravity");
            targetY = targetY + targetPlayer.RbVelocityY() + (targetPlayer.floatCap / offsetCam);
        }


        targetX = targetX + (targetPlayer.RbVelocityX() * 0.5f);

        activeCamera.transform.position = new Vector3 (Mathf.Lerp(cameraX,targetX,lerpForceX*Time.deltaTime), Mathf.Lerp(cameraY, targetY, lerpForceY*Time.deltaTime), activeCamera.transform.position.z);
    }
    void MarioLikeUpdate()
    {

    }

}
