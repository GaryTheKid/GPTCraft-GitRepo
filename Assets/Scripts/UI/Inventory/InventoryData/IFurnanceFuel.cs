using System;

public interface IFurnanceFuel
{
    void LoadFuel();
    void InitializeCombustion();
    void SetCombustionProgressUpdate(Action<float> OnCombustionProgressUpdate);
    bool IsInventoryStateMatched(InventoryPageState state);
}
