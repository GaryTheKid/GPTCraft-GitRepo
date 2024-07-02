using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public PlayerWeaponController weaponController; // 引用WeaponController脚本

    // 当敌人进入触发器范围时被调用
    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.GetComponent<MobWorldController>();
        if (enemy != null)
        {
            // 调用WeaponController中的方法将敌人添加到列表中
            weaponController.AddEnemy(enemy);
        }
    }

    // 当敌人离开触发器范围时被调用
    private void OnTriggerExit(Collider other)
    {
        var enemy = other.GetComponent<MobWorldController>();
        if (enemy != null)
        {
            // 调用WeaponController中的方法将敌人从列表中移除
            weaponController.RemoveEnemy(enemy);
        }
    }
}
