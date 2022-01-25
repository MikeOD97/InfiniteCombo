using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Controller))]
public class Player : Entity
{
    // Start is called before the first frame update
    public GameObject Attack1Box, Attack2Box, Attack3Box;
    public Sprite Attack1HitSprite, Attack2HitSprite, Attack3HitSprite;

    void Start()
    {
        controller = GetComponent<Controller>();
        spriteRenderer = GetComponent<SpriteRenderer>(); //set the sprite renderer
        animator = GetComponent<Animator>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVel = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVel = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }
    void Update()
    {
        CalculateVelocity();
        HandleWallSliding();
        controller.Move(vel * Time.deltaTime, dirInput);

        currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        currentState = currentStateInfo.fullPathHash;

        if (controller.collisions.above || controller.collisions.below) //stop moving if on surface
            vel.y = 0;
    }
    public void Attack(float timer)
    {
        if(timer <= 0)
            animator.SetBool("Attacking", false);
        else
            animator.SetBool("Attacking", true);
    }
    
}
