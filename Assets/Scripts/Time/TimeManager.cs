using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager singleton;

    // �֡�ʱ���յı��ر���
    public int minutes = 0;
    public int hours = 0;
    public int days = 0;

    // ������ҹ״̬��ö������
    public enum DayNightState
    {
        Day,
        Night
    }
    public DayNightState CurrentDayNightState { get; private set; }

    private LightManager lightManager;
    private float accumulatedTime = 0f;
    private float secondsPerMinute = 0.833f; // ����Ϊ��Ϸ��һ���Ӷ�Ӧ��ʵ���������

    private int lastEnemySpawnHour;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }

        lightManager = LightManager.singleton;
    }

    private void Update()
    {
        // �ۻ�ʱ��
        accumulatedTime += Time.deltaTime;

        // ����ۻ�ʱ�䳬����һ�룬���ӷ��Ӳ������ۻ�ʱ��
        if (accumulatedTime >= secondsPerMinute)
        {
            int minutesToAdd = Mathf.FloorToInt(accumulatedTime / secondsPerMinute);
            AddMinutes(minutesToAdd);
            accumulatedTime -= minutesToAdd * secondsPerMinute;

            // ���ݵ�ǰСʱ�ж���ҹ״̬
            if (hours >= 0 && hours < 12)
            {
                CurrentDayNightState = DayNightState.Day;
            }
            else
            {
                CurrentDayNightState = DayNightState.Night;
            }

            // ���»�����
            UpdateLight();
        }

        // Day / Night time events
        switch (CurrentDayNightState)
        {
            case DayNightState.Day: 
                {

                } 
                break;

            case DayNightState.Night:
                {
                    HandleSpawnAggressiveEnemyAIs();
                }
                break;
        }
    }

    public void AddMinutes(int minutesToAdd)
    {
        // ���ӷ���
        minutes += minutesToAdd;

        // ������Ӵﵽ��60�����н�λ
        if (minutes >= 60)
        {
            minutes -= 60;
            AddHours(1);
        }
    }

    public void AddHours(int hoursToAdd)
    {
        // ����Сʱ
        hours += hoursToAdd;

        // ���Сʱ�ﵽ��24�����н�λ
        if (hours >= 24)
        {
            hours -= 24;
            AddDays(1);
        }
    }

    public void AddDays(int daysToAdd)
    {
        // ��������
        days += daysToAdd;
    }

    // ��ȡ��ǰʱ��ķ���
    public string GetCurrentTime()
    {
        return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, days);
    }

    // �����Ӻ�Сʱת��Ϊ 0 �� 24 ��С��
    private float ConvertToDecimalTime()
    {
        return hours + (minutes / 60f);
    }

    // ���¹�����ɫ
    private void UpdateLight()
    {
        // ʹ�� LightManager �е� EvaluateColors ������������ɫ
        lightManager.EvaluateGlobalLight(ConvertToDecimalTime());
    }
    
    private void HandleSpawnAggressiveEnemyAIs()
    {
        if (lastEnemySpawnHour != hours)
        {
            WorldObjectSpawner.singleton.SpawnMobsNearPlayer();
            lastEnemySpawnHour = hours;
        }
    }
}