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
    [Tooltip("Boost ratio")]
    [SerializeField]
    private float boostRatioGain;
    [Tooltip("Boost ratio")]
    [SerializeField]
    private float boostRatioLoss;

    private float actualBoost, boostStock;
    bool isBoosting, lockedBoost, nearWall;
    float actualCapX, actualCapY;

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
        actualCapX = speedCapX;
        actualCapY = speedCapY;
        lockedBoost = false;

    }


    // Update is called once per frame
    void Update()
    {
        InputUpdate();
        CapSpeed();
    }

    // Update is called a fixed number of time per second based on deltaTime

    private void FixedUpdate()
    {
        PackUpdate();
        BoostUpdate();                          
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


        if(isBoosting)
            rb_.AddForce(new Vector2(accelerationX + accelBoostX, accelerationY + accelBoostY));
        else
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

        if (isBoosting)
            rb_.AddForce(new Vector2(-accelerationX - accelBoostX, accelerationY + accelBoostY));
        else
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

        if (isBoosting)
            rb_.AddForce(new Vector2(0, accelerationUp + accelBoostUp));
        else
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

    // Function BoostForwardAction()
    // Manage what does the character when the player uses his boost input
    void BoostForwardAction()
    {
        if(!isBoosting && lockedBoost == false)
        {
            isBoosting = true;
            actualCapX = speedCapXB;
            actualCapY = speedCapYB;
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
        Debug.Log("near Wall : " + nearWall);
        gauge.fillAmount = actualBoost / maxBoost;
        stockGauge.fillAmount = (actualBoost +boostStock) / maxBoost;

        if (isBoosting)
        {
            actualBoost -= boostRatioLoss;
        }

        if (actualBoost <= 0)
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
        }
    }

    void SpeedUpdate()
    {
        actualSpeed = rb_.velocity.magnitude;

        speedGauge.fillAmount = actualSpeed / 15;
    }

    // Function CapSpeed()
    // Caps the speed of the player
    void CapSpeed()
    {
        float ySpeed = rb_.velocity.y;
        float xSpeed = rb_.velocity.x;

        if (ySpeed > actualCapY)
        {
            ySpeed = Mathf.Lerp(ySpeed,actualCapY,0.1f);
        }

        if (ySpeed < -floatCap)
        {
            ySpeed = -floatCap;
        }

        if (xSpeed > actualCapX)
        {
            xSpeed = Mathf.Lerp(xSpeed, actualCapX, 0.1f); 
        }
        else if (xSpeed < -actualCapX)
        {
            xSpeed = Mathf.Lerp(xSpeed, -actualCapX, 0.1f); 
        }

        rb_.velocity = new Vector2(xSpeed, ySpeed);

    }

    public void BoostChargeStart()
    {
        boostStock = 0;
        nearWall = true;
    }
    public void BoostChargeStop()
    {
        actualBoost += boostStock;
        if (boostStock > maxBoost)
            actualBoost = maxBoost;

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

}
