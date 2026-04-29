using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeLayoutBoxData : MonoBehaviour
{
    [SerializeField] private Image _icon, _currencyIcon;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _maxUpgradeCounter;
    [SerializeField] private TextMeshProUGUI _cost;
    [SerializeField] private Slider _progressionSlider;
    [SerializeField] private TextMeshProUGUI _progressionText;
    [SerializeField] private Button _button;
    [SerializeField] private Color _nonUpgradedIconColor;
    [SerializeField] private GameObject _tutorialFinger;

    private Sprite _upgradedIcon;
    private ResourcesContainer _resContainer;
    private UpgradePanel _upgradePanel;
    private UpgradeBoxData _boxData;
    private UpgradeTypes _upgradeType;
    private int _currentCost;
    private bool _isMaxed, _isOfficeUpgrade;

    public void FillWithData(UpgradeBoxData data, UpgradePanel upgradePanel, UpgradeBackData buttonData, ResourcesContainer resourcesContainer, bool isOfficeUpgrade)
    {
        _isOfficeUpgrade = isOfficeUpgrade;

        bool isEnoughMoney = isOfficeUpgrade ? resourcesContainer.IsEnoughMoney(buttonData.IntNextCost) : resourcesContainer.IsEnoughDiamonds(buttonData.IntNextCost);

        _resContainer = resourcesContainer;
        _boxData = data;

        _upgradePanel = upgradePanel;
        _upgradeType = data.Type;
        _icon.sprite = data.Icon;
        _currencyIcon.sprite = data.CurrencyIcon;
        if (!_isOfficeUpgrade)
        {
            _currencyIcon.transform.localRotation = Quaternion.identity;
        }
        _title.text = data.Title;
        _description.text = data.Description;
        if (!isOfficeUpgrade && buttonData.CurrentUpgradeProgression == 1)
        {
            _maxUpgradeCounter.text = "LOCKED";
        }
        else
        {
            _maxUpgradeCounter.text = buttonData.CurrentUpgradeProgression + "/" + data.MaxUpgradeCounter;
        }
        _cost.text = buttonData.StringNextCost;
        _progressionSlider.value = (float)(buttonData.CurrentUpgradeProgression - (isOfficeUpgrade || data.IsIntValue ? 0 : 1)) / buttonData.MaxUpgradeProgression;
        _button.interactable = isEnoughMoney && buttonData.CurrentUpgradeProgression < buttonData.MaxUpgradeProgression;
        _currentCost = buttonData.IntNextCost;
        _isMaxed = buttonData.CurrentUpgradeProgression >= buttonData.MaxUpgradeProgression;

        SetProgressionValueText(_progressionSlider.value);

        if (_isOfficeUpgrade)
        {
            _resContainer.OnMoneyChanged += CheckForEnoughMoney;
        }
        else
        {
            _resContainer.OnDiamondsChanged += CheckForEnoughDiamonds;

            _upgradedIcon = data.Icon;

            if (buttonData.CurrentUpgradeProgression == 1)
            {
                _icon.sprite = data.NotUpgradedIcon;
                _icon.color = _nonUpgradedIconColor;
            }
            else
            {
                _icon.sprite = data.Icon;
                _icon.color = Color.white;
            }
        }
    }

    private void OnDestroy()
    {
        if (_isOfficeUpgrade)
        {
            _resContainer.OnMoneyChanged -= CheckForEnoughMoney;
        }
        else
        {
            _resContainer.OnDiamondsChanged -= CheckForEnoughDiamonds;
        }
    }

    public void UpdateData(UpgradeBackData buttonData)
    {
        if (buttonData.IsUpdateValues)
        {
            _cost.text = buttonData.StringNextCost;
            _isMaxed = buttonData.CurrentUpgradeProgression >= buttonData.MaxUpgradeProgression;
            _maxUpgradeCounter.text = buttonData.CurrentUpgradeProgression + "/" + buttonData.MaxUpgradeProgression;
            _progressionSlider.value = (float)(buttonData.CurrentUpgradeProgression) / buttonData.MaxUpgradeProgression;
            _button.interactable = buttonData.IsCanUpgrade;
            _currentCost = buttonData.IntNextCost;

            if (!_isOfficeUpgrade && buttonData.CurrentUpgradeProgression == 2)
            {
                _icon.sprite = _upgradedIcon;
                _icon.color = Color.white;
            }

            SetProgressionValueText(_progressionSlider.value);
        }
    }

    private void SetProgressionValueText(float lerpValue)
    {
        string format = _progressionSlider.value % 1 == 0 || _boxData.IsIntValue ? "0" : "1";

        if (_boxData.IsIntValue)
        {
            _progressionText.text = Mathf.Floor(_boxData.GetProgressionValue(lerpValue)).ToString();
        }
        else
        {
           _progressionText.text = _boxData.GetProgressionValue(lerpValue).ToString("F" + format);
        }
    }

    private void CheckForEnoughMoney(int moneyAmount)
    {
        _button.interactable = _currentCost <= moneyAmount && !_isMaxed;
    }

    private void CheckForEnoughDiamonds(int diamondsAmount)
    {
        _button.interactable = _currentCost <= diamondsAmount && !_isMaxed;
    }

    public void UpgradeButtonPressed()
    {
        _upgradePanel.UpgradeButtonPressed(_upgradeType, _isOfficeUpgrade);
    }

    public UpgradeTypes GetBoxType()
    {
        return _upgradeType;
    }

    public void SetActiveTutorialFinger(bool value)
    {
        _tutorialFinger.SetActive(value);
    }
}