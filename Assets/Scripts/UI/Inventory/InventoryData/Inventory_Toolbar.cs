public class Inventory_Toolbar : Inventory
{
    public const int INVENTORY_INDEX = 0;
    public const int SIZE = 9;
    public const int ID_HEAD = 0;

    public Inventory_Toolbar() : base(SIZE, ID_HEAD)
    {
        // 可以在这里对 inventory 进行初始化
        size = SIZE;
        idHead = ID_HEAD;
        inventorySlots = new InventorySlot[size];
    }
}
