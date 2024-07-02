public interface IUIPage
{
    bool IsPageActive();
    void PageOn();
    void PageOff();

    InventoryPageState GetPageState();
}
