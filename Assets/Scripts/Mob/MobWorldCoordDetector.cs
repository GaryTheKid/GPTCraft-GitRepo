using UnityEngine;

public class MobWorldCoordDetector : MonoBehaviour
{
    public MobWorldController mobClass;

    void Update()
    {
        // 定义射线起点为当前位置
        Vector3 rayOrigin = transform.position;

        // 定义射线方向为正下方
        Vector3 rayDirection = Vector3.down;

        // 定义射线长度为无限大
        float rayLength = Mathf.Infinity;

        // 执行射线检测
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength, LayerMask.GetMask("Default")))
        {
            // 如果射线击中了地面格子
            // 将击中点的位置设置为 aiClass 的世界坐标
            mobClass.UpdateWorldCoord(hit.point);
        }
    }
}
