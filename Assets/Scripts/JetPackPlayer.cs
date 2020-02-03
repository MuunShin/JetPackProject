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
    bool leftJet, rightJet, pumpAction, revEngine ;

    //External Components
    Rigidbody2D rb_;

    //GamePlay Variables
    [Header("Mouvement")]
    [SerializeField]
    float speedCapY, speedCapX;



    //------------------------//
    //       Functions        //
    //------------------------//

    // Start is called before the first frame update
    void Start()
    {
        rb_ = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        InputUpdate();
    }

    // Function InputUpdate
    // !Called in Update!  Manage inputs related to the player
    void InputUpdate()
    {
        //Updates the states of all the inputs the character can do
        // !! Inputs must be modified in Project Settings -> Input Manager !!

        rightJet = Input.GetButton("RightJet");
        leftJet = Input.GetButton("LeftJet");
        pumpAction = Input.GetButtonDown("PumpAction");
        revEngine = Input.GetButtonDown("RevEngine");

        //--Jet pack controls--
        
        if (rightJet && leftJet){ UpPackAction();    }
        else if (rightJet)      { RightPackAction(); }
        else if (leftJet)       { LeftPackAction();  }

        //--Pump control--

        if(pumpAction){ PumpAction(); }

        //--Engine control--

        if (revEngine){ EngineRevAction(); }
    }

    // Function RightPackAction()
    // Manage what does the character when the player uses his right Jet
    void RightPackAction()
    {
        rb_.AddForce(new Vector2(4, 10));
    }

    // Function LeftPackAction()
    // Manage what does the character when the player uses his left Jet
    void LeftPackAction()
    {
        rb_.AddForce(new Vector2(-4, 10));
    }

    // Function UpPackAction()
    // Manage what does the character when the player uses both of his Jets
    void UpPackAction()
    {
        Debug.Log(" UP ");
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
        }

        if (xSpeed > speedCapX)
        {
            xSpeed = speedCapX;
        }


    }
}
