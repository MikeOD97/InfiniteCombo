using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    // Start is called before the first frame update
    float moveTimer = 0;
    Vector2 moveVel;
    public bool playerDetected;
    public GameObject player;
    public Collider2D visionCol;
    Collider2D playerCol;
    public bool launched;
    void Start()
    {
        controller = GetComponent<Controller>();
        spriteRenderer = GetComponent<SpriteRenderer>(); //set the sprite renderer
        animator = GetComponent<Animator>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVel = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVel = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        moveVel = new Vector2(1, 0);
        player = GameObject.FindGameObjectWithTag("Player");
        playerCol = player.GetComponent<BoxCollider2D>();
        health = 60;
/*         visionCol = gameObject.transform.GetChild(0).GetComponent<BoxCollider2D>();
        playerCol = player.GetComponent<BoxCollider2D>(); */
    }

    // Update is called once per frame
    void Update()
    {
        CalculateVelocity(false);
        HandleWallSliding();
        controller.Move(vel * Time.deltaTime, dirInput);

        currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        currentState = currentStateInfo.fullPathHash;

        if (controller.collisions.above || controller.collisions.below) //stop moving if on surface
            vel.y = 0;

        if(visionCol.IsTouching(playerCol))
            playerDetected = true;
        else
            playerDetected = false;
        faceDir = Mathf.Sign(gameObject.transform.localScale.x);

        if(health <= 0)
            GameObject.Destroy(gameObject);
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
    public void InstantStop()
    {
        vel.x = 0;
    }
    public IEnumerator Stunned(Vector2 stunForce, float stunTime, float attackStrength)
    {
        //from any different method, call "StartCoroutine(Stunned())
        health -= attackStrength;
        //healthBar.value = health;       
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

}
