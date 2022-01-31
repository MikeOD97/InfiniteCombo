using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyInput : MonoBehaviour
{
    NavMeshAgent nav;
    Enemy parent;
    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        parent = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if(parent.playerDetected)
        {
            parent.SetInput(parent.player.transform.position - parent.transform.position);
        }
        else
        {
            parent.SetInput(Vector2.zero);
        }
    }
}
