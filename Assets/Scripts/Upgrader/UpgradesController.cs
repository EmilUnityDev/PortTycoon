using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesController : MonoBehaviour
{
    public event Action OnSquareUpgraded;
    public event Action OnDiamondCapacityUpgraded;

    [SerializeField] private List<UpgradeBoxData> _upgrades;

    private Dictionary<UpgradeTypes, UpgradeBoxData> _upgrades_map = new Dictionary<UpgradeTypes, UpgradeBoxData>();
    private Dictionary<UpgradeTypes, int> _upgradesProgression = new Dictionary<UpgradeTypes, int>();
    private ResourcesContainer _resourcesContainer;

    public void InitUpgradesData(ResourcesContainer resourcesContainer)
    {
        _resourcesContainer = resourcesContainer;

        for (int i = 0; i < _upgrades.Count; i++)
        {
            int reloadedProgression = PlayerPrefs.HasKey(PrefsContainer.PROGRESS_UPGRADE + _upgrades[i].Type) ? PlayerPrefs.GetInt(PrefsContainer.PROGRESS_UPGRADE + _upgrades[i].Type) : 1;

            _upgrades_map.Add(_upgrades[i].Type, _upgrades[i]);
            _upgradesProgression.Add(_upgrades[i].Type, reloadedProgression);
        }
    }

    public void InvokeActions()
    {
        OnSquareUpgraded?.Invoke();
        OnDiamondCapacityUpgraded?.Invoke();
    }

    public UpgradeBackData TryToUpgrade(UpgradeTypes type, bool isMoneySpend)
    {
        if (type == UpgradeTypes.StorageCapacity)
        {
            OnSquareUpgraded?.Invoke();
        }

        int cost = GetCost(type);

        bool isEnoughMoney = isMoneySpend ? IsEnoughMoney(cost) : IsEnoughDiamonds(cost);
        bool isCanUpgrade = isEnoughMoney && _upgradesProgression[type] < _upgrades_map[type].MaxUpgradeCounter;

        UpgradeBackData backData = new UpgradeBackData();
        backData.StringNextCost = "MAX";
        backData.IsUpdateValues = isEnoughMoney;
        backData.IsCanUpgrade = false;
        backData.CurrentUpgradeProgression = _upgradesProgression[type];
        backData.MaxUpgradeProgression = _upgrades_map[type].MaxUpgradeCounter;

        if (isCanUpgrade)
        {
            if (isMoneySpend)
            {
                _resourcesContainer.RemoveMoney(cost);
            }
            else
            {
                _resourcesContainer.RemoveDiamonds(cost);
            }

            _upgradesProgression[type]++;

            PlayerPrefs.SetInt(PrefsContainer.PROGRESS_UPGRADE + type, _upgradesProgression[type]);

            int nextCost = GetCost(type);
            
            backData.CurrentUpgradeProgression = _upgradesProgression[type];

            if (_upgradesProgression[type] < _upgrades_map[type].MaxUpgradeCounter)
            {
                backData.IntNextCost = nextCost;
                backData.StringNextCost = nextCost.ConvertToString();
                backData.IsCanUpgrade = isMoneySpend ? IsEnoughMoney(nextCost) : IsEnoughDiamonds(nextCost);
            }
            else
            {
                backData.StringNextCost = "MAX";
                backData.IsCanUpgrade = false;
            }

            if (type == UpgradeTypes.DiamondStorage || type == UpgradeTypes.Manager_DiamondStorage)
            {
                OnDiamondCapacityUpgraded?.Invoke();
            }
        }

        return backData;
    }

    public UpgradeBackData GetUpgradeDataByType(UpgradeTypes type, bool isMoneySpend)
    {
        UpgradeBackData backData = new UpgradeBackData();

        int cost = GetCost(type);
        int currentUpgradeProgression = _upgradesProgression[type];
        int maxUpgradeProgression = _upgrades_map[type].MaxUpgradeCounter;

        bool isEnoughMoney = isMoneySpend ? IsEnoughMoney(cost) : IsEnoughDiamonds(cost);

        backData.IsUpdateValues = true;
        backData.IsCanUpgrade = isEnoughMoney && currentUpgradeProgression < maxUpgradeProgression;
        backData.IntNextCost = cost;
        backData.StringNextCost = currentUpgradeProgression < maxUpgradeProgression ? cost.ConvertToString() : "MAX";
        backData.CurrentUpgradeProgression = currentUpgradeProgression;
        backData.MaxUpgradeProgression = maxUpgradeProgression;

        return backData;
    }

    public float GetProgressionValue(UpgradeTypes type)
    {
        return Mathf.Lerp(_upgrades_map[type].StartValue, _upgrades_map[type].EndValue, (float)GetCurrentProgressionByType(type) / GetMaxProgressionByType(type));
    }

    public int GetCurrentProgressionByType(UpgradeTypes type)
    {
        return _upgradesProgression[type];
    }

    public int GetMaxProgressionByType(UpgradeTypes type)
    {
        return _upgrades_map[type].MaxUpgradeCounter;
    }

    public bool IsEnoughMoney(UpgradeTypes type)
    {
        return _resourcesContainer.IsEnoughMoney(GetCost(type));
    }

    public bool IsEnoughMoney(int cost)
    {
        return _resourcesContainer.IsEnoughMoney(cost);
    }

    public bool IsEnoughDiamonds(int cost)
    {
        return _resourcesContainer.IsEnoughDiamonds(cost);
    }

    private int GetCost(UpgradeTypes type)
    {
        return (int)Mathf.Lerp(_upgrades_map[type].StartCost, _upgrades_map[type].EndCost, (float)(_upgradesProgression[type] - 1) / (_upgrades_map[type].MaxUpgradeCounter - 1));
    }
}

public struct UpgradeBackData
{
    public bool IsUpdateValues;
    public bool IsCanUpgrade;
    public string StringNextCost;
    public int IntNextCost;
    public int CurrentUpgradeProgression, MaxUpgradeProgression;
}