using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyInput : MonoBehaviour
{
    NavMeshAgent nav;
    Enemy parent;

    public float desiredDistance = 1;
    float distance;
    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        parent = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!parent.stunned)
        {
            distance = (float)System.Math.Round(Vector3.Distance(parent.player.transform.position, parent.transform.position), 2);
            if(parent.playerDetected && distance > desiredDistance)
            {
                parent.SetInput(parent.player.transform.position - parent.transform.position);
                parent.Attack(0);
            }
            else if(parent.playerDetected && distance <= desiredDistance)
            {
                parent.SetInput(Vector2.zero);
                parent.InstantStop();
                parent.Attack(0.4f);        
            }
            else
            {
                parent.SetInput(Vector2.zero);
                parent.Attack(0);
            }
        }
        else
        {
            parent.SetInput(Vector2.zero);
        }
        
    }
}
