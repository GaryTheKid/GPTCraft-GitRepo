using UnityEngine;

[System.Serializable]
// Byte3结构定义
public struct Byte3
{
    public byte x;
    public byte y;
    public byte z;

    // 构造函数
    public Byte3(byte x, byte y, byte z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    // 为Byte3添加一个ToString方法，方便调试
    public override string ToString()
    {
        return $"({x}, {y}, {z})";
    }

    // 可以添加更多的方法来模仿Vector3的功能，比如加法、减法等
    // 例如，定义加法操作
    public static Byte3 operator +(Byte3 a, Byte3 b)
    {
        return new Byte3((byte)(a.x + b.x), (byte)(a.y + b.y), (byte)(a.z + b.z));
    }

    public static Vector3 operator +(Byte3 byte3, Vector3 vector)
    {
        // 将Byte3的x, y, z转换为float并与Vector3相加
        return new Vector3(byte3.x + vector.x, byte3.y + vector.y, byte3.z + vector.z);
    }
}

