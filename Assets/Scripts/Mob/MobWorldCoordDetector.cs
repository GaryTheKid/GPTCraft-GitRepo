using UnityEngine;

public class MobWorldCoordDetector : MonoBehaviour
{
    public MobWorldController mobClass;

    void Update()
    {
        // �����������Ϊ��ǰλ��
        Vector3 rayOrigin = transform.position;

        // �������߷���Ϊ���·�
        Vector3 rayDirection = Vector3.down;

        // �������߳���Ϊ���޴�
        float rayLength = Mathf.Infinity;

        // ִ�����߼��
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength, LayerMask.GetMask("Default")))
        {
            // ������߻����˵������
            // �����е��λ������Ϊ aiClass ����������
            mobClass.UpdateWorldCoord(hit.point);
        }
    }
}
