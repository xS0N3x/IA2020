using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public bool pickedKey;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            pickedKey = true;
            Destroy(gameObject);
        }
        else if (collider.gameObject.tag == "Player" && pickedKey == true)
        {
            Destroy(gameObject);
        }
    }
}
