public class Inventory_Backpack : Inventory
{
    public const int INVENTORY_INDEX = 1;
    public const int SIZE = 27;
    public const int ID_HEAD = 9;

    public Inventory_Backpack() : base(SIZE, ID_HEAD)
    {
        // ����������� inventory ���г�ʼ��
        size = SIZE;
        idHead = ID_HEAD;
        inventorySlots = new InventorySlot[size];
    }
}