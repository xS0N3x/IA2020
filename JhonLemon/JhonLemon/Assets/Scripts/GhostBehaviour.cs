using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBehaviour : MonoBehaviour
{
    public WaypointNavigator Patrol;
    public Pathfinding Pathfinding;

    // Start is called before the first frame update
    void Start()
    {
        Patrol.enabled = false;
        Pathfinding.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
