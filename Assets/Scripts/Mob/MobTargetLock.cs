using UnityEngine;

public class MobTargetLock : MonoBehaviour
{
    public MobWorldController mobController;

    private void OnTriggerEnter(Collider other)
    {
        if(mobController.mobClass is IAggressiveMob) 
            mobController.chaseTarget = other.transform;
    }

    private void OnTriggerExit(Collider other)
    {
        if (mobController.mobClass is IAggressiveMob)
            mobController.chaseTarget = null;
    }
}
