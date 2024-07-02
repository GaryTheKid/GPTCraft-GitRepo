using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public PlayerWeaponController weaponController; // ����WeaponController�ű�

    // �����˽��봥������Χʱ������
    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.GetComponent<MobWorldController>();
        if (enemy != null)
        {
            // ����WeaponController�еķ�����������ӵ��б���
            weaponController.AddEnemy(enemy);
        }
    }

    // �������뿪��������Χʱ������
    private void OnTriggerExit(Collider other)
    {
        var enemy = other.GetComponent<MobWorldController>();
        if (enemy != null)
        {
            // ����WeaponController�еķ��������˴��б����Ƴ�
            weaponController.RemoveEnemy(enemy);
        }
    }
}
