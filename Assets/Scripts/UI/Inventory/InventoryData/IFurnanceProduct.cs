using System;

public interface IFurnanceProduct
{
    #region Initialization
    void SetProductionData(FurnanceProductionData productionData);
    FurnanceProductionData GetProductionData();
    void SetProductionProgressUpdate(Action<float> OnProductionProgressUpdate);
    void SetOnCompletionUIUpdate(Action OnCompletionUIUpdate);
    #endregion


    #region Production
    bool StartProductionProcess(Action OnAttemptReInitializeProduction, Action OnReloadSource, Action ConsumeOneSourceItem, FurnanceProductionData productionData);
    void StopProductionProcess();
    bool IsProductSlotReadyForProduction(Item prodItem);
    bool IsProductSlotStackFull();
    #endregion


    #region Combustion
    bool StartCombustionProcess(Action OnAttemptReInitializeCombustion, Action OnReloadFuel, Action<float> OnCombustionProgressUpdate, FurnanceProductionData productionData);
    #endregion
}