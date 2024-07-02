using UnityEngine;

public class PlayerViewController : MonoBehaviour
{
    public Transform headTransform; // ���ｫHead�����Transform�����Ϸŵ�Inspector��

    private PlayerStats stats;

    private float rotationX = 0.0f;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (stats.STATE_invPageState != InventoryPageState.None) return;

        // ��ȡ�������
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // ������ת�Ƕ�
        rotationX -= mouseY * stats.VIEW_sensitivity;
        rotationX = Mathf.Clamp(rotationX, -90.0f, 90.0f); // �����ӽǵ�������ת

        // ��תHead����
        headTransform.localRotation = Quaternion.Euler(rotationX, 0.0f, 0.0f);
        transform.Rotate(Vector3.up * mouseX * stats.VIEW_sensitivity);
    }
}
