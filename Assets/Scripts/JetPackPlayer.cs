using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    bool leftJet, rightJet, leftJetDown, rightJetDown, pumpAction, pumpActionDown, revEngine, revEngineDown;



    //External Components
    Rigidbody2D rb_;

    //Particles System
    ParticleSystem effRightPack, effLeftPack, effUpPack, effHeatLv1, effHeatLv2, effHeatLv3, effRev, effEngineOn;
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
    [Tooltip("Speed Gauge")]
    [SerializeField]
    Image speedGauge;
    float actualSpeed,tmpCap;

    [Header("Gameplay ")]
    [Tooltip("Time to rev consecutively the engine")]
    [SerializeField]
    float revWindow;
    [SerializeField]
    float hitTaken;
    //Gameplay boolean
    bool  reving;

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
    [Tooltip("Heat Gauge")]
    [SerializeField]
    Image gauge;
    float actualHeat;

    [Tooltip("Horizontal speed when exploding left or right")]
    public float explodeSpeedX;
    [Tooltip("Vertical speed when exploding left or right")]
    public float explodeSpeedY;
    [Tooltip("Vertical speed when exploding up")]
    public float explodeSpeedUp;
    [Tooltip("Number of imput in critical heat")]
    [SerializeField]
    int inputNumberCrit = 4;

    int checkState;
    bool criticalHeat, heating, initCrit;
    float lastHeat, noHeatTimer;
    
    int[] inputList;
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
        effHeatLv1 = GameObject.Find("Eff_HeatLv1").GetComponent<ParticleSystem>();
        effHeatLv2 = GameObject.Find("Eff_HeatLv2").GetComponent<ParticleSystem>();
        effHeatLv3 = GameObject.Find("Eff_HeatLv3").GetComponent<ParticleSystem>();
        effRev = GameObject.Find("Eff_Rev").GetComponent<ParticleSystem>();
        effEngineOn = GameObject.Find("Eff_EngineOn").GetComponent<ParticleSystem>();

        heating = true;
        noHeatTimer = 0;
    }
    
    // TEMPORAIRE
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    // Update is called once per frame
    void Update()
    {
        InputUpdate();
        CapSpeed();

        if(reving) { EngineRevUpdate(); }
        if (criticalHeat) { CriticalPackUpdate(); }
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
        rightJetDown = Input.GetButtonDown("RightJet");
        leftJetDown = Input.GetButtonDown("LeftJet");
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
        int heatMode;
        //--Jet pack controls--
        switch (criticalHeat)
        {
            case true:

                heatMode = 4;
                break;
            default:
                if (rightJet && leftJet) { UpPackAction(); heatMode = 3; }
                else if (rightJet) { RightPackAction(); heatMode = 2; }
                else if (leftJet) { LeftPackAction(); heatMode = 1; }
                else { PackRest(); heatMode = 0; }
                break;

        }
        SpeedUpdate();

        if (heating)
        {

            lastHeat = actualHeat;
            OverheatUpdate(heatMode);
        }
    }

    void CriticalPackUpdate()
    {
        switch (criticalHeat)
        {
            case true:
                if (rightJetDown) { CriticalSequenceUpdate(2); }
                else if (leftJetDown) { CriticalSequenceUpdate(1); }
                else { CriticalSequenceUpdate(0); }

                break;

            default:

                break;
        }

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
        if (!criticalHeat)
        {
            switch (actualHeat / maxHeat)
            {
                case float n when 0.21f < n && n < 0.29f: HeatTiming(1); break;
                case float n when 0.46f < n && n < 0.54f: HeatTiming(2); break;
                case float n when 0.71f < n && n < 0.79f: HeatTiming(3); break;
                default: actualHeat += maxHeat / 10; break;
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
            Debug.Log(revTimer);
            reving = false;
            revNumber = 0;
        }
    }

    // Function OverheatUpdate()
    // Updates the overheat mechanic and indicator
    void OverheatUpdate(int heatMode)
    {
        
        switch(heatMode)
        {
            case 0:
                if (actualHeat > 0)
                {
                    actualHeat -= heatLossRatio;
                }
                break;

            case 1:
                actualHeat += heatGainRatio;
                break;
            case 2:
                actualHeat += heatGainRatio;
                break;
            case 3: actualHeat += heatGainRatio *1.5f; break ;
            case 4: actualHeat += critHeatGainRatio; break;
        }


          
    

        gauge.fillAmount = actualHeat / maxHeat;

        switch (actualHeat / maxHeat)
        {
            case float n when (0.21f < n && n < 0.29f) || (0.46f < n && n < 0.54f) || (0.71f < n && n < 0.79f): gauge.color = new Color(0.8f, 0.8f, 0.8f); break;
            case float n when n >= 0.79f:
                gauge.color = new Color(1f, 0, 0);
                if (!criticalHeat)
                {
                    tmpCap = floatCap;
                    floatCap = 0;
                    criticalHeat = true;
                }
                rb_.velocity = Vector2.zero;

                break;
            case float n when n >= 0.54f:  gauge.color = new Color(1f, 1f, 0); break;
            case float n when n >= 0.29f:  gauge.color = new Color(0, 0.8f, 0); break;
            default                     :  gauge.color = new Color(0, 0.4f, 0);  break;
        }

        if(actualHeat >= maxHeat)
        {
            Explode();
        }
    }

    void SpeedUpdate()
    {
        actualSpeed = rb_.velocity.magnitude;

        speedGauge.fillAmount = actualSpeed / 15;
    }


    void CriticalSequenceUpdate(int inputIn)
    {
        if(!initCrit)
        {
            checkState = 0;
            inputList = new int[inputNumberCrit];
            inputList = new int[] { 1,2,1,2 };
            initCrit = true;
        }

        

        switch (inputIn)
        {

            case 0:
                Debug.Log("No in   Attendu :" + inputList[checkState]);
                break;
            default:
                Debug.Log(inputIn + "   Attendu :" + inputList[checkState]);
                if (inputIn == inputList[checkState])
                {
                    checkState++;
                    if (checkState == inputNumberCrit)
                        EndCritical();

                }
                else
                {
                    checkState = 0; 
                }
                break;
        }

        
    }

    void EndCritical()
    {
        actualHeat = maxHeat / 3;
        criticalHeat = false;
        floatCap = tmpCap;
        initCrit = false;
    }

    void Explode()
    {
        SceneManager.LoadScene("TestScene");
        actualHeat = 0;
        criticalHeat = false;
        
        if(rightJet && leftJet)
        {
            //rb_.velocity = new Vector3(rb_.velocity.x, 0);
            //rb_.AddForce(new Vector2(0, explodeSpeedUp));
        }
        else if (rightJet)
        {
            //rb_.AddForce(new Vector2(explodeSpeedX, explodeSpeedY));
        }
        else if (leftJet)
        {
            //rb_.AddForce(new Vector2(-explodeSpeedX, explodeSpeedY));
        }
        else
        {
            //rb_.velocity = new Vector3(rb_.velocity.x, 0);
            //rb_.AddForce(new Vector2(0, explodeSpeedUp));
        }

    }

    void HeatTiming(int level)
    {
        actualHeat = 0;
        heating = false;

        switch(level)
        {
            case 1:
                effHeatLv1.Play();
                StartCoroutine(NoHeatWhile(1f,1));

                break;
            case 2:
                effHeatLv2.Play();
                StartCoroutine(NoHeatWhile(2f,2));

                break;
            case 3:
                effHeatLv3.Play();
                StartCoroutine(NoHeatWhile(3f,3));

                break;
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

    IEnumerator NoHeatWhile(float timer, int level)
    {

        while (noHeatTimer < timer)
        {
            noHeatTimer += Time.deltaTime;

            yield return null;

        }
        noHeatTimer = 0;
        actualHeat = 0;
        heating = true;

        switch (level)
        {
            case 1:
                effHeatLv1.Stop();
                break;
            case 2:
                effHeatLv2.Stop();
                break;
            case 3:
                effHeatLv3.Stop();
                break;
            default:
                break;
        }
        StopCoroutine(NoHeatWhile(timer, level));
        yield return null;
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
