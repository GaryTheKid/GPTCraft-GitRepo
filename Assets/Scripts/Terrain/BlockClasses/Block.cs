using System.Collections.Generic;
using UnityEngine;

public enum BlockDirectionalState 
{
    N,
    H,
    V,
    HV
}

public abstract class Block : ScriptableObject
{
    [Header("====== Identity ======")]
    public byte id;
    public string blockName;
    public Item dropItem;

    [Space(25)]

    [Header("====== Attributes ======")]
    public bool isTransparent;
    public bool isGeneratingCollision;
    public bool isStateBlock;
    public bool isInteractable;
    public BlockDirectionalState directionalState;
    public float toughness;
    public float destroyEXPWorth;

    [Space(25)]

    [Header("====== Block Faces ======")]
    public Face[] faces;

    // functions
    protected void InitFaces()
    {
        foreach (Face face in faces)
        {
            // init directional block info
            switch (directionalState)
            {
                case BlockDirectionalState.H: face.SetDirectionalVerts_H(); break;
                case BlockDirectionalState.V: face.SetDirectionalVerts_V(); break;
                case BlockDirectionalState.HV: face.SetDirectionalVerts_HV(); break;
            }

            // init uv coords
            face.SetUVsFromSprite();
        }
    }
}

[System.Serializable]
public class Face
{
    private const float uvOffsets = 0.001f;

    public Vector3Int dir;
    
    public Vector3[] verts;
    public int[] tris;
    private Vector2[] uvs;
    private Dictionary<Vector3, Vector3[]> verts_Directional;

    public Sprite sprite;

