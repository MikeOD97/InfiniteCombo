using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Controller))]
public class Player : Entity
{
    // Start is called before the first frame update
    float startWalkSpeed;
    public Slider healthBar;
    public Enemy engagedEnemy;
    public bool extending;
    float desiredDistance = 1;
    void Start()
    {
        controller = GetComponent<Controller>();
        spriteRenderer = GetComponent<SpriteRenderer>(); //set the sprite renderer
        animator = GetComponent<Animator>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVel = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVel = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        startWalkSpeed = walkSpeed;
        health = 100;
    }
    void Update()
    {
        CalculateVelocity(extending);
        HandleWallSliding();
        controller.Move(vel * Time.deltaTime, dirInput);

        currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        currentState = currentStateInfo.fullPathHash;

        if (controller.collisions.above || controller.collisions.below) //stop moving if on surface
        {
            vel.y = 0;
            animator.SetBool("Jumping", false);
        }
        faceDir = Mathf.Sign(gameObject.transform.localScale.x);
    }
    public void Attack(float timer)
    {
        if(timer <= 0)
        {
            animator.SetBool("Attacking", false);
        }

        else
        {
            animator.SetBool("Attacking", true);              
        }
        if(Attack1HitSprite == spriteRenderer.sprite)
            Attack1Box.gameObject.SetActive(true);
        else if (Attack2HitSprite == spriteRenderer.sprite)
            Attack2Box.gameObject.SetActive(true);
        else if(Attack3HitSprite == spriteRenderer.sprite)
             Attack3Box.gameObject.SetActive(true);
        else
        {
            Attack1Box.gameObject.SetActive(false);
            Attack2Box.gameObject.SetActive(false);
            Attack3Box.gameObject.SetActive(false);
        }
    }
    public void Block(bool block)
    {
        if(block)
        {
            animator.SetBool("Blocking", true);
            walkSpeed = 0;
            blocking = true;
        }
        else
        {
            animator.SetBool("Blocking", false);
            walkSpeed = startWalkSpeed;
            blocking = false;
        }

    }
    
    public void Damaged(bool hit)
    {
        animator.SetBool("Damaged", hit);
    }

    //This'll be used in the future when I figure out how I wanna implement knock-down mechanics, 
    //points where you're unable to act, times where you're sent flying, etc.
    public IEnumerator Stunned(Vector2 stunForce, float stunTime, float attackStrength)
    {
        //from any different method, call "StartCoroutine(Stunned())
        health -= attackStrength;
        healthBar.value = health;       
        vel += (Vector3)stunForce;    
        
        if(!blocking)
        {
            animator.Play("Hurt");
            stunned = true;
            animator.SetBool("Stunned", true);

            yield return new WaitForSeconds(stunTime);
            stunned = false;
            animator.SetBool("Stunned", false);
            animator.Play("Idle");
        }   
    }
    public void PlayStun(Vector2 stunForce, float stunTime, float attackStrength)
    {
        if(blocking)
        {
            attackStrength /= 3;
            stunForce /= 3;
        }
        StartCoroutine(Stunned(stunForce, stunTime, attackStrength));
    }
    public IEnumerator ComboExtend(Vector2 distance)
    {
        float extension = Vector3.Distance(engagedEnemy.transform.position, transform.position);
        while(desiredDistance < extension)
        {
            extension = Vector3.Distance(engagedEnemy.transform.position, transform.position);
            extending = true;
            SetInput(distance);
            yield return null;
        }
        extending = false;
        InstantStop();
    }
}
