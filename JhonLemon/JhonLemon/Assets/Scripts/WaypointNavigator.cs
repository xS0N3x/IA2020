using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNavigator : MonoBehaviour
{
    public float distanceToTargetThreshold = 0.5f;
    public float turnSpeed = 20;
    public float speed = 2f;
    public Vector3 velocity = Vector3.zero;
    public GameObject AllWaypoints;
    public Waypoint waypoint1, waypoint2, waypoint3, waypoint4;
    public Waypoint currentWaypoint;
    Waypoint target;
    public List<Waypoint> pathToTarget;

    private Dictionary<Waypoint, List<Waypoint>> graph;
    private PaulMovement paulCaught;

    Quaternion m_Rotation = Quaternion.identity;
    Waypoint[] waypoints;

    private void Awake()
    {
        paulCaught = GameObject.FindObjectOfType<PaulMovement>();
    }

    // Start is called before the first frame update
    void Start()
    {

        ConstructGraph();

        FindNearestWaypoint();
        
        // Select nearest node as target and build path towards it
        Waypoint target = null;
        target = currentWaypoint;
        pathToTarget = getPath(graph, currentWaypoint, target);

    }

    void ConstructGraph() {
        graph = new Dictionary<Waypoint, List<Waypoint>>();
        waypoints = GameObject.FindObjectsOfType<Waypoint>();

        // Construct the graph
        foreach (Waypoint w in waypoints)
        {
            List<Waypoint> edges = new List<Waypoint>();
            if (w.autogenerateEdges)
            {
                // Generate the edges from this node
                foreach (Waypoint other in waypoints)
                {
                    if (w != other && !Physics.Raycast(w.transform.position,
                            other.transform.position - w.transform.position,
                            Vector3.Distance(w.transform.position, other.transform.position)))
                    {
                        edges.Add(other);
                    }
                }
            }
            else
            {
                foreach (Waypoint e in w.adjacents)
                {
                    edges.Add(e);
                }
            }
            graph.Add(w, edges);
        }
    }

    void FindNearestWaypoint() {
        // Find nearest waypoint to start with
        float minDistance = Mathf.Infinity;
        foreach (Waypoint w in waypoints)
        {
            float distance = Vector3.Distance(transform.position, w.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                currentWaypoint = w;
            }
        }
    }

    public void ComeBack() //When target reached pathes back using the waypoints
    {

        FindNearestWaypoint();
        pathToTarget = getPath(graph, currentWaypoint, currentWaypoint);

    }

    // Update is called once per frame
    void Update()
    {

        if (paulCaught.caughtPaul == true) //If Paul had been defused
        {
            ComeBack();
            paulCaught.caughtPaul = false;
            
        }
        

        float distanceToTarget = Vector3.Distance(transform.position, currentWaypoint.transform.position); //Distance to next waypoint

        if (distanceToTarget < distanceToTargetThreshold)
        { // If the waypoint has been reached
          // Find next waypoint
            if (pathToTarget.Count == 0) //If path completed finds next path between asigned waypoints
            {
                // Reached target
                target = null;
                while (!target || target == currentWaypoint) // Choose a new target, different from the current waypoint
                {
                    FindDiferentTarget();
                }
         
                pathToTarget = getPath(graph, currentWaypoint, target);
            }

            // Select next waypoint
            currentWaypoint = pathToTarget[0];
            pathToTarget.RemoveAt(0);

        }

        MoveGhost();
    }

    void MoveGhost() {
        Vector3 direction = currentWaypoint.transform.position - transform.position;

        //Steering Behaviour
        Vector3 desiredVelocity = direction.normalized * speed;
        Vector3 steering = desiredVelocity - velocity;
        velocity += steering * Time.deltaTime;

        transform.position += velocity.normalized * Time.deltaTime;

        //Rotation
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, velocity, turnSpeed * Time.deltaTime, 0f);
        Quaternion rotation = Quaternion.LookRotation(desiredForward);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 20f * Time.deltaTime);
    }

    void FindDiferentTarget() {
        int dice = Random.Range(0, 4);
        switch (dice)
        {
            case 0:
                target = waypoint1;
                break;
            case 1:
                target = waypoint2;
                break;
            case 2:
                target = waypoint3;
                break;
            case 3:
                target = waypoint4;
                break;
            default:
                Debug.Log("Wrong Waypoint");
                break;
        }
    }
    List<Waypoint> getPath(Dictionary<Waypoint, List<Waypoint>> graph, Waypoint start, Waypoint goal)
    {
        SortedList<float, Waypoint> frontier = new SortedList<float, Waypoint>();
        Dictionary<Waypoint, Waypoint> visitedFrom = new Dictionary<Waypoint, Waypoint>();
        Dictionary<Waypoint, float> g = new Dictionary<Waypoint, float>(); // costsFromStart

        visitedFrom.Add(start, null);
        g.Add(start, 0);
        frontier.Add(Vector3.Distance(start.transform.position, goal.transform.position), start);

        while (frontier.Count > 0)
        {
            Waypoint current = frontier.Values[0];
            frontier.RemoveAt(0);

            if (current == goal)
                break;

            foreach (Waypoint next in graph[current])
            {
                float newG = g[current] + Vector3.Distance(next.transform.position, current.transform.position);
                if (!g.ContainsKey(next) || newG < g[next])
                {
                    if (frontier.ContainsValue(next))
                    {
                        frontier.RemoveAt(frontier.IndexOfValue(next));
                    }
                    frontier.Add(newG + Vector3.Distance(next.transform.position, goal.transform.position), next);

                    if (visitedFrom.ContainsKey(next))
                    {
                        visitedFrom.Remove(next);
                    }
                    visitedFrom.Add(next, current);

                    if (g.ContainsKey(next))
                        g.Remove(next);
                    g.Add(next, newG);
                }
            }
        }

        // Return the path to the goal
        List<Waypoint> path = new List<Waypoint>();
        Waypoint w = goal;
        path.Add(goal);
        while (visitedFrom[w] != null)
        {
            path.Add(visitedFrom[w]);
            w = visitedFrom[w];
        }
        path.Reverse();
        return path;
    } 

}
