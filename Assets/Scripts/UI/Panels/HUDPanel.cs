using UnityEngine;
using UnityEngine.UI;

public class HUDPanel : UIPanel
{
    [SerializeField] private TMPro.TextMeshProUGUI _moneyText, _crystalText;

    public override void SetActive(bool value, Upgraders type = Upgraders.None)
    {
        base.SetActive(value);
    }

    protected override void Open()
    {
        base.Open();
    }

    protected override void Close()
    {
        base.Close();
    }

    public void UpdateMoneyText(int value)
    {
        _moneyText.text = value.ConvertToString();
    }

    public void UpdateCrystalText(int value)
    {
        _crystalText.text = value.ConvertToString();
    }
}
