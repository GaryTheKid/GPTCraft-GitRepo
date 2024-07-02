using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionDetector : MonoBehaviour
{
    public PlayerInteractionController interactionController;

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            interactionController.canConstruct = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null)
        {
            interactionController.canConstruct = true;
        }
    }
}
