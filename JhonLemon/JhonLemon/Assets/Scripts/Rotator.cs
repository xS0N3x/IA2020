using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    Rigidbody m_Rigidbody;
    public bool facingRight = false; 
    public Transform Posleft;
    public Transform Posright;
    Quaternion m_Rotation = Quaternion.identity;
    public float turnSpeed = 10f;
    private static Transform myTransform;

    Vector3 leftDirection;
    Vector3 rightDirection;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        leftDirection = Posleft.position - transform.position;
        rightDirection = Posright.position - transform.position;
        StartCoroutine(Turn());
    }

    // Update is called once per frame
    void Update()
    {

        if (!facingRight)
        {
            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, leftDirection.normalized, turnSpeed * Time.deltaTime, 0f);
            m_Rotation = Quaternion.LookRotation(desiredForward);
        }
        else
        {  
            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, rightDirection.normalized, turnSpeed * Time.deltaTime, 0f);
            m_Rotation = Quaternion.LookRotation(desiredForward);
        }
        

    }

    private void OnAnimatorMove()
    {
        m_Rigidbody.MoveRotation(m_Rotation);
    }

    IEnumerator Turn() {

        facingRight = !facingRight;

        yield return new WaitForSeconds(4);

        StartCoroutine(Turn());

    }
}
