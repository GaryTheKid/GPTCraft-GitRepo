using UnityEngine;

public class MobRayHillDetector : MonoBehaviour
{
    public MobWorldController mobClass;

    void Update()
    {
        // �����������Ϊ��ǰλ��
        Vector3 rayOrigin = transform.position;

        // �������߷���Ϊ��ǰ��
        Vector3 rayDirection = transform.forward;

        // �������߳���Ϊ1����λ
        float rayLength = 1f;

        // ִ�����߼��
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength, LayerMask.GetMask("Default")))
        {
            // ������е���ɽ���� aiClass �е� hasHillAtFront ����Ϊ true
            mobClass.hasHillAtFront = true;
        }
        else
        {
            // �������δ�����κ����壬�� aiClass �е� hasHillAtFront ����Ϊ false
            mobClass.hasHillAtFront = false;
        }
    }
}

