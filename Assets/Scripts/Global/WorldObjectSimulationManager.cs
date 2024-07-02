using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectSimulationManager : MonoBehaviour
{
    private IEnumerator CachedCoroutine_FurnanceProduction;
    private IEnumerator CachedCoroutine_FurnanceCombustion;

    public static WorldObjectSimulationManager singleton;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
    }


    #region Simulation
    public bool Simulate_StartFurnanceProduction(
        FurnanceProductionData productionData,
        Action OnAttemptReInitializeProduction, 
        Action OnReloadSource,
        Action ConsumeOneSourceItem,
        Action<float> OnProductionProgressUpdate, 
        Action<Action, Action, Action, IForgeable, Item> OnProductionComplete, 
        Action OnCompletionUIUpdate)
    {
        if (CachedCoroutine_FurnanceProduction == null)
        {
            CachedCoroutine_FurnanceProduction = Coroutine_FurnanceProduction(productionData, OnAttemptReInitializeProduction, OnReloadSource, ConsumeOneSourceItem, OnProductionProgressUpdate, OnProductionComplete, OnCompletionUIUpdate);
            StartCoroutine(CachedCoroutine_FurnanceProduction);

            return true;
        }

        return false;
    }
    
    public void Simulate_StopFurnanceProduction(
        FurnanceProductionData productionData,
        Action<float> OnProductionProgressUpdate,
        Action OnCompletionUIUpdate)
    {
        if (CachedCoroutine_FurnanceProduction != null)
        {
            StopCoroutine(CachedCoroutine_FurnanceProduction);
            CachedCoroutine_FurnanceProduction = null;
        }

        productionData.productionProgress = 0f;
        OnProductionProgressUpdate(0f);
        OnCompletionUIUpdate();
    }

    public bool Simulate_StartFurnanceCombustion(
        FurnanceProductionData productionData,
        Action OnAttemptReInitializeCombustion,
        Action OnReloadFuel,
        Action<float> OnCombustionProgressUpdate, 
        Action<Action, Action> OnCombustionComplete, 
        Action OnCompletionUIUpdate)
    {
        if (CachedCoroutine_FurnanceCombustion == null)
        {
            CachedCoroutine_FurnanceCombustion = Coroutine_FurnanceCombust(productionData, OnAttemptReInitializeCombustion, OnReloadFuel, OnCombustionProgressUpdate, OnCombustionComplete, OnCompletionUIUpdate);
            StartCoroutine(CachedCoroutine_FurnanceCombustion);

            return true;
        }

        return false;
    }
    #endregion


    #region Coroutines
    private IEnumerator Coroutine_FurnanceProduction(
    FurnanceProductionData productionData,
    Action OnAttemptReInitializeProduction,
    Action OnReloadSource,
    Action ConsumeOneSourceItem,
    Action<float> OnProductionProgressUpdate,
    Action<Action, Action, Action, IForgeable, Item> OnProductionComplete,
    Action OnCompletionUIUpdate)
    {
        float totalTime = productionData.prodForgeInfo.GetProductionTime(); // ��ȡ�ӹ�������ʱ��
        float elapsedTime = productionData.productionProgress * totalTime; // ���ó�ʼʱ��Ϊ��һ�μӹ��Ľ���

        while (elapsedTime < totalTime)
        {
            // ���
            if (!productionData.isActive)
            {
                CachedCoroutine_FurnanceProduction = null;
                yield break;
            }

            // ���¼ӹ�����
            productionData.productionProgress = elapsedTime / totalTime;
            OnProductionProgressUpdate(productionData.productionProgress);

            // �ȴ�һ֡
            yield return null;

            // �ۼ��Ѿ����ŵ�ʱ��
            elapsedTime += Time.deltaTime;
        }

        // clear cache
        CachedCoroutine_FurnanceProduction = null;

        // �ӹ����
        productionData.productionProgress = 0f;
        OnProductionProgressUpdate(0f);
        OnProductionComplete(OnAttemptReInitializeProduction, OnReloadSource, ConsumeOneSourceItem, productionData.prodForgeInfo, productionData.prodItem);
        OnCompletionUIUpdate();
    }


    private IEnumerator Coroutine_FurnanceCombust(
        FurnanceProductionData productionData,
        Action OnAttemptReInitializeCombustion,
        Action OnReloadFuel,
        Action<float> OnCombustionProgressUpdate, 
        Action<Action, Action> OnCombustionComplete, 
        Action OnCompletionUIUpdate)
    {
        float elapsedTime = 0f;
        float totalCombustionTime = productionData.fuelInfo.GetCombustValue();

        while (elapsedTime < totalCombustionTime)
        {
            // ����ȼ�ս���
            float progress = (totalCombustionTime - elapsedTime) / totalCombustionTime;

            // ���������ȼ�ս��Ȼ򴥷���Ӧ�Ļص�����
            OnCombustionProgressUpdate(progress);

            // �ȴ�һ֡
            yield return null;

            // �ۼ��Ѿ����ŵ�ʱ��
            elapsedTime += Time.deltaTime;
        }

        // clear cache
        CachedCoroutine_FurnanceCombustion = null;

        // ȼ�����
        OnCombustionProgressUpdate(0f);
        OnCombustionComplete(OnAttemptReInitializeCombustion, OnReloadFuel);
        OnCompletionUIUpdate();
    }

    #endregion
}