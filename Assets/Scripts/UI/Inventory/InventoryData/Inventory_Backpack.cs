public class Inventory_Backpack : Inventory
{
    public const int INVENTORY_INDEX = 1;
    public const int SIZE = 27;
    public const int ID_HEAD = 9;

    public Inventory_Backpack() : base(SIZE, ID_HEAD)
    {
        // 可以在这里对 inventory 进行初始化
        size = SIZE;
        idHead = ID_HEAD;
        inventorySlots = new InventorySlot[size];
    }
}