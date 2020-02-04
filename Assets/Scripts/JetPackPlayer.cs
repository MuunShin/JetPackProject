using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPackPlayer : MonoBehaviour
{
    /*-----------------------------------*/
    //             object                //
    //          JetPackPlayer            //
    //                                   //
    //   Handles the player's character  //
    /*-----------------------------------*/

    //------------------------//
    //       Variables        //
    //------------------------//


    //Inputs booleans//
    bool leftJet, rightJet, pumpAction, pumpActionDown, revEngine, revEngineDown;



    //External Components
    Rigidbody2D rb_;

    //Particles System
    ParticleSystem effRightPack, effLeftPack, effUpPack;
    bool firstUp, firstRight, firstLeft;

    //GamePlay Variables
    [Header("Mouvement")]
    [Tooltip("Upward vertical speed cap")]
    [SerializeField]
    float speedCapY;
    [SerializeField]
    [Tooltip("Horizontal speed cap")]
    float speedCapX;
    [SerializeField]
    [Tooltip("How the jectpack is affected by gravity ( 0 makes it float)")]
    float floatCap;
    [SerializeField]
    [Tooltip("The force applied horizontally")]
    float accelerationX;
    [SerializeField]
    [Tooltip("The force applied vertically when the player goes left or right")]
    float accelerationY;
    [SerializeField]
    [Tooltip("The force applied upwards when both buttons are pressed")]
    float accelerationUp;



    //------------------------//
    //       Functions        //
    //------------------------//

    // Start is called before the first frame update
    void Start()
    {
        rb_ = GetComponent<Rigidbody2D>();
        effRightPack = GameObject.Find("Eff_PackR").GetComponent<ParticleSystem>();
        effLeftPack = GameObject.Find("Eff_PackL").GetComponent<ParticleSystem>();
        effUpPack = GameObject.Find("Eff_PackUP").GetComponent<ParticleSystem>();

    }

    // Update is called once per frame
    void Update()
    {
        InputUpdate();
        CapSpeed();
    }

    // Function InputUpdate
    // !Called in Update!  Manage inputs related to the player
    void InputUpdate()
    {
        //Updates the states of all the inputs the character can do
        // !! Inputs must be modified in Project Settings -> Input Manager !!
        rightJet = Input.GetButton("RightJet");
        leftJet = Input.GetButton("LeftJet");
        pumpActionDown = Input.GetButtonDown("PumpAction");
        revEngineDown = Input.GetButtonDown("RevEngine");


        //--Jet pack controls--

        if (rightJet && leftJet) { UpPackAction(); }
        else if (rightJet) { RightPackAction(); }
        else if (leftJet) { LeftPackAction(); }
        else { StopJetParticle(); }

        //--Pump control--

        if (pumpActionDown) { PumpAction(); }

        //--Engine control--

        if (revEngineDown) { EngineRevAction(); }
    }


    // Function RightPackAction()
    // Manage what does the character when the player uses his right Jet
    void RightPackAction()
    {
        if(firstRight)
        { 
            effUpPack.Stop();
            effLeftPack.Stop();
            effRightPack.Play();
            firstLeft = firstUp = true;
            firstRight = false;
        }

        rb_.AddForce(new Vector2(accelerationX, accelerationY));
    }

    // Function LeftPackAction()
    // Manage what does the character when the player uses his left Jet
    void LeftPackAction()
    {
        if (firstLeft)
        {
            effRightPack.Stop();
            effUpPack.Stop();
            effLeftPack.Play();
            firstRight = firstUp = true;
            firstLeft = false;
        }

        rb_.AddForce(new Vector2(-accelerationX, accelerationY));
    }

    // Function UpPackAction()
    // Manage what does the character when the player uses both of his Jets
    void UpPackAction()
    {
        if (firstUp)
        {
            effRightPack.Stop();
            effLeftPack.Stop();
            effUpPack.Play();
            firstRight = firstLeft = true;
            firstUp = false;
        }

        rb_.AddForce(new Vector2(0, accelerationUp));
    }

    void StopJetParticle()
    {
        if (!firstRight || !firstLeft || !firstUp)
        {
            effRightPack.Stop();
            effLeftPack.Stop();
            effUpPack.Stop();
            firstRight = firstLeft = firstUp = true;
        }
    }


    // Function PumpAction()
    // Manage what does the character when the uses his pump input
    void PumpAction()
    {
        Debug.Log(" Pump ! ");
    }

    // Function EngineRevAction()
    // Manage what does the character when the player uses his engine input
    void EngineRevAction()
    {
        Debug.Log(" VROUMMMbububububu ! ");
    }

    void CapSpeed()
    {
        float ySpeed = rb_.velocity.y;
        float xSpeed = rb_.velocity.x;

        if (ySpeed > speedCapY)
        {
            ySpeed = speedCapY;

            Debug.Log(" Capcap Y+");
        }

        if (ySpeed < -floatCap)
        {
            ySpeed = -floatCap;

            Debug.Log(" Gravity Cap");
        }

        if (xSpeed > speedCapX)
        {
            xSpeed = speedCapX;

            Debug.Log(" Capcap x+ ");
        }
        else if (xSpeed < -speedCapX)
        {
            xSpeed = -speedCapX;

            Debug.Log(" Capcap x-");
        }

        rb_.velocity = new Vector2(xSpeed, ySpeed);

    }
}
