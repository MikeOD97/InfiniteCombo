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

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //left and right movement
        int wallDirX = (controller.collisions.left) ? -1 : 1;

        float targetVelX = input.x * walkSpeed;
        vel.x = Mathf.SmoothDamp(vel.x, targetVelX, ref velSmoothing, controller.collisions.below ? accelTimeGround : accelTimeAir);

        bool wallSliding = false;
        if((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && vel.y < 0)
        {
            wallSliding = true;
            if (vel.y < -wallSlideSpeed)
                vel.y = -wallSlideSpeed;

            if (timeToWallUnstick > 0)
            {
                velSmoothing = 0;
                vel.x = 0;
                if (input.x != wallDirX && input.x != 0)
                    timeToWallUnstick -= Time.deltaTime;
                else
                    timeToWallUnstick = wallStickTime;
            }
            else
                timeToWallUnstick = wallStickTime;

        }
        if (controller.collisions.above || controller.collisions.below) //stop moving if on surface
            vel.y = 0;

        //Debug.Log(controller.collisions.below);
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(wallSliding)
            {
                if(input.x == 0 || wallDirX == input.x)
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
            if(controller.collisions.below)
                vel.y = maxJumpVel;
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            if(vel.y > minJumpVel)
                vel.y = minJumpVel;
        }

        vel.y += gravity * Time.deltaTime;
        controller.Move(vel * Time.deltaTime, input);
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
