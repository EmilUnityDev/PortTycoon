using System;
using UnityEngine;

public class ResourcesContainer : MonoBehaviour
{
    public event Action<int> OnMoneyChanged, OnDiamondsChanged;

    private int _moneyValue = 0;
    private int _diamondsValue = 0;

    public void Init()
    {
        _moneyValue += PlayerPrefs.HasKey(PrefsContainer.MONEY) ? PlayerPrefs.GetInt(PrefsContainer.MONEY) : 600;
        _diamondsValue += PlayerPrefs.GetInt(PrefsContainer.DIAMONDS);

        OnMoneyChanged += SaveMoney;
        OnDiamondsChanged += SaveDiamonds;

        OnMoneyChanged?.Invoke(_moneyValue);
        OnDiamondsChanged?.Invoke(_diamondsValue);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            AddMovey(UnityEngine.Random.Range(1000, 3000));
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            AddCrystal(UnityEngine.Random.Range(5, 20));
        }
    }
#endif

    public void AddMovey(int value)
    {
        _moneyValue += value;

        OnMoneyChanged?.Invoke(_moneyValue);
    }

    public void AddCrystal(int value)
    {
        _diamondsValue += value;

        OnDiamondsChanged?.Invoke(_diamondsValue);
    }

    public void RemoveMoney(int value)
    {
        if (IsEnoughMoney(_moneyValue - value))
        {
            _moneyValue -= value;

            OnMoneyChanged?.Invoke(_moneyValue);
        }
    }

    public void RemoveDiamonds(int value)
    {
        if (IsEnoughDiamonds(_diamondsValue - value))
        {
            _diamondsValue -= value;

            OnDiamondsChanged?.Invoke(_diamondsValue);
        }
    }

    public bool IsEnoughMoney(int cost)
    {
        return _moneyValue >= cost;
    }

    public bool IsEnoughDiamonds(int cost)
    {
        return _diamondsValue >= cost;
    }

    private void SaveMoney(int amount)
    {
        PlayerPrefs.SetInt(PrefsContainer.MONEY, amount);
    }

    private void SaveDiamonds(int amount)
    {
        PlayerPrefs.SetInt(PrefsContainer.DIAMONDS, amount);
    }
}