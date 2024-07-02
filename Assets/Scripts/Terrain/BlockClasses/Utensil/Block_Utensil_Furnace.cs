using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Block_Utensil_Furnace", menuName = "ScriptableObject/Block/Utensil/Furnace")]
public class Block_Utensil_Furnace : Block_Utensil, IBlock_Stone, IBlock_StateBlock, IBlock_InteractableBlock
{
    public delegate void OnBlockInteractEventHandler();
    public static event OnBlockInteractEventHandler OnBlockInteractEvent;

    [Header("====== State Block ======")]
    public Face[] faces_state_1_On;
    public List<Face[]> stateFaces; 

    public static byte refID;

    private void OnEnable()
    {
        refID = id;
        InitFaces();
        InitializeStateFaces();
    }

    public void InitializeStateFaces()
    {
        InitDirectionalStateFaces();

        stateFaces = new List<Face[]>();
        stateFaces.Add(faces);
        stateFaces.Add(faces_state_1_On);
    }

    public Face[] GetStateFaces(byte state)
    {
        return stateFaces[state];
    }

    private void InitDirectionalStateFaces()
    {
        foreach (Face face in faces_state_1_On)
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

    public void Interact()
    {
        OnBlockInteractEvent.Invoke();
    }
}