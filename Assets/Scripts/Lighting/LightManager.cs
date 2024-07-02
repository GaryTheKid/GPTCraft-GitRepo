using UnityEngine;

public class LightManager : MonoBehaviour
{
    public static LightManager singleton;

    public Gradient lightColorGradient;
    public Gradient ambientColorGradient;
    public Gradient fogColorGradient;
    public AnimationCurve lightIntensityCurve;

    private Light directionalLight;

    // ������Ҫ�仯����պУ������Ҫ�Ļ���
    // public Material skyboxMaterial;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }

        directionalLight = RenderSettings.sun;
    }

    // ���ݵ�ǰʱ��������������ɫ��ֵ��̫�������ת��ǿ��
    public void EvaluateGlobalLight(float currentTime)
    {
        // �������ʱ�����24���õ���Ӧ�Ľ���ֵ
        float evaluatedTime = currentTime / 24f;

        // ����������ҹ������ɫ����ת��ǿ��
        if (directionalLight != null)
        {
            // ����������ҹ������ɫ
            directionalLight.color = lightColorGradient.Evaluate(evaluatedTime);

            // ����̫�������ת�Ƕ�
            float rotationAngle = evaluatedTime * 360f; // 360�ȱ�ʾһ���������ת
            directionalLight.transform.rotation = Quaternion.Euler(rotationAngle, 130, 90);

            // ����̫�����ǿ��
            // ������Ը�����Ҫ����ǿ�ȵı仯���ɣ��������ʱ���������ǿ��
            float intensity = lightIntensityCurve.Evaluate(evaluatedTime);
            directionalLight.intensity = intensity;
        }

        // ����������ҹ��������ɫ
        RenderSettings.ambientLight = ambientColorGradient.Evaluate(evaluatedTime);

        // ����������ҹ����ɫ
        RenderSettings.fogColor = fogColorGradient.Evaluate(evaluatedTime);
    }

}
