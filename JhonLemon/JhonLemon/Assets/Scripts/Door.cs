using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Key keyState;

    public GameObject door;

    public AudioSource doorSound;

    private void Start()
    {
        door.SetActive(false);
        doorSound = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        keyState = GameObject.FindObjectOfType<Key>();
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && keyState.pickedKey == true)
        {
            Debug.Log("Abrete sesamo");
            doorSound.Play();
            gameObject.SetActive(false);
            door.SetActive(true);
        }
    }
}
