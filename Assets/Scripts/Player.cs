﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Controller))]
public class Player : MonoBehaviour
{
    Controller controller;//Controller for handling all movement calculations
    //FSM for the player's animation state
    public enum PlayerState
    {
        Idle,
        Walking,
        Jumping,
        Attacking
    }
    PlayerState playerState = PlayerState.Idle; // Set it to idle first

    private SpriteRenderer spriteRenderer; //sprite renderer property
    public Sprite[] idleSprites; //Array for the idle sprites
    public Sprite[] walkingSprites; //Array for the walking sprites

    private Vector3 vel; //Velocity vector
    public Vector3 Velocity
    {
        get { return vel; }
        set { vel = value; }
    }
    private Vector3 acc; //Acceleration vector
    public Vector3 Acceleration
    {
        get { return acc; }
        set { acc = value; }
    }
    public float mass = 1f; //The mass of the player
    public float walkSpeed; //The player's walk speed

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller>();
        spriteRenderer = GetComponent<SpriteRenderer>(); //set the sprite renderer
        StartCoroutine("Animate"); //Start the idle animation
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        vel.x = input.x * -walkSpeed;
        controller.Move(vel * Time.deltaTime);
        //if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) playerState = PlayerState.Idle; //Make the player idle if the user isn't pressing a button

        ////Code to walk right
        //if (Input.GetKey(KeyCode.D))
        //{
        //    transform.localRotation = Quaternion.Euler(0, 0, 0); //make sure the sprite faces right
        //    playerState = PlayerState.Walking; //Set the enum accurately
        //    acc += walkSpeed * new Vector3(1, 0) / mass; //Calculate acceleration
           
        //}
        ////Do the same thing for left
        //else if (Input.GetKey(KeyCode.A))
        //{
        //    transform.localRotation = Quaternion.Euler(0, 180, 0);
        //    playerState = PlayerState.Walking;
        //    acc += walkSpeed * new Vector3(-1, 0) / mass;
        //}
        //vel += acc; //Calculate velocity
        //vel += vel * -1f * 0.15f; //friction
        //transform.position += (Vector3)vel; //add velocity to position
        //GetComponent<SpriteRenderer>().sprite = spriteRenderer.sprite; //Set the sprite accurately
        //acc = Vector3.zero; //Reset the acceleration vector
    }

    //Animation coroutine
    public IEnumerator Animate()
    {
        //Depending on the player state, move through each frame of animation in the appropriate array
        switch(playerState)
        {
            case PlayerState.Idle:
                for (int i = 0; i < idleSprites.Length; i++)
                {
                    if (playerState != PlayerState.Idle) break; //Break if the state changes mid loop
                    spriteRenderer.sprite = idleSprites[i];
                    yield return new WaitForSeconds(.1f);
                }
                break;

            case PlayerState.Walking:
                for (int i = 0; i < walkingSprites.Length; i++)
                {
                    if (playerState != PlayerState.Walking) break;
                    spriteRenderer.sprite = walkingSprites[i];
                    yield return new WaitForSeconds(.1f);
                }
                break;
        }
       
        StartCoroutine("Animate"); //Keep it looping
    }
}