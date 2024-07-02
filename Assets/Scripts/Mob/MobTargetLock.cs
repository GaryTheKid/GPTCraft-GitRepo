using UnityEngine;

public class MobTargetLock : MonoBehaviour
{
    public MobWorldController mobClass;

    private void OnTriggerEnter(Collider other)
    {
        mobClass.chaseTarget = other.transform;
    }

    private void OnTriggerExit(Collider other)
    {
        mobClass.chaseTarget = null;
    }
}
