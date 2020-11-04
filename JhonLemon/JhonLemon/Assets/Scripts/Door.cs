using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Key keyState;

    private void Awake()
    {
        keyState = GameObject.FindObjectOfType<Key>();
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && keyState.pickedKey == true)
        {
            Destroy(gameObject);
        }
    }
}
