using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    // Start is called before the first frame update
    float moveTimer = 0;
    Vector2 moveVel;
    void Start()
    {
        controller = GetComponent<Controller>();
        spriteRenderer = GetComponent<SpriteRenderer>(); //set the sprite renderer
        animator = GetComponent<Animator>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVel = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVel = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        moveVel = new Vector2(1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateVelocity();
        HandleWallSliding();
        controller.Move(vel * Time.deltaTime, dirInput);

        currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        currentState = currentStateInfo.fullPathHash;

        if (controller.collisions.above || controller.collisions.below) //stop moving if on surface
            vel.y = 0;

        if(moveTimer < 2)
        {
            SetInput(moveVel);
            moveTimer += Time.deltaTime;
        }
        else
        {
            moveVel.x *= -1;
            moveTimer = 0;
        }
    }
}
