using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollisions : MonoBehaviour
{
    public float attackStrength;
    public Vector2 launchDir;
    public float stunTime;
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
            EnemyDamage(other.gameObject, gameObject, launchDir, stunTime);
        }
        else if(gameObject.tag == "EnemyAttackBox" && other.tag == "PlayerHitBox")
        {
            PlayerDamage(other.gameObject, gameObject, launchDir, stunTime);
        }
    }

    void PlayerDamage(GameObject obj, GameObject other, Vector2 launchDir, float stunTime)
    {
        Player player = obj.transform.parent.gameObject.GetComponent<Player>();
        Enemy enemy = other.transform.parent.gameObject.GetComponent<Enemy>();
        player.PlayStun(new Vector2(launchDir.x * enemy.faceDir, launchDir.y), stunTime);
        player.health -= attackStrength;
    }
    void EnemyDamage(GameObject obj, GameObject other, Vector2 launchDir, float stunTime)
    {
        Enemy enemy = obj.transform.parent.gameObject.GetComponent<Enemy>();
        Player player = other.transform.parent.gameObject.GetComponent<Player>();
        enemy.PlayStun(new Vector2(launchDir.x * player.faceDir, launchDir.y), stunTime);
        enemy.health -= attackStrength;
    }
}
