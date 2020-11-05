using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartGhost : MonoBehaviour
{
    public Transform player;
    public Transform Paul;
    public GameEnding gameEnding;
    //public Rigidbody rigidBody;
    public float speed = 4f;

    bool m_IsPlayerInRange;
    bool m_IsPaulInRange;

    private PaulMovement paulActive;

    private void Awake()
    {
        paulActive = GameObject.FindObjectOfType<PaulMovement>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.transform == player)
        {
            m_IsPlayerInRange = true;
        }
        else if (other.transform == Paul)
        {
            m_IsPaulInRange = true;
            paulActive.activePaul = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == player)
        {
            m_IsPlayerInRange = false;
        }
        else if (other.transform == Paul)
        {
            m_IsPaulInRange = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsPlayerInRange)
        {
            Vector3 direction = player.position - transform.position + Vector3.up;
            Ray ray = new Ray(transform.position, direction);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit))
            {
                if (raycastHit.collider.transform == player)
                {
                    //gameEnding.CaughtPlayer();
                    this.transform.position = raycastHit.point;
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
                if (raycastHit.collider.transform == Paul && paulActive.activePaul == true)
                {
                    //gameEnding.CaughtPlayer();
                    this.transform.position = raycastHit.point;
                    paulActive.activePaul = false;
                    paulActive.caughtPaul = true;
                }
            }
        }
    }
}
