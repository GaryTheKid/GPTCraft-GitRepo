using UnityEngine;

public class LightManager : MonoBehaviour
{
    public static LightManager singleton;

    public Gradient lightColorGradient;
    public Gradient ambientColorGradient;
    public Gradient fogColorGradient;
    public AnimationCurve lightIntensityCurve;

    private Light directionalLight;

    // 引用需要变化的天空盒（如果需要的话）
    // public Material skyboxMaterial;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }

        directionalLight = RenderSettings.sun;
    }

    // 根据当前时间重新评估各个色彩值和太阳光的旋转、强度
    public void EvaluateGlobalLight(float currentTime)
    {
        // 将输入的时间除以24，得到对应的渐变值
        float evaluatedTime = currentTime / 24f;

        // 重新评估昼夜光照颜色、旋转和强度
        if (directionalLight != null)
        {
            // 重新评估昼夜光照颜色
            directionalLight.color = lightColorGradient.Evaluate(evaluatedTime);

            // 计算太阳光的旋转角度
            float rotationAngle = evaluatedTime * 360f; // 360度表示一天的完整旋转
            directionalLight.transform.rotation = Quaternion.Euler(rotationAngle, 130, 90);

            // 计算太阳光的强度
            // 这里可以根据需要调整强度的变化规律，比如根据时间段来调整强度
            float intensity = lightIntensityCurve.Evaluate(evaluatedTime);
            directionalLight.intensity = intensity;
        }

        // 重新评估昼夜环境光颜色
        RenderSettings.ambientLight = ambientColorGradient.Evaluate(evaluatedTime);

        // 重新评估昼夜雾颜色
        RenderSettings.fogColor = fogColorGradient.Evaluate(evaluatedTime);
    }

}
