using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public float gravity = -5f;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Player p in FindObjectsOfType(typeof (Player)))
        {
            p.Velocity += gravity * new Vector3(0, 1) * Time.deltaTime;
        }
    }
}
