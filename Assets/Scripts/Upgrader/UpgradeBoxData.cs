using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade box data")]
public class UpgradeBoxData : ScriptableObject
{
    public UpgradeTypes Type;
    public Sprite Icon, NotUpgradedIcon;
    public Sprite CurrencyIcon;
    public string Title, Description;
    public int MaxUpgradeCounter;
    public int StartCost;
    public float EndCost;
    public float StartValue;
    public float EndValue;
    public bool IsIntValue;

    public float GetProgressionValue(float progressionLerp)
    {
        return Mathf.Lerp(StartValue, EndValue, progressionLerp);
    }
}
