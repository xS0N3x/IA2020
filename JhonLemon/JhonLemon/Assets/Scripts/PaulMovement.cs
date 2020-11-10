using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaulMovement : MonoBehaviour
{

    public float followSpeed = 15f;
    public float slowDistance = 1f;
    public float turnSpeed = 20;
    public float minDistance = 1;
    public int candyNumber = 0;
    public bool activePaul = false;
    public bool caughtPaul = false;
    public Text candyDisplay;
    public AudioSource paulSound;
    public SmartGhost script;
    public Waypoint[] waypoints;
    public GameObject ghost1;
    public GameObject ghost2;
    public GameObject ghost3;
    public GameObject nearestGhost;
    public Waypoint nearestWaypoint;

    private SmartGhost audioAlert;

    Quaternion m_Rotation = Quaternion.identity;
    Rigidbody m_Rigidbody;
    Animator m_Animator;
    Vector3 velocity = Vector3.zero;
    Vector3 playerDistance;
    GameObject player;

    private void Awake()
    {
        audioAlert = GameObject.FindObjectOfType<SmartGhost>();

    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("JohnLemon");
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();
        paulSound = GetComponent<AudioSource>();
        waypoints = GameObject.FindObjectsOfType<Waypoint>();

    }

    // Update is called once per frame
    void Update()
    {
        //Steering Behaviour
        if (candyNumber > 0 && Input.GetKeyDown(KeyCode.Q)) //If Q pressed actives the bait
        {
            FindNearestWaypoint();

            ActualizeCandyUI();

            activePaul = true;

            BaitNearestGhost();

            //Audio facts
            audioAlert.AlertSound.Play();
            paulSound.Play();

        }
        else if (activePaul == false) //When paul isn't active
        {
            FindNearestGhosts(); //Find the nearests ghost for calling then if active

            Vector3 targetPosition = player.transform.position;
            playerDistance = targetPosition - transform.position;

            MovePaul(); //Moves towards John

            paulSound.Stop(); //Stops Sounds
        }
    }

    void BaitNearestGhost() {
        script.Patrol.currentWaypoint = nearestWaypoint;
        script.Patrol.pathToTarget.Clear();
        script.Patrol.enabled = false;
        script.Pathfinder.enabled = true;
    }

    void ActualizeCandyUI()
    {
        candyNumber--;
        candyDisplay.text = "Candy: " + candyNumber.ToString();
    }
    void FindNearestWaypoint() {
        //Find nearest Waypoint
        float minDistance = Mathf.Infinity;
        foreach (Waypoint w in waypoints)
        {
            float distance = Vector3.Distance(transform.position, w.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestWaypoint = w;
            }
        }
    }

    void FindNearestGhosts() {
        float minDistance = Mathf.Infinity;
        float distanceToGhost;
        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                distanceToGhost = (ghost1.transform.position - transform.position).magnitude;
            }
            else if (i == 1)
            {
                distanceToGhost = (ghost2.transform.position - transform.position).magnitude;
            }
            else
            {
                distanceToGhost = (ghost3.transform.position - transform.position).magnitude;
            }
            if (distanceToGhost < minDistance)
            {
                minDistance = distanceToGhost;
                nearestGhost = ghost1;
            }

        }
        script = nearestGhost.transform.Find("PointOfView").GetComponent<SmartGhost>();
    }

    void MovePaul()
    {
        if (playerDistance.magnitude >= minDistance) //While player is to far 
        {
            Vector3 desiredVelocity = playerDistance.normalized * followSpeed;
            Vector3 steering = desiredVelocity - velocity;

            velocity += steering * Time.deltaTime;
            float slowDownFactor = Mathf.Clamp01(playerDistance.magnitude / slowDistance);
            velocity *= slowDownFactor;

            m_Animator.SetBool("walking", true);
        }
        else //When player is near
        {
            velocity = Vector3.zero;
            m_Animator.SetBool("walking", false);
        }

        transform.position += velocity * Time.deltaTime;

        //Rotation
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, velocity, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);
        m_Rigidbody.MoveRotation(m_Rotation);
    }
}
