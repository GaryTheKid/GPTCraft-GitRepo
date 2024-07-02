using UnityEngine;

public class PlayerViewController : MonoBehaviour
{
    public Transform headTransform; // 这里将Head物体的Transform引用拖放到Inspector中

    private PlayerStats stats;

    private float rotationX = 0.0f;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (stats.STATE_invPageState != InventoryPageState.None) return;

        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 计算旋转角度
        rotationX -= mouseY * stats.VIEW_sensitivity;
        rotationX = Mathf.Clamp(rotationX, -90.0f, 90.0f); // 限制视角的上下旋转

        // 旋转Head物体
        headTransform.localRotation = Quaternion.Euler(rotationX, 0.0f, 0.0f);
        transform.Rotate(Vector3.up * mouseX * stats.VIEW_sensitivity);
    }
}
