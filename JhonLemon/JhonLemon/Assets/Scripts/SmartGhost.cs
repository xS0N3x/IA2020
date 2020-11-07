using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmartGhost : MonoBehaviour
{
    public Transform player;
    public Transform Paul;
    public GameEnding gameEnding;
    //public Rigidbody rigidBody;
    public float speed = 4f;
    public WaypointNavigator Patrol;
    public Pathfinding Pathfinder;
    public AudioSource AlertSound;

    bool m_IsPlayerInRange;
    bool m_IsPaulInRange;

    private float patrolTime = 0f;
    private float seconds = 1f;

    private PaulMovement paulActive;

    private void Start()
    {
        Patrol.enabled = true;
        Pathfinder.enabled = false;
        AlertSound = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        paulActive = GameObject.FindObjectOfType<PaulMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
        {
            m_IsPlayerInRange = true;
            Patrol.enabled = false;
            Pathfinder.enabled = true;
            AlertSound.Play();
            patrolTime = 0;
        }
        else if (other.transform == Paul && paulActive.activePaul == true)
        {
            m_IsPaulInRange = true;
            paulActive.activePaul = false;
            paulActive.caughtPaul = true;
            Patrol.enabled = true;
            Pathfinder.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsPlayerInRange)
        {
            patrolTime += seconds * Time.deltaTime;
            if (patrolTime > 4)
            {
                m_IsPlayerInRange = false;
                Patrol.enabled = true;
                Pathfinder.enabled = false;
            }
        }
        if (paulActive.activePaul == true)
        {
            Patrol.enabled = false;
            Pathfinder.enabled = true;
        }
        /*
        if (m_IsPlayerInRange)
        {
            Patrol.enabled = false;
            Pathfinder.enabled = true;
            AlertSound.Play();
            //patrolTime += seconds * Time.deltaTime;

            Vector3 direction = player.position - transform.position + Vector3.up;
            Ray ray = new Ray(transform.position, direction);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit))
            {
                if (raycastHit.collider.transform == player)
                {
                    //gameEnding.CaughtPlayer();
                    //this.transform.position = raycastHit.point;
                }
            }
        }
        else if (m_IsPaulInRange)
        {
            Vector3 direction = Paul.position - transform.position + Vector3.up;
            Ray ray = new Ray(transform.position, direction);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit))
            {
                if (raycastHit.collider.transform == Paul)
                {
                    this.transform.position = raycastHit.point;
                    paulActive.activePaul = false;
                    paulActive.caughtPaul = true;
                    Patrol.enabled = true;
                    Pathfinder.enabled = false;
                }
            }
        }*/
    }
}
