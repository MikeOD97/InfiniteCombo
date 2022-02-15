using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollisions : MonoBehaviour
{
    public float attackStrength;
    public Vector2 launchDir;
    public float stunTime;
    public bool launcher;
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
        float damage = attackStrength;
        Vector2 force = launchDir;
        Player player = obj.transform.parent.gameObject.GetComponent<Player>();
        Enemy enemy = other.transform.parent.gameObject.GetComponent<Enemy>();
        player.PlayStun(new Vector2(force.x * enemy.faceDir, force.y), stunTime, damage);
    }
    void EnemyDamage(GameObject obj, GameObject other, Vector2 launchDir, float stunTime)
    {
        float damage = attackStrength;
        Vector2 force = launchDir;
        Enemy enemy = obj.transform.parent.gameObject.GetComponent<Enemy>();
        Player player = other.transform.parent.gameObject.GetComponent<Player>();
        player.engagedEnemy = enemy;
        enemy.PlayStun(new Vector2(force.x * player.faceDir, force.y), stunTime, damage);
        if(launcher)
            enemy.launched = true;
    }
}
