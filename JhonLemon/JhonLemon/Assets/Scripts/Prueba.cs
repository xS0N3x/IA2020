using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prueba : MonoBehaviour
{
    public float distanceToTargetThreshold = 0.5f;

    public Vector3 velocity = Vector3.zero;
    public float turnSpeed = 20;
    public float speed = 2f;
    Quaternion m_Rotation = Quaternion.identity;

    public Waypoint waypoint1, waypoint2, waypoint3, waypoint4;

    public bool paulCaught = false;

    public  bool lost = false;
    public bool moveWaypointToPaul = false;

    private Dictionary<Waypoint, List<Waypoint>> graph;
    private Waypoint currentWaypoint;
    public Waypoint JohnWaypoint;
    public Waypoint PaulWaypoint;
    private List<Waypoint> pathToTarget;
    public float heardDistance;
    private GameObject RinGhost;

    private Rigidbody RinGhostRB;

    private PaulMovement paulScript;

    public GameObject AllWaypoints;

    private Vector3 direction;

    private GameObject JohnLemon;
    public bool waiting = false;

    private float distanceToTarget;
    RaycastHit hit;

    LineRenderer lineRenderer;

    public float distanceToJohn;

    [Range(0f, 360f)]
    public float visionAngle = 30f;

    public float visionDistance;

    public bool detected = false;

    private void Awake()
    {
        paulScript = GameObject.FindObjectOfType<PaulMovement>();
        JohnLemon = GameObject.Find("JohnLemon");
        lineRenderer = GetComponent<LineRenderer>();
    }

    //Pruebitas
    private void OnDrawGizmos()
    {
        if (visionAngle <= 0) return;

        float halfVisionAngle = visionAngle / 2;

        Vector3 p1, p2;

        p1 = PointForAngle(halfVisionAngle, visionDistance);
        p2 = PointForAngle(-halfVisionAngle, visionDistance);

        Gizmos.color = detected ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + p1);
        Gizmos.DrawLine(transform.position, transform.position + p2);

        Gizmos.DrawRay(transform.position, transform.forward * visionDistance);
    }

    Vector3 PointForAngle(float angle, float distance) {
        return transform.TransformDirection( new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad),0, Mathf.Cos(angle * Mathf.Deg2Rad))) * distance;
    }
    //Fin pruebitas 

    // Start is called before the first frame update
    void Start()
    {
        RinGhost = GetComponent<Transform>().gameObject; //provisional

        RinGhostRB = GetComponent<Rigidbody>();

        //ActualizeGraph();
        graph = new Dictionary<Waypoint, List<Waypoint>>();
        Waypoint[] waypoints = GameObject.FindObjectsOfType<Waypoint>();

        // Construct the graph
        foreach (Waypoint w in waypoints)
        {
           

            if (w == PaulWaypoint)
                Debug.Log("Tu puta madre");

            List<Waypoint> edges = new List<Waypoint>();
            if (w.autogenerateEdges)
            {
                // Generate the edges from this node
                foreach (Waypoint other in waypoints)
                {
                    if (w != other && !Physics.Raycast(w.transform.position, other.transform.position - w.transform.position, Vector3.Distance(w.transform.position, other.transform.position)))
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

        //Find nearest waypoint to start with
        float minDistance = Mathf.Infinity;
       // currentWaypoint = waypoints[0];
        foreach (Waypoint w in waypoints)
        {
            float distance = Vector3.Distance(transform.position, w.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                currentWaypoint = w;
            }
        }

        // Select random node as target and build path towards it
        Waypoint target = null;
        target = currentWaypoint;
        pathToTarget = getPath(graph, currentWaypoint, target);

    }

    // Update is called once per frame
    void Update()
    {
        //ActualizeGraph();
        /*Vector3 vectorJhon = JohnLemon.transform.position - transform.position;
        distanceToJohn = vectorJhon.magnitude;

        if (distanceToJohn < heardDistance) {
            if (JohnLemon.GetComponent<AudioSource>().volume > 0.75) {
                Vector3 desiredForward = Vector3.RotateTowards(transform.forward,vectorJhon, turnSpeed, 0f);
                Quaternion rotation = Quaternion.LookRotation(desiredForward);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 20f * Time.deltaTime);
            }
        }*/
        
        if (paulScript.activePaul)//Perseguir al pollo si esta activo
        {
            if (moveWaypointToPaul) {
                PaulWaypoint.transform.position = paulScript.gameObject.transform.position;
                ActualizeGraph();
                moveWaypointToPaul = false;
            }
            
            /*pathToTarget = getPath(graph, currentWaypoint, FindClosestWaypointTo(paulScript.gameObject));
            currentWaypoint = pathToTarget[0];
            pathToTarget.RemoveAt(0);
            direction = currentWaypoint.transform.position - transform.position;*/


        }
        else { //Si el pollo no esta activo

            if (paulCaught) //Si el pollo ha sido detectado
            {
                //StartCoroutine(RetunPatrol());
                paulCaught = false;
                currentWaypoint = waypoint1;
            }
            else { // Caso general

                Vector3 playerVector = JohnLemon.transform.position - transform.position;
                if (Vector3.Angle(playerVector.normalized, transform.forward) < visionAngle / 2)
                {
                    if (playerVector.magnitude < visionDistance)
                    {

                        RaycastHit hit;
                        if (Physics.Raycast(transform.position + new Vector3(0,0.5f,0), JohnLemon.transform.position - transform.position + new Vector3(0, 0.5f, 0), out hit)) {
                            if (hit.collider.gameObject.CompareTag("Player")) {
                                detected = true;
                            }
                            //Debug.Log(hit.collider.gameObject.CompareTag("Player"));
                        }
                        

                        //currentWaypoint = FindClosestWaypointTo(JohnLemon);
                    }
                    else
                    {
                        if (detected) {
                            lost = true;
                        }
                        detected = false;
                    }
                }
                else
                {
                    if (detected)
                    {
                        lost = true;
                    }
                    detected = false;
                }
            }           
        }

        if (detected)
        {
            currentWaypoint = JohnWaypoint;
        }
        else {
            if (lost)
            {
                StartCoroutine(RetunPatrol());
            }
            else {
                Patrol();
            }
           
        }
        if (waiting)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, JohnLemon.transform.position - transform.position, 1f * Time.deltaTime, 0f);
            Quaternion rotation = Quaternion.LookRotation(desiredForward);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 20f * Time.deltaTime);
        }
        else {
            SteeringAndRotating();
        }
        //Patrol();
        
    }

    IEnumerator RetunPatrol() {
        waiting = true;
        currentWaypoint = FindClosestWaypointTo(JohnLemon);
        yield return new WaitForSeconds(2);
        
        waiting = false;
        lost = false;
        //wait = false;
    }

    public Waypoint FindClosestWaypointTo(GameObject target) {

        Waypoint[] waypoints = GameObject.FindObjectsOfType<Waypoint>();

        // Find nearest waypoint to start with
        float minDistance = Mathf.Infinity;
        foreach (Waypoint w in waypoints)
        {
            if (w != JohnWaypoint)
            {
                if (w != PaulWaypoint)
                {
                    float distance = Vector3.Distance(transform.position, w.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        currentWaypoint = w;
                    }
                }
            }
            /*if (lost)
            {
                if (w != JohnWaypoint)
                {
                    if (w != PaulWaypoint)
                    {
                        float distance = Vector3.Distance(transform.position, w.transform.position);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            currentWaypoint = w;
                        }
                    }
                }
            }
            else {
                float distance = Vector3.Distance(target.transform.position, w.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    currentWaypoint = w;
                }
            }*/
        } 

        return currentWaypoint;
    }

    void Patrol() {
        distanceToTarget = Vector3.Distance(transform.position, currentWaypoint.transform.position);

        if (distanceToTarget < distanceToTargetThreshold)
        { // If the waypoint has been reached
          // Find next waypoint
            /*if (wait)
            {

                FindClosestWaypointTo(JohnLemon);
                StartCoroutine(RetunPatrol());
            }
            else {
                FindNextWaypoint();
            }*/
            FindNextWaypoint();

            // Select next waypoint
            currentWaypoint = pathToTarget[0];
            pathToTarget.RemoveAt(0);

        }

        direction = currentWaypoint.transform.position - transform.position;
    }

    void SteeringAndRotating() {
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

    void ActualizeGraph() 
    {
        graph = new Dictionary<Waypoint, List<Waypoint>>();
        GameObject[] ways = GameObject.FindGameObjectsWithTag("Waypoint"); // ineficinete by MMM
        Waypoint[] waypoints = new Waypoint[ways.Length];
       
        int i = 0;
        foreach (GameObject g in ways) {
            waypoints[i] = g.GetComponent<Waypoint>();
            i++;
        }

        // Construct the graph
        foreach (Waypoint w in waypoints)
        {
            if (w == PaulWaypoint)
            Debug.Log("Tu puta madre");

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

    void FindNextWaypoint() {
        if (pathToTarget.Count == 0)
        {
            int dice;
            // Reached target
            Waypoint target = null;
            while (!target || target == currentWaypoint) // Choose a new target, different from the current waypoint
            {
                
                if (paulScript.activePaul)
                {
                    dice = 5;
                }
                else {
                    dice = Random.Range(0, 4);
                }
                
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
                    case 5:
                        target = PaulWaypoint;
                        break;
                    default:
                        Debug.Log("Wrong Waypoint");
                        break;
                }
            }

            pathToTarget = getPath(graph, currentWaypoint, target);
        }
    }
}
