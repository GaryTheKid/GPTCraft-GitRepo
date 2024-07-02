using UnityEngine;

[CreateAssetMenu(fileName = "Block_Utensil_CraftingTable", menuName = "ScriptableObject/Block/Utensil/CraftingTable")]
public class Block_Utensil_CraftingTable : Block_Utensil, IBlock_Wood, IBlock_InteractableBlock
{
    public delegate void OnBlockInteractEventHandler();
    public static event OnBlockInteractEventHandler OnBlockInteractEvent;

    public static byte refID;

    private void OnEnable()
    {
        refID = id;
        InitFaces();
    }

    public void Interact()
    {
        OnBlockInteractEvent.Invoke();
    }
}
