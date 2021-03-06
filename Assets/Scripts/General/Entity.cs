using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity : MonoBehaviour
{
    protected Controller controller;//Controller for handling all movement calculations
    public float faceDir;
    public bool blocking;

    public float minJumpHeight = 1;
    public float maxJumpHeight = 4;
    public float timeToJumpApex = .7f;
    protected float gravity;
    public float walkSpeed; //The player's walk speed
    protected float maxJumpVel;
    protected float minJumpVel;
    protected float velSmoothing;
    protected float velSmoothingY;
    protected float accelTimeAir = .1f;
    protected float accelTimeGround = .2f;

    public float wallSlideSpeed = 2f;
    public Vector2 wallJumpNeutral;
    public Vector2 wallJumpAway;
    public float wallStickTime = 1f;
    protected float timeToWallUnstick;
    protected bool wallSliding;
    protected int wallDirX;

    protected Vector2 dirInput;

    protected Animator animator;
    protected AnimatorStateInfo currentStateInfo;

    protected int currentState;
    protected int idleState = Animator.StringToHash("Base Layer.Idle");
    protected  int runState = Animator.StringToHash("Base Layer.Run");

    protected SpriteRenderer spriteRenderer; //sprite renderer property

    protected Vector3 vel; //Velocity vector

    public bool stunned = false;

    public GameObject Attack1Box, Attack2Box, Attack3Box;
    public Sprite Attack1HitSprite, Attack2HitSprite, Attack3HitSprite;

    public float health;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInput(Vector2 input)
    {
        dirInput = input;
        if ((int)Mathf.Sign(transform.localScale.x) > 0 && dirInput.x < 0)
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
        else if ((int)Mathf.Sign(transform.localScale.x) < 0 && dirInput.x > 0)
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
            Debug.Log((int)Mathf.Sign(dirInput.x));
        }

        animator.SetFloat("Speed", vel.sqrMagnitude);
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
            animator.SetBool("Jumping", true);
        }

    }

    public void OnJumpUp()
    {
        if (vel.y > minJumpVel)
            vel.y = minJumpVel;
    }

    protected void HandleWallSliding()
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

    protected void CalculateVelocity(bool altered)
    {
        if(!altered)
        {
            float targetVelX = dirInput.x * walkSpeed;
            vel.x = Mathf.SmoothDamp(vel.x, targetVelX, ref velSmoothing, controller.collisions.below ? accelTimeGround : accelTimeAir);
            vel.y += gravity * Time.deltaTime;
        }
        else
        {
            float targetVelX = dirInput.x * walkSpeed;
            float targetVelY = dirInput.y * walkSpeed * 2;
            vel.x = Mathf.SmoothDamp(vel.x, targetVelX, ref velSmoothing, accelTimeAir);
            vel.y = Mathf.SmoothDamp(vel.y, targetVelY, ref velSmoothingY, accelTimeAir);
        }
    }
    public void InstantStop()
    {
        vel.x = 0;
        vel.y = 0;
    }
}
