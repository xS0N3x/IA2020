using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    public Transform player;
    public GameEnding gameEnding;

    private SkinnedMeshRenderer light;
    private Color lightColorCaught;
    private Color lightColorInitial;
    private Color lightColorChange;

    bool m_IsPlayerInRange;

    private void Start()
    {
        light = transform.parent.gameObject.transform.GetComponentInChildren<SkinnedMeshRenderer>();
        lightColorCaught = Color.red;
        lightColorInitial = Color.green;
        lightColorChange = lightColorInitial;
        light.materials[0].SetColor("_EmissionColor", lightColorInitial);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
        {
            m_IsPlayerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == player)
        {
            m_IsPlayerInRange = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (lightColorChange.r >= 0.95)
        {
            gameEnding.CaughtPlayer();
        }

        if (m_IsPlayerInRange)
        {
            Vector3 direction = player.position - transform.position + Vector3.up;
            Ray ray = new Ray(transform.position, direction);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit))
            {
                if (raycastHit.collider.transform == player)
                {
                    lightColorChange = Color.Lerp(lightColorChange, lightColorCaught, Time.deltaTime * 1.3f);
                    light.materials[0].SetColor("_EmissionColor", lightColorChange);
                }
            }
        }
        else
        {
            lightColorChange = Color.Lerp(lightColorChange, lightColorInitial, Time.deltaTime * 0.5f);
            light.materials[0].SetColor("_EmissionColor", lightColorChange);
        }
    }
}