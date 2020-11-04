using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Candy : MonoBehaviour
{
    private PaulMovement candyNum;

    private void Awake()
    {
        candyNum = GameObject.FindObjectOfType<PaulMovement>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            candyNum.candyNumber++;
            candyNum.candyDisplay.text = "Candy: " + candyNum.candyNumber.ToString();
            Destroy(gameObject);
        }
    }
}
