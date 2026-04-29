using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade data")]
public class UpgradePanelData : ScriptableObject
{
    public string UpgraderTitle;
    public string UpgraderTitleManager;

    public List<UpgradesData> _upgrades;

    [System.Serializable]
    public class UpgradesData
    {
        public List<UpgradeBoxData> _upgradeBoxesArray;
    }
}

public enum UpgradeTypes
{
    // port crane
    CraneSpeed, CartSpeed, OneMoreCart,
    Manager_CraneSpeed, Manager_CartSpeed,
    // square
    StorageCapacity, ReachStackerSpeed, StorageCraneSpeed,
    Manager_ReachStackerSpeed, Manager_StorageCraneSpeed,
    // cars
    OneMoreCar, CarSpeed, CheckPostSpeed,
    Manager_CarSpeed, Manager_CheckPostSpeed,
    // shipping office
    ContainerEarning, ShipSize, DiamondBonus, DiamondStorage,
    Manager_ContainerEarning, Manager_DiamondEarning, Manager_DiamondStorage
}