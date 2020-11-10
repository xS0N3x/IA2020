using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmartGhost : MonoBehaviour { //Manages the Ghost State between Patrol and Pathfind

    public bool m_IsPlayerInRange;
    public float speed = 4f;
    public Transform player;
    public Transform Paul;
    public GameEnding gameEnding;
    public WaypointNavigator Patrol;
    public Pathfinding Pathfinder;
    public AudioSource AlertSound;
   
    bool m_IsPaulInRange;
    private float patrolTime = 0f;
    private float seconds = 1f;
    private PaulMovement scriptPaul;

    private void Start() //Starts Patrol behaviour
    {
        Patrol.enabled = true;
        Pathfinder.enabled = false;
        AlertSound = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        scriptPaul = GameObject.FindObjectOfType<PaulMovement>();
    }

    private void OnTriggerEnter(Collider other) //FieldOfView
    {
        if (other.transform == player) //If player is detected changes from Patrol to Pathfind
        {
            m_IsPlayerInRange = true;

            if (Patrol.enabled)
            {
                AlertSound.Play();
            }

            FromPatrolToPathFind();

            patrolTime = 0; //resets patrol timer
        }
        else if (other.transform == Paul && scriptPaul.activePaul == true) //If Paul is detected and is active, it is defused and return to Patrol behaviour
        {
            m_IsPaulInRange = true;
            DefusePaul();
            FromPathfindToPatrol();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsPlayerInRange) //If player is in range
        {
            patrolTime += seconds * Time.deltaTime;

            if (patrolTime > 4) //If player is undetected for more than 4 secs
            {
                m_IsPlayerInRange = false;
                FromPathfindToPatrol();
                
            }
        }
        if (scriptPaul.caughtPaul) { //If Paul had been defused
            FromPathfindToPatrol();
        }
    }

    void FromPatrolToPathFind() {
        Patrol.enabled = false;
        Pathfinder.enabled = true;
    }

    void FromPathfindToPatrol() {
        Pathfinder.enabled = false;
        Patrol.enabled = true;
    }
    void DefusePaul()
    {
        scriptPaul.activePaul = false;
        scriptPaul.caughtPaul = true;
    }
}
