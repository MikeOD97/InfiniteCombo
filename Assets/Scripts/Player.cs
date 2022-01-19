using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Controller))]
public class Player : MonoBehaviour
{
    Controller controller;//Controller for handling all movement calculations

    public float minJumpHeight = 1;
    public float maxJumpHeight = 4;
    public float timeToJumpApex = .7f;
    float gravity;
    public float walkSpeed; //The player's walk speed
    float maxJumpVel;
    float minJumpVel;
    float velSmoothing;
    float accelTimeAir = .1f;
    float accelTimeGround = .2f;

    public float wallSlideSpeed = 2f;
    public Vector2 wallJumpNeutral;
    public Vector2 wallJumpAway;
    public float wallStickTime = 1f;
    float timeToWallUnstick;
    bool wallSliding;
    int wallDirX;

    Vector2 dirInput;
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
    //public Sprite[] idleSprites; //Array for the idle sprites
    //public Sprite[] walkingSprites; //Array for the walking sprites

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

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller>();
        spriteRenderer = GetComponent<SpriteRenderer>(); //set the sprite renderer
        StartCoroutine("Animate"); //Start the idle animation

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVel = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVel = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }

    void Update()
    {
        CalculateVelocity();
        HandleWallSliding();

        controller.Move(vel * Time.deltaTime, dirInput);

        if (controller.collisions.above || controller.collisions.below) //stop moving if on surface
            vel.y = 0;
    }

    public void SetInput(Vector2 input)
    {
        dirInput = input;
    }


    public void OnJumpDown()
    {
        if (wallSliding)
        {
            if (dirInput.x == 0 || wallDirX == dirInput.x)
            {
                vel.x = -wallDirX * wallJumpNeutral.x;
                vel.y = wallJumpNeutral.y;
            }
            else
            {
                vel.x = -wallDirX * wallJumpAway.x;
                vel.y = wallJumpAway.y;
            }
        }
        if (controller.collisions.below)
        {
            vel.y = maxJumpVel;
        }

    }

    public void OnJumpUp()
    {
        if (vel.y > minJumpVel)
            vel.y = minJumpVel;
    }

    //Animation coroutine
    //public IEnumerator Animate()
    //{
    //    //Depending on the player state, move through each frame of animation in the appropriate array
    //    switch(playerState)
    //    {
    //        case PlayerState.Idle:
    //            for (int i = 0; i < idleSprites.Length; i++)
    //            {
    //                if (playerState != PlayerState.Idle) break; //Break if the state changes mid loop
    //                spriteRenderer.sprite = idleSprites[i];
    //                yield return new WaitForSeconds(.1f);
    //            }
    //            break;

    //        case PlayerState.Walking:
    //            for (int i = 0; i < walkingSprites.Length; i++)
    //            {
    //                if (playerState != PlayerState.Walking) break;
    //                spriteRenderer.sprite = walkingSprites[i];
    //                yield return new WaitForSeconds(.1f);
    //            }
    //            break;
    //    }
       
    //    StartCoroutine("Animate"); //Keep it looping
    //}

    void HandleWallSliding()
    {

        wallSliding = false;
        wallDirX = (controller.collisions.left) ? -1 : 1;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && vel.y < 0)
        {
            wallSliding = true;
            if (vel.y < -wallSlideSpeed)
                vel.y = -wallSlideSpeed;

            if (timeToWallUnstick > 0)
            {
                velSmoothing = 0;
                vel.x = 0;
                if (dirInput.x != wallDirX && dirInput.x != 0)
                    timeToWallUnstick -= Time.deltaTime;
                else
                    timeToWallUnstick = wallStickTime;
            }
            else
                timeToWallUnstick = wallStickTime;

        }
    }

    void CalculateVelocity()
    {
        float targetVelX = dirInput.x * walkSpeed;
        vel.x = Mathf.SmoothDamp(vel.x, targetVelX, ref velSmoothing, controller.collisions.below ? accelTimeGround : accelTimeAir);
        vel.y += gravity * Time.deltaTime;
    }
}
