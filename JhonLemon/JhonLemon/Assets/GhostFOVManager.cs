using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFOVManager : MonoBehaviour {

    PaulMovement paulScript;
    Prueba ghostScript;
    public GameEnding gameEnding;
    
    // Start is called before the first frame update
    void Start()
    {
        paulScript = GameObject.Find("PaulMcChickenMario").GetComponent<PaulMovement>();
        ghostScript = gameObject.GetComponentInParent<Prueba>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Paul")) {
            if (paulScript.activePaul) {
                paulScript.activePaul = false;
                ghostScript.paulCaught = true;
            }
        } else if (other.CompareTag("Player")){
            //gameEnding.CaughtPlayer();
        }
    }
}
