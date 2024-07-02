using System.Collections;
using UnityEngine;

public class ItemWorldObject : MonoBehaviour
{
    public ItemData itemData;
    public int amount;

    public bool isPickable;

    private void OnEnable()
    {
        WorldObjectSpawner.singleton.activeSpawnedItems.Add(gameObject);
    }

    private void OnDisable()
    {
        WorldObjectSpawner.singleton.activeSpawnedItems.Remove(gameObject);
    }

    private void OnDestroy()
    {
        WorldObjectSpawner.singleton.spawnedItems.Remove(gameObject);
    }

    public void SetDestroyTimeCoroutine(float pickableTime, float destroyTime)
    {
        StartCoroutine(DestroyAfterDelay(pickableTime, destroyTime));
    }

    private IEnumerator DestroyAfterDelay(float pickableTime, float destroyTime)
    {
        yield return new WaitForSeconds(pickableTime);
        isPickable = true;

        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject); // 延迟销毁物品对象
    }

}