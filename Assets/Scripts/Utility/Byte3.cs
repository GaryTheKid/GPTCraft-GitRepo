using UnityEngine;

[System.Serializable]
// Byte3�ṹ����
public struct Byte3
{
    public byte x;
    public byte y;
    public byte z;

    // ���캯��
    public Byte3(byte x, byte y, byte z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    // ΪByte3���һ��ToString�������������
    public override string ToString()
    {
        return $"({x}, {y}, {z})";
    }

    // ������Ӹ���ķ�����ģ��Vector3�Ĺ��ܣ�����ӷ���������
    // ���磬����ӷ�����
    public static Byte3 operator +(Byte3 a, Byte3 b)
    {
        return new Byte3((byte)(a.x + b.x), (byte)(a.y + b.y), (byte)(a.z + b.z));
    }

    public static Vector3 operator +(Byte3 byte3, Vector3 vector)
    {
        // ��Byte3��x, y, zת��Ϊfloat����Vector3���
        return new Vector3(byte3.x + vector.x, byte3.y + vector.y, byte3.z + vector.z);
    }
}

