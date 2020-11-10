using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Candy : MonoBehaviour
{
    private PaulMovement paulScript;

    private void Awake()
    {
        paulScript = GameObject.FindObjectOfType<PaulMovement>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            paulScript.candyNumber++;
            paulScript.candyDisplay.text = "Candy: " + paulScript.candyNumber.ToString();
            Destroy(gameObject);
        }
    }
}
