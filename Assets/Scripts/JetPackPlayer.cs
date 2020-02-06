using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    ParticleSystem effRightPack, effLeftPack, effUpPack, effReving, effRev, effEngineOn;
    bool firstUp, firstRight, firstLeft;


    //Mouvement Variables
    [Header("Mouvement")]
    [Tooltip("Upward vertical speed cap")]
    [SerializeField]
    public float speedCapY;
    [SerializeField]
    [Tooltip("Horizontal speed cap")]
    public float speedCapX;
    [SerializeField]
    [Tooltip("How the jectpack is affected by gravity ( 0 makes it float)")]
    public float floatCap;
    [SerializeField]
    [Tooltip("The force applied horizontally")]
    float accelerationX;
    [SerializeField]
    [Tooltip("The force applied vertically when the player goes left or right")]
    float accelerationY;
    [SerializeField]
    [Tooltip("The force applied upwards when both buttons are pressed")]
    float accelerationUp;

    [Header("Gameplay ")]
    [Tooltip("Time to rev consecutively the engine")]
    [SerializeField]
    float revWindow;
    [SerializeField]
    float hitTaken;
    //Gameplay boolean
    bool engineUp,  reving;

    float revNumber, revTimer;

    [Tooltip("Must be the same as the camera")]
    [SerializeField]
    float offsetCam;

    [Header("Overheat ")]
    [Tooltip("Maximum heat you can manage")]
    [SerializeField]
    float maxHeat;
    [Tooltip("Heat gained per frame of active jet")]
    [SerializeField]
    float heatGainRatio;
    [Tooltip("Heat lost per frame of inactive jet")]
    [SerializeField]
    float heatLossRatio;

    [Tooltip("Critcal Heat gain per frame passed during critical heat")]
    [SerializeField]
    float critHeatGainRatio;
    [Tooltip("Critcal Heat gain per frame passed during critical heat")]
    [SerializeField]
    Image gauge;
    float actualHeat;

    [Tooltip("Horizontal speed when exploding left or right")]
    [SerializeField]
    public float explodeSpeedX;
    [Tooltip("Vertical speed when exploding left or right")]
    [SerializeField]
    public float explodeSpeedY;
    [Tooltip("Vertical speed when exploding up")]
    [SerializeField]
    public float explodeSpeedUp;

    bool criticalHeat;
    float lastHeat;
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
        effReving = GameObject.Find("Eff_Reving").GetComponent<ParticleSystem>();
        effRev = GameObject.Find("Eff_Rev").GetComponent<ParticleSystem>();
        effEngineOn = GameObject.Find("Eff_EngineOn").GetComponent<ParticleSystem>();
        engineUp = false;
        
    }


    // Update is called once per frame
    void Update()
    {
        InputUpdate();
        if (engineUp) { CapSpeed(); }

        if(reving) { EngineRevUpdate(); }
    }

    // Update is called a fixed number of time per second based on deltaTime

    private void FixedUpdate()
    {
        PackUpdate();
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






        //--Pump control--

        if (pumpActionDown) { PumpAction(); }

        //--Engine control--

        if (revEngineDown) { EngineRevAction(); }
    }


    // Function PackUpdate()
    // Updates the state of the jetpack. Must be called in Fixed update
    void PackUpdate()
    {
        if (engineUp)
        {
            //--Jet pack controls--
            lastHeat = actualHeat;
            if (rightJet && leftJet) { UpPackAction(); }
            else if (rightJet) { RightPackAction(); }
            else if (leftJet) { LeftPackAction(); }
            else { PackRest(); }

            
        }
        OverheatUpdate();
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
        if (actualHeat < maxHeat)
            actualHeat += heatGainRatio;
        rb_.AddForce(new Vector2(accelerationX , accelerationY));
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
        if (actualHeat < maxHeat)
            actualHeat += heatGainRatio;
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

        if (actualHeat < maxHeat)
            actualHeat += heatGainRatio*1.5f;
        rb_.AddForce(new Vector2(0, accelerationUp));
    }

    // Function PackRest()
    // Stops all jet particles emmiter and resets booleans associated to them + Apply passive heat management
    void PackRest()
    {
        if (!firstRight || !firstLeft || !firstUp)
        {
            effRightPack.Stop();
            effLeftPack.Stop();
            effUpPack.Stop();
            firstRight = firstLeft = firstUp = true;


        }
        if (actualHeat > 0)
        {
            if (criticalHeat)
                actualHeat += critHeatGainRatio;
            else
                actualHeat -= heatLossRatio;
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
        if (!engineUp)
        { 
            if (!reving)
            { 
                effReving.Play();
                revNumber = 0;
                reving = true;
            }
            revTimer = revWindow;
            revNumber++;
            effRev.Play();

            if (revNumber - hitTaken == 1)
            {
                effEngineOn.Play();
                effReving.Stop();
                engineUp = true;
                reving = false;
            }
        }
        else
        {

            switch (actualHeat / maxHeat)
            {
                case float n when n < 0.75f: actualHeat += maxHeat/4; break;
                default: Explode(); break;
            }
        }
    }

    // Function EngineRevUpdate()
    // Updates the reving state of the player if the player is starting his engine
    void EngineRevUpdate()
    {
        revTimer -= Time.deltaTime;
        if(revTimer <= 0)
        {
            effReving.Stop();
            Debug.Log(revTimer);
            reving = false;
            revNumber = 0;
        }
    }

    // Function OverheatUpdate()
    // Updates the overheat mechanic and indicator
    void OverheatUpdate()
    {

        gauge.fillAmount = Mathf.Lerp(lastHeat, actualHeat / maxHeat,1) ;



        switch (actualHeat / maxHeat)
        {
            case float n when n >= 0.75f:
                gauge.color = new Color(1f, 0, 0);
                if (!criticalHeat) { criticalHeat = true; }
                break;
            case float n when n >= 0.5f :  gauge.color = new Color(1f, 1f, 0); break;
            case float n when n >= 0.25f:  gauge.color = new Color(0, 0.8f, 0); break;
            default                     :  gauge.color = new Color(0, 0.4f, 0);  break;
        }

        if(actualHeat >= maxHeat)
        {
            Explode();
        }
    }

    void Explode()
    {
        actualHeat = 0;
        engineUp = false;
        criticalHeat = false;


        
        if(rightJet && leftJet)
        {
            rb_.velocity = new Vector3(rb_.velocity.x, 0);
            rb_.AddForce(new Vector2(0, explodeSpeedUp));
        }
        else if (rightJet)
        {
            rb_.AddForce(new Vector2(explodeSpeedX, explodeSpeedY));
        }
        else if (leftJet)
        {
            rb_.AddForce(new Vector2(-explodeSpeedX, explodeSpeedY));
        }
        else
        {
            rb_.velocity = new Vector3(rb_.velocity.x, 0);
            rb_.AddForce(new Vector2(0, explodeSpeedUp));
        }

    }

    // Function CapSpeed()
    // Caps the speed of the player
    void CapSpeed()
    {
        float ySpeed = rb_.velocity.y;
        float xSpeed = rb_.velocity.x;

        if (ySpeed > speedCapY)
        {
            ySpeed = speedCapY;


        }

        if (ySpeed < -floatCap)
        {
            ySpeed = -floatCap;

            
        }

        if (xSpeed > speedCapX)
        {
            xSpeed = speedCapX;

            
        }
        else if (xSpeed < -speedCapX)
        {
            xSpeed = -speedCapX;


        }

        rb_.velocity = new Vector2(xSpeed, ySpeed);

    }

    // Function CapSpeed() returns a boolean
    // Tells if the player is ascending
    public bool Rising()
    {
        
        return (rb_.velocity.y > speedCapY / offsetCam);
    }

    // Function Falling() returns a boolean
    // Tells if the player is falling
    public bool Falling()
    {

        return (rb_.velocity.y < -(floatCap / offsetCam));

    }

    // Function rbVelocityY() returns a float
    // returns the velocity on the Y axis
    public float rbVelocityY()
    {
        return rb_.velocity.y;
    }

}
