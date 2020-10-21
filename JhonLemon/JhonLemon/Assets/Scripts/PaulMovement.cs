using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaulMovement : MonoBehaviour
{

    public float followSpeed = 15f;
    public float slowDistance = 1f;
    public float turnSpeed = 20;
    public float minDistance = 1;

    Quaternion m_Rotation = Quaternion.identity;

    Rigidbody m_Rigidbody;

    Animator m_Animator;

    Vector3 velocity = Vector3.zero;

    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("JohnLemon");
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Steering Behaviour

        Vector3 targetPosition = player.transform.position;
        Vector3 playerDistance = targetPosition - transform.position;

        if (playerDistance.magnitude >= minDistance)
        {
            Vector3 desiredVelocity = playerDistance.normalized * followSpeed;
            Vector3 steering = desiredVelocity - velocity;

            velocity += steering * Time.deltaTime;
            float slowDownFactor = Mathf.Clamp01(playerDistance.magnitude / slowDistance);
            velocity *= slowDownFactor;

            m_Animator.SetBool("walking", true);
        }
        else {
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
