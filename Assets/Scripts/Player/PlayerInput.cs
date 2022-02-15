using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    Player player;
    float attackTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(1))
            player.Block(false);
        if(!player.stunned)
        {
            if(!player.extending)
            {
                Vector2 dirInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //left and right movement
                player.SetInput(dirInput);

                if (Input.GetKeyDown(KeyCode.Space))
                    player.OnJumpDown();
                if (Input.GetKeyUp(KeyCode.Space))
                    player.OnJumpUp();
            }

            if(Input.GetMouseButtonDown(0))
                attackTimer = 0.4f;
            else if(Input.GetMouseButton(1))
                player.Block(true);
            if(attackTimer > 0)
            {            
                attackTimer -= Time.deltaTime;          
            }
            player.Attack(attackTimer);
            if(Input.GetMouseButtonDown(2))
            {
                if(player.engagedEnemy != null && player.engagedEnemy.launched)
                {
                    StartCoroutine(player.ComboExtend(player.engagedEnemy.transform.position - player.transform.position));
                }
            }
        }
        else
        {
            player.SetInput(Vector2.zero);
        }
           
    }
}