    #region Directional Block
    public void SetDirectionalVerts_H()
    {
        verts_Directional = new Dictionary<Vector3, Vector3[]>();

        verts_Directional[new Vector3(0, 0, 1)] = verts;
        verts_Directional[new Vector3(0, 1, 1)] = verts;
        verts_Directional[new Vector3(0, -1, 1)] = verts;  // 001 

        Vector3[] V00N1 = CalcVertsDirectional_H_00N1();
        verts_Directional[new Vector3(1, 0, 0)] = V00N1;
        verts_Directional[new Vector3(1, 1, 0)] = V00N1;
        verts_Directional[new Vector3(1, -1, 0)] = V00N1; // 100

        Vector3[] VN100 = CalcVertsDirectional_H_N100();
        verts_Directional[new Vector3(0, 0, -1)] = VN100;
        verts_Directional[new Vector3(0, 1, -1)] = VN100;
        verts_Directional[new Vector3(0, -1, -1)] = VN100; // 00-1

        Vector3[] V001 = CalcVertsDirectional_H_001();
        verts_Directional[new Vector3(-1, 0, 0)] = V001;
        verts_Directional[new Vector3(-1, 1, 0)] = V001;
        verts_Directional[new Vector3(-1, -1, 0)] = V001;   // -100
    }
    public void SetDirectionalVerts_V()
    {
        verts_Directional = new Dictionary<Vector3, Vector3[]>();

        // 假设（1，1，0）为原始方向向量：
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                verts_Directional[new Vector3(x, 0, z)] = verts;
                verts_Directional[new Vector3(x, 1, z)] = verts;
            }
        }

        // 旋转顺时针180°：方向向量（1，-1，0）
        Vector3[] V0N10 = CalcVertsDirectional_V_0N10();
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                verts_Directional[new Vector3(x, -1, z)] = V0N10;
            }
        }
    }
    public void SetDirectionalVerts_HV()
    {
        verts_Directional = new Dictionary<Vector3, Vector3[]>();

        for (int y = 0; y <= 1; y++)
        {
            verts_Directional[new Vector3(0, y, 1)] = verts; // 001
            verts_Directional[new Vector3(1, y, 0)] = CalcVertsDirectional_H_00N1(); // 100
            verts_Directional[new Vector3(0, y, -1)] = CalcVertsDirectional_H_N100(); // 00-1
            verts_Directional[new Vector3(-1, y, 0)] = CalcVertsDirectional_H_001();   // -100
        }

        verts_Directional[new Vector3(0, -1, 1)] = CalcVertsDirectional_HV_1N10();   // 001
        verts_Directional[new Vector3(1, -1, 0)] = CalcVertsDirectional_HV_0N1N1(); // 100
        verts_Directional[new Vector3(0, -1, -1)] = CalcVertsDirectional_HV_N1N10(); // 00-1
        verts_Directional[new Vector3(-1, -1, 0)] = CalcVertsDirectional_HV_0N11();   // -100
    }
    private Vector3[] CalcVertsDirectional_H_00N1()
    {
        Vector3[] transformedVerts = new Vector3[verts.Length];

        for (int i = 0; i < verts.Length; i++)
        {
            transformedVerts[i] = new Vector3(verts[i].z, verts[i].y, 1 - verts[i].x);
        }

        return transformedVerts;
    }
    private Vector3[] CalcVertsDirectional_H_N100()
    {
        Vector3[] transformedVerts = new Vector3[verts.Length];

        for (int i = 0; i < verts.Length; i++)
        {
            transformedVerts[i] = new Vector3(1 - verts[i].x, verts[i].y, 1 - verts[i].z);
        }

        return transformedVerts;
    }
    private Vector3[] CalcVertsDirectional_H_001()
    {
        Vector3[] transformedVerts = new Vector3[verts.Length];

        for (int i = 0; i < verts.Length; i++)
        {
            transformedVerts[i] = new Vector3(1 - verts[i].z, verts[i].y, verts[i].x);
        }

        return transformedVerts;
    }
    private Vector3[] CalcVertsDirectional_V_0N10()
    {
        Vector3[] transformedVerts = new Vector3[verts.Length];

        for (int i = 0; i < verts.Length; i++)
        {
            transformedVerts[i] = new Vector3(verts[i].x, 1 - verts[i].y, 1 - verts[i].z);
        }

        return transformedVerts;
    }
    private Vector3[] CalcVertsDirectional_HV_1N10()
    {
        Vector3[] transformedVerts = new Vector3[verts.Length];

        for (int i = 0; i < verts.Length; i++)
        {
            transformedVerts[i] = new Vector3(1 - verts[i].x, 1 - verts[i].y, verts[i].z);
        }

        return transformedVerts;
    }
    private Vector3[] CalcVertsDirectional_HV_0N1N1()
    {
        Vector3[] transformedVerts = new Vector3[verts.Length];

        for (int i = 0; i < verts.Length; i++)
        {
            transformedVerts[i] = new Vector3(1 - verts[i].z, 1 - verts[i].y, 1 - verts[i].x);
        }

        return transformedVerts;
    }
    private Vector3[] CalcVertsDirectional_HV_N1N10()
    {
        Vector3[] transformedVerts = new Vector3[verts.Length];

        for (int i = 0; i < verts.Length; i++)
        {
            transformedVerts[i] = new Vector3(verts[i].x, 1 - verts[i].y, 1 - verts[i].z);
        }

        return transformedVerts;
    }
    private Vector3[] CalcVertsDirectional_HV_0N11()
    {
        Vector3[] transformedVerts = new Vector3[verts.Length];

        for (int i = 0; i < verts.Length; i++)
        {
            transformedVerts[i] = new Vector3(verts[i].z, 1 - verts[i].y, verts[i].x);
        }

        return transformedVerts;
    }
    public Vector3[] GetVertsDirectional(Vector3 blockDir)
    {
        return verts_Directional[blockDir];
    }
    #endregion


    #region UVs
    public void SetUVsFromSprite()
    {
        float uMin = sprite.rect.x / sprite.texture.width;
        float vMin = sprite.rect.y / sprite.texture.height;
        float uMax = (sprite.rect.x + sprite.rect.width) / sprite.texture.width;
        float vMax = (sprite.rect.y + sprite.rect.height) / sprite.texture.height;

        uvs = new Vector2[4];
        uvs[0] = new Vector2(uMin + uvOffsets, vMin + uvOffsets);
        uvs[1] = new Vector2(uMax - uvOffsets, vMin + uvOffsets);
        uvs[2] = new Vector2(uMax - uvOffsets, vMax - uvOffsets);
        uvs[3] = new Vector2(uMin + uvOffsets, vMax - uvOffsets);
    }
    public Vector2[] GetUVs()
    {
        return uvs;
    }
    #endregion
}