public interface IFurnanceSource
{
    void LoadSource();
    void InitializeProduction();
    void ResetProduction();
    string GetProductHashString();
    bool IsInventoryStateMatched(InventoryPageState state);
}
