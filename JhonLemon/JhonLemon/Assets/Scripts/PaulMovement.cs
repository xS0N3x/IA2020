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
    public Text candyDisplay;
    public bool activePaul = false;
    public bool caughtPaul = false;
    public AudioSource paulSound;
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
    }

    // Update is called once per frame
    void Update()
    {
        //Steering Behaviour
        if (candyNumber > 0 && Input.GetKeyDown(KeyCode.Q))
        {
            candyNumber--;
            candyDisplay.text = "Candy: " + candyNumber.ToString();
            activePaul = true;
            audioAlert.AlertSound.Play();
            paulSound.Play();

            
        }
        else if (activePaul == false)
        {
            Vector3 targetPosition = player.transform.position;
            playerDistance = targetPosition - transform.position;
            MovePaul();
            paulSound.Stop();
        }
    }

    void MovePaul()
    {
        if (playerDistance.magnitude >= minDistance)
        {
            Vector3 desiredVelocity = playerDistance.normalized * followSpeed;
            Vector3 steering = desiredVelocity - velocity;

            velocity += steering * Time.deltaTime;
            float slowDownFactor = Mathf.Clamp01(playerDistance.magnitude / slowDistance);
            velocity *= slowDownFactor;

            m_Animator.SetBool("walking", true);
        }
        else
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
