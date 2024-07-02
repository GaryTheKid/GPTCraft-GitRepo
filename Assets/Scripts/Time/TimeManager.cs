using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager singleton;

    // 分、时、日的本地变量
    public int minutes = 0;
    public int hours = 0;
    public int days = 0;

    // 定义昼夜状态的枚举类型
    public enum DayNightState
    {
        Day,
        Night
    }
    public DayNightState CurrentDayNightState { get; private set; }

    private LightManager lightManager;
    private float accumulatedTime = 0f;
    private float secondsPerMinute = 0.833f; // 设置为游戏中一分钟对应现实世界的秒数

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
        // 累积时间
        accumulatedTime += Time.deltaTime;

        // 如果累积时间超过了一秒，增加分钟并重置累积时间
        if (accumulatedTime >= secondsPerMinute)
        {
            int minutesToAdd = Mathf.FloorToInt(accumulatedTime / secondsPerMinute);
            AddMinutes(minutesToAdd);
            accumulatedTime -= minutesToAdd * secondsPerMinute;

            // 根据当前小时判断昼夜状态
            if (hours >= 0 && hours < 12)
            {
                CurrentDayNightState = DayNightState.Day;
            }
            else
            {
                CurrentDayNightState = DayNightState.Night;
            }

            // 更新环境光
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
        // 增加分钟
        minutes += minutesToAdd;

        // 如果分钟达到了60，进行进位
        if (minutes >= 60)
        {
            minutes -= 60;
            AddHours(1);
        }
    }

    public void AddHours(int hoursToAdd)
    {
        // 增加小时
        hours += hoursToAdd;

        // 如果小时达到了24，进行进位
        if (hours >= 24)
        {
            hours -= 24;
            AddDays(1);
        }
    }

    public void AddDays(int daysToAdd)
    {
        // 增加天数
        days += daysToAdd;
    }

    // 获取当前时间的方法
    public string GetCurrentTime()
    {
        return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, days);
    }

    // 将分钟和小时转换为 0 到 24 的小数
    private float ConvertToDecimalTime()
    {
        return hours + (minutes / 60f);
    }

    // 更新光照颜色
    private void UpdateLight()
    {
        // 使用 LightManager 中的 EvaluateColors 函数来更新颜色
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