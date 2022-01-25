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
        Vector2 dirInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //left and right movement
        Debug.Log(player);
        Debug.Log(dirInput);
        player.SetInput(dirInput);

        if (Input.GetKeyDown(KeyCode.Space))
            player.OnJumpDown();
        if (Input.GetKeyUp(KeyCode.Space))
            player.OnJumpUp();
        if(Input.GetMouseButtonDown(0))
            attackTimer = 0.4f;
        if(attackTimer > 0)
        {            
            attackTimer -= Time.deltaTime;
            player.Attack(attackTimer);
        }

    }
}
