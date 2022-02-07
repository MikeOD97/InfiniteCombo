using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollisions : MonoBehaviour
{
    public float attackStrength;
    public Vector2 launchDir;
    public float stunTime;
    GameObject otherObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if(gameObject.tag == "PlayerAttackBox" && other.tag == "EnemyHitBox")
        {
            EnemyDamage(other.gameObject, launchDir, stunTime);
        }
        else if(gameObject.tag == "EnemyAttackBox" && other.tag == "PlayerHitBox")
        {
            PlayerDamage(other.gameObject, launchDir, stunTime);
        }
    }

    void PlayerDamage(GameObject obj, Vector2 launchDir, float stunTime)
    {
        otherObject = obj.transform.parent.gameObject;
        Player player = otherObject.GetComponent<Player>();
        player.PlayStun(launchDir, stunTime);
    }
    void EnemyDamage(GameObject obj, Vector2 launchDir, float stunTime)
    {
        otherObject = obj.transform.parent.gameObject;
        Enemy enemy = otherObject.GetComponent<Enemy>();
        enemy.PlayStun(launchDir, stunTime);
    }
}
