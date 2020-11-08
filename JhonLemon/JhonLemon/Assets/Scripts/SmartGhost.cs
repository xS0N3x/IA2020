using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmartGhost : MonoBehaviour
{
    public Transform player;
    public Transform Paul;
    public GameEnding gameEnding;
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
    }
}
