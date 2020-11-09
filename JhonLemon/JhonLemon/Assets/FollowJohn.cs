using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowJohn : MonoBehaviour
{
    GameObject John;

    // Start is called before the first frame update
    void Start()
    {
        John = GameObject.Find("JohnLemon");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = John.transform.position;
    }
}
