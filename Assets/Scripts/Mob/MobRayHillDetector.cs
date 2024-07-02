using UnityEngine;

public class MobRayHillDetector : MonoBehaviour
{
    public MobWorldController mobClass;

    void Update()
    {
        // 定义射线起点为当前位置
        Vector3 rayOrigin = transform.position;

        // 定义射线方向为正前方
        Vector3 rayDirection = transform.forward;

        // 定义射线长度为1个单位
        float rayLength = 1f;

        // 执行射线检测
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength, LayerMask.GetMask("Default")))
        {
            // 如果击中的是山丘，则将 aiClass 中的 hasHillAtFront 设置为 true
            mobClass.hasHillAtFront = true;
        }
        else
        {
            // 如果射线未击中任何物体，则将 aiClass 中的 hasHillAtFront 设置为 false
            mobClass.hasHillAtFront = false;
        }
    }
}

