using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float turnSpeed = 20;
    public float velocidadHorizontal = 0f;
    public float velocidadVertical = 0f;
    public float velocidadAnimacion = 0f;
    public float incremento = 0.3f;
    public float vel_max = 2f;
    public float vel_base = 0.5f;

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;
    AudioSource m_AudioSource;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AcceleratingMove(); //Gets de Inputs and increase the velocity of movement while pressed

        m_AudioSource.volume = velocidadAnimacion * 0.5f; //Increase noise as velocity increase

        Footsteps();

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f); //Look towards direction
        m_Rotation = Quaternion.LookRotation(desiredForward);
    }

    void OnAnimatorMove()
    {
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);
        m_Rigidbody.MoveRotation(m_Rotation);
    }

    void AcceleratingMove() { 
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal > 0)
        {
            if (velocidadHorizontal > 0)
            {
                velocidadHorizontal += incremento * Time.deltaTime;
                if (velocidadHorizontal > vel_max)
                    velocidadHorizontal = vel_max;
            }
            else
            {
                velocidadHorizontal += vel_base;
            }
        }
        else if (horizontal < 0)
        {
            if (velocidadHorizontal < 0)
            {
                velocidadHorizontal -= incremento * Time.deltaTime;
                if (velocidadHorizontal < (vel_max * -1))
                    velocidadHorizontal = (vel_max * -1);
            }
            else
            {
                velocidadHorizontal -= vel_base;
            }
        }
        else
        {

            velocidadHorizontal = 0;
        }
        if (vertical > 0)
        {
            if (velocidadVertical > 0)
            {
                velocidadVertical += incremento * Time.deltaTime;
                if (velocidadVertical > vel_max)
                    velocidadVertical = vel_max;
            }
            else
            {
                velocidadVertical += vel_base;
            }
        }
        else if (vertical < 0)
        {
            if (velocidadVertical < 0)
            {
                velocidadVertical -= incremento * Time.deltaTime;
                if (velocidadVertical < (vel_max * -1))
                    velocidadVertical = (vel_max * -1);
            }
            else
            {
                velocidadVertical -= vel_base;
            }
        }
        else
        {
            velocidadVertical = 0;
        }

        velocidadAnimacion = Mathf.Max(Mathf.Abs(velocidadHorizontal), Mathf.Abs(velocidadVertical));

        m_Movement.Set(velocidadHorizontal, 0f, velocidadVertical);
        m_Movement.Normalize();

        m_Animator.SetFloat("multiplier", velocidadAnimacion);
    }

    void Footsteps() {
        bool hasHorizontalInput = !Mathf.Approximately(velocidadHorizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(velocidadVertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool("IsWalking", isWalking);
        if (isWalking)
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else
        {
            m_AudioSource.Stop();
        }
    }
}
