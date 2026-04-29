using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Panels _startPanel;
    [SerializeField] private List<PanelData> _panels;
    [SerializeField] private List<GameObject> _uiToHide;

    [Header("Panels components")]
    [SerializeField] private HUDPanel _hud;
    [SerializeField] private OfflineEarningPanel _offlineEarningPanel;
    [Space]

    private Dictionary<Panels, List<GameObject>> _panels_map = new Dictionary<Panels, List<GameObject>>();
    private Tutorial _tutorial;
    private Panels _currentPanel;
    private bool _isHidedMoney;

    public void Init(UpgradesController upgradesController, ResourcesContainer resourcesContainer, Tutorial tutorial, int offlineEarning = -1)
    {
        _tutorial = tutorial;

        for (int i = 0; i < _panels.Count; i++)
        {
            _panels_map.Add(_panels[i].PanelType, _panels[i].AssociatedObjects);

            for (int j = 0; j < _panels[i].AssociatedObjects.Count; j++)
            {
                if (_panels[i].AssociatedObjects[0].TryGetComponent(out UIPanel uipanel))
                {
                    uipanel.Init(this, upgradesController, resourcesContainer);
                }
            }
        }

        resourcesContainer.OnMoneyChanged += UpdateMoneyValue;
        resourcesContainer.OnDiamondsChanged += UpdateCrystalValue;

        if (offlineEarning > 0)
        {
            EnablePanel(Panels.OfflineEarning);
            _offlineEarningPanel.InitPanelData(offlineEarning.ConvertToString());

            _isHidedMoney = true;
        }
        else
        {
            EnablePanel(_startPanel);
        }
    }

    public void EnablePanel(Panels panel, Upgraders upgraderType = Upgraders.None)
    {
        _currentPanel = panel;

        if (_isHidedMoney)
        {
            for (int i = 0; i < _uiToHide.Count; i++)
            {
                _uiToHide[i].gameObject.SetActive(true);
            }
        }

        if (panel == Panels.BuildingPanel || panel == Panels.OfflineEarning)
        {
            for (int i = 0; i < _uiToHide.Count; i++)
            {
                _uiToHide[i].gameObject.SetActive(false);
            }

            _isHidedMoney = true;
        }

        foreach (var panelData in _panels_map)
        {
            for (int i = 0; i < panelData.Value.Count; i++)
            {
                bool isSetActive = panelData.Key == panel;

                if (panelData.Value[i].TryGetComponent(out UIPanel uipanel))
                {
                    uipanel.SetActive(isSetActive, upgraderType);

                    if (upgraderType != Upgraders.None && 
                        (
                        (upgraderType == Upgraders.PortCrane && _tutorial.IsPowerStationUpgrade && (uipanel is UpgradePanel)) ||
                        (upgraderType == Upgraders.Cars && _tutorial.IsGarageUpgrade && (uipanel is UpgradePanel))
                        ))
                    {
                        (uipanel as UpgradePanel).CreateFingerAtFirstAndForbidToQuit();
                    }
                }
                else
                {
                    panelData.Value[i].SetActive(isSetActive);
                }
            }
        }
    }

    private void UpdateMoneyValue(int value)
    {
        _hud.UpdateMoneyText(value);
    }

    private void UpdateCrystalValue(int value)
    {
        _hud.UpdateCrystalText(value);
    }

    public void OpenUpgradePanel(Upgraders type)
    {
        EnablePanel(Panels.UpgradePanel, type);
    }

    public Panels GetCurrentPanelType()
    {
        return _currentPanel;
    }

    public void BuildButtonPressed()
    {
        EnablePanel(Panels.HudPanel);
        _tutorial.StopBuildWaiting();
    }

    [System.Serializable]
    private class PanelData
    {
        public Panels PanelType;
        public List<GameObject> AssociatedObjects;
    }

    public void SetWaterActive(bool value)
    {
        GameObject.Find("WaterParent").transform.GetChild(0).gameObject.SetActive(value);
    }
}

public enum Panels
{
    HudPanel, UpgradePanel, OfflineEarning, BuildingPanel
}