using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OfflineEarningPanel : UIPanel
{
    [SerializeField] private TextMeshProUGUI _earning;

    public override void SetActive(bool value, Upgraders upgrader = Upgraders.None)
    {
        base.SetActive(value);

        if (value)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    protected override void Open()
    {
        base.Open();
    }

    protected override void Close()
    {
        base.Close();
    }

    public void InitPanelData(string moneyAmount)
    {
        _earning.text = moneyAmount;
    }

    public void ClaimReward()
    {
        _ui.EnablePanel(Panels.HudPanel);
    }
}