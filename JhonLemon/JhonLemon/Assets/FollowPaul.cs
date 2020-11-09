using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPaul : MonoBehaviour
{
    GameObject Paul;

    // Start is called before the first frame update
    void Start()
    {
        Paul = GameObject.Find("PaulMcChickenMario");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Paul.transform.position;
    }
}
