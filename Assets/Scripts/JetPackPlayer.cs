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
    bool leftJet, rightJet, leftJetDown, rightJetDown, revEngineUp, revEngineDown;



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
    private float accelerationX;
    [SerializeField]
    [Tooltip("The force applied vertically when the player goes left or right")]
    private float accelerationY;
    [SerializeField]
    [Tooltip("The force applied upwards when both buttons are pressed")]
    private float accelerationUp;
    [Tooltip("Speed Gauge")]
    [SerializeField]
    Image speedGauge;
    private float actualSpeed,tmpCap;

    [Header("Gameplay ")]

    [Tooltip("The added force while boosting Up")]
    [SerializeField]
    private float speedCollisionMalus;

    [Tooltip("Must be the same as the camera")]
    [SerializeField]
    private float offsetCam;

    [Header("Boost ")]
    [Tooltip("Boost Gauge")]
    [SerializeField]
    private Image gauge;
    [Tooltip("Boost Gauge")]
    [SerializeField]
    private Image stockGauge;
    [Tooltip("Boost max")]
    [SerializeField]
    private float maxBoost;
    [Tooltip("The added force while boosting horizontally")]
    [SerializeField]
    private float accelBoostX;
    [Tooltip("The added force while boosting vertically")]
    [SerializeField]
    private float accelBoostY;
    [Tooltip("The added force while boosting Up")]
    [SerializeField]
    private float accelBoostUp;
    [Tooltip("Upward vertical speed cap boosted")]
    [SerializeField]
    public float speedCapYB;
    [SerializeField]
    [Tooltip("Horizontal speed cap boosted")]
    public float speedCapXB;
    [Tooltip("Boost ratio")]
    [SerializeField]
    private float boostRatioGain;
    [Tooltip("Boost ratio")]
    [SerializeField]
    private float boostRatioLoss;

    [Header("WallBreak")]
    [Tooltip("Boost Gauge")]
    [SerializeField]
    private float wallBreakSpeedBest;
    [Tooltip("Boost Gauge")]
    [SerializeField]
    private float wallBreakSpeedSlow;

    [Header("Speed Pads")]
    [Tooltip("Boost Gauge")]
    [SerializeField]
    private float speedPadX;
    [Tooltip("Boost Gauge")]
    [SerializeField]
    private float speedPadY;
    [Tooltip("Boost Gauge")]
    [SerializeField]
    private AnimationCurve curve;
    [Tooltip("Boost Gauge")]
    [SerializeField]
    private float speedPadAccelAdd;

    [Header("Wind")]
    [Tooltip("The added force while boosting horizontally")]
    [SerializeField]
    private float accelWindX;
    [Tooltip("The added force while boosting vertically")]
    [SerializeField]
    private float accelWindY;
    [Tooltip("The added force while boosting Up")]
    [SerializeField]
    private float accelWindUp;
    [Tooltip("Upward vertical speed cap boosted")]
    [SerializeField]
    public float speedCapYWind;
    [SerializeField]
    [Tooltip("Horizontal speed cap boosted")]
    public float speedCapXWind;

    private float actualBoost, boostStock, position;
    bool isBoosting, lockedBoost, nearWall, winded, speedPadBoost,paused;
    float actualCapX, actualCapY, normX, normY;

    AudioSource jet, boost, boostCharge, boostMax, bunk, speedPad, bumper, wallBreak;

    [SerializeField]
    AudioClip jetClassic, jetActivate;

    //------------------------//
    //       Functions        //
    //------------------------//

    // Start is called before the first frame update
    private void OnDisable()
    {
        jet.Stop();
    }

    void Start()
    {


        rb_ = GetComponent<Rigidbody2D>();
        effRightPack = GameObject.Find("Eff_PackR").GetComponent<ParticleSystem>();
        effLeftPack = GameObject.Find("Eff_PackL").GetComponent<ParticleSystem>();
        effUpPack = GameObject.Find("Eff_PackUP").GetComponent<ParticleSystem>();
        actualCapX = speedCapX;
        actualCapY = speedCapY;
        lockedBoost = false;
        speedPadBoost = false;

        jet = transform.Find("SoundEmmiter").Find("jet").GetComponent<AudioSource>();
        boost = transform.Find("SoundEmmiter").Find("boost").GetComponent<AudioSource>();
        boostCharge = transform.Find("SoundEmmiter").Find("boostCharge").GetComponent<AudioSource>();
        bunk = transform.Find("SoundEmmiter").Find("bunk").GetComponent<AudioSource>();
        speedPad = transform.Find("SoundEmmiter").Find("speedPad").GetComponent<AudioSource>();
        wallBreak = transform.Find("SoundEmmiter").Find("wallBreak").GetComponent<AudioSource>();
        bumper = transform.Find("SoundEmmiter").Find("bumper").GetComponent<AudioSource>();
        boostMax = transform.Find("SoundEmmiter").Find("boostMax").GetComponent<AudioSource>();
    }

    public void Paused(bool pause)
    {
        paused = pause;
        switch(pause)
        {
            case true: jet.Stop(); break;
            case false: jet.Play(); break;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused)
        {
            InputUpdate();
        }
        CapSpeed();
    }

    // Update is called a fixed number of time per second based on deltaTime

    private void FixedUpdate()
    {
        PackUpdate();
        BoostUpdate();
        if (speedPadBoost)
            SpeedPadAccel();
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

        revEngineDown = Input.GetButtonDown("RevEngine");
        revEngineUp = Input.GetButtonUp("RevEngine");

        if (Input.GetButtonDown("pauseButton"))
        {
            
        }
            


        //--Pump control--


        //--Engine control--

        if (revEngineDown) { BoostForwardAction(); }
        if (revEngineUp) { BoostForwardStop(); }
    }


    // Function PackUpdate()
    // Updates the state of the jetpack. Must be called in Fixed update
    void PackUpdate()
    {
        //--Jet pack controls--


        if (rightJet && leftJet) { UpPackAction(); }
        else if (rightJet) { RightPackAction();  }
        else if (leftJet) { LeftPackAction(); }
        else { PackRest();  }

        if ((rightJet || leftJet) && !jet.isPlaying)
        {
            jet.Play();
        }

        if (isBoosting && jet.clip != jetActivate)
        {

            jet.clip = jetActivate;
            jet.Play();
        }

        if (!isBoosting && jet.clip != jetClassic)
        {
            jet.clip = jetClassic;
            jet.Play();
        }

        SpeedUpdate();
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

        float tmpAccelX = accelerationX;
        float tmpAccelY = accelerationY;

        if (winded)
        {
            tmpAccelX += accelWindX;
            tmpAccelY += accelWindY;
        }
        if (isBoosting)
        {
            tmpAccelX += accelBoostX;
            tmpAccelY += accelBoostY;
        }
        rb_.AddForce(new Vector2(tmpAccelX , tmpAccelY));
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

        float tmpAccelX = accelerationX;
        float tmpAccelY = accelerationY;

        if (winded)
        {
            tmpAccelX += accelWindX;
            tmpAccelY += accelWindY;
        }
        if (isBoosting)
        {
            tmpAccelX += accelBoostX;
            tmpAccelY += accelBoostY;
        }
        rb_.AddForce(new Vector2(-tmpAccelX, tmpAccelY));
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


        float tmpAccelUp = accelerationUp;

        if (winded)
        {
            tmpAccelUp += accelWindUp;
        }
        if (isBoosting)
        {
            tmpAccelUp += accelBoostUp;
        }
        rb_.AddForce(new Vector2(0, tmpAccelUp));
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

    // Function BoostForwardAction()
    // Manage what does the character when the player uses his boost input
    void BoostForwardAction()
    {
        if(!isBoosting && lockedBoost == false)
        {
            
            isBoosting = true;
            actualCapX = speedCapXB;
            actualCapY = speedCapYB;

            if(gauge.fillAmount != 0)
            {
                boost.Play();
            }
        }

    }

    void BoostForwardStop()
    {

        isBoosting = false;
        lockedBoost = false;
        actualCapX = speedCapX;
        actualCapY = speedCapY;
    }

    void BoostUpdate()
    {
        gauge.fillAmount = actualBoost / maxBoost;
        stockGauge.fillAmount = (actualBoost +boostStock) / maxBoost;

        if (isBoosting)
        {
            actualBoost -= boostRatioLoss;
        }

        if (actualBoost <= 0 && isBoosting)
        {
            actualBoost = 0;
            isBoosting = false;
            actualCapX = speedCapX;
            actualCapY = speedCapY;
            lockedBoost = true;
        }

        if ((boostStock < maxBoost) && nearWall)
        {

            boostStock += boostRatioGain * (actualSpeed / 10);
            boostCharge.pitch = stockGauge.fillAmount;
        }
    }

    void SpeedUpdate()
    {
        actualSpeed = rb_.velocity.magnitude;

        speedGauge.fillAmount = actualSpeed / 25;
        jet.pitch = actualSpeed / 25; 
    }

    // Function CapSpeed()
    // Caps the speed of the player
    void CapSpeed()
    {
        float ySpeed = rb_.velocity.y;
        float xSpeed = rb_.velocity.x;

        if (ySpeed > actualCapY)
        {
            ySpeed = Mathf.Lerp(ySpeed,actualCapY,0.004f);
        }

        if (ySpeed < -floatCap)
        {
            ySpeed = -floatCap;
        }

        if (xSpeed > actualCapX)
        {
            xSpeed = Mathf.Lerp(xSpeed, actualCapX, 0.004f); 
        }
        else if (xSpeed < -actualCapX)
        {
            xSpeed = Mathf.Lerp(xSpeed, -actualCapX, 0.004f); 
        }

        rb_.velocity = new Vector2(xSpeed, ySpeed);

    }

    public void BoostChargeStart()
    {
        boostStock = 0;
        nearWall = true;
        boostCharge.Play();
    }
    public void BoostChargeStop()
    {
        boostCharge.Stop();
        actualBoost += boostStock;
        if (actualBoost > maxBoost)
        {
            actualBoost = maxBoost;
            boostMax.Play();

        }
        lockedBoost = false;

        nearWall = false;
        boostStock = 0;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Wall")
        {
            rb_.velocity = new Vector2(rb_.velocity.x - (rb_.velocity.x / speedCollisionMalus), rb_.velocity.y - (rb_.velocity.y / speedCollisionMalus));
            boostStock = 0;
            nearWall = false;
            boostCharge.Stop();
            bunk.Play();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Breakable")
        {
            if (actualSpeed > wallBreakSpeedBest)
            {
                collision.gameObject.GetComponent<BreakWall>().Destruction();
                wallBreak.Play();
            }
            else if (actualSpeed > wallBreakSpeedSlow)
            {
                collision.gameObject.GetComponent<BreakWall>().Destruction();
                rb_.velocity = new Vector2(rb_.velocity.x - (rb_.velocity.x / speedCollisionMalus), rb_.velocity.y - (rb_.velocity.y / speedCollisionMalus));
                wallBreak.Play();
            }

        }

        if (collision.gameObject.tag == "Wind")
        {
            winded = true;
            actualCapX += speedCapXWind;
            actualCapY += speedCapYWind;
        }

        if (collision.gameObject.tag == "BoostPad")
        {

            if (!speedPadBoost)
            {
                speedPadBoost = true;
                speedPad.Play();
                normX = collision.transform.right.normalized.x;
                normY = collision.transform.right.normalized.y;
            }
            position = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wind")
        {
            winded = false;
            actualCapX -= speedCapXWind;
            actualCapY -= speedCapYWind;
        }
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
    public float RbVelocityY()
    {
        return rb_.velocity.y;
    }

    public float RbVelocityX()
    {
        return rb_.velocity.x;
    }

    public float RbVelocityXNorm()
    {
        /*if(-1 < rb_.velocity.x && rb_.velocity.x < 1)
            return rb_.velocity.x;
        else*/
            return rb_.velocity.normalized.x;
    }

    private void SpeedPadAccel()
    {

        float speedBoost;
        if (position < 1)
        {
            position += speedPadAccelAdd;
            speedBoost = curve.Evaluate(position);

            rb_.AddForce(new Vector2(normX * (speedBoost * speedPadX), normY * (speedBoost * speedPadY)));

        }
        else
            speedPadBoost = false;

    }

    //rb_.AddForce(new Vector2(rb_.velocity.normalized.x* speedPadX, rb_.velocity.normalized.y* speedPadY));
}
