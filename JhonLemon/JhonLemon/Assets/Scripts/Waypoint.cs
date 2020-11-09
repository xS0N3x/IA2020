using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{

    public Waypoint[] adjacents;

    public bool autogenerateEdges = false;

    private void Awake()
    {
        
    }
}
