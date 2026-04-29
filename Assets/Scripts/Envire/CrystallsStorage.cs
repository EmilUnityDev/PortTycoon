using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystallsStorage : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private TMPro.TextMeshProUGUI _crystallsAmount;

    private List<GameObject> _createdCrystalls = new List<GameObject>();
    private UpgradesController _upgradesController;
    private CrystalsFactory _crFactory;
    private ResourcesContainer _resContainer;
    private int _currentAmount = 0;

    public void Init(CrystalsFactory crFactory, UpgradesController upgradesController, ResourcesContainer resContainer, int offlineDiamondEarning)
    {
        _resContainer = resContainer;
        _upgradesController = upgradesController;
        _crFactory = crFactory;

        upgradesController.OnDiamondCapacityUpgraded += UpdateCapacityValues;

        UpdateCapacityValues();
        RespawnSavedDiamonds(offlineDiamondEarning);
    }

    private void RespawnSavedDiamonds(int offlineEarning)
    {
        //if (PlayerPrefs.GetInt(PrefsContainer.IS_TUTORIAL_PASSED) == 1)
        {
            int savedDiamonds = PlayerPrefs.GetInt(PrefsContainer.DIAMONDS_IN_CONTAINER) + offlineEarning;

            if (savedDiamonds != 0)
            {
                Spawn(savedDiamonds);
            }
        }
    }

    public void Spawn(int amount)
    {
        int maxValue = (int)(_upgradesController.GetProgressionValue(UpgradeTypes.DiamondStorage) +
             (_upgradesController.GetCurrentProgressionByType(UpgradeTypes.Manager_DiamondStorage) == 1 ? 0 :
            ((int)_upgradesController.GetProgressionValue(UpgradeTypes.Manager_DiamondStorage))));

        int oldValue = _currentAmount;
        _currentAmount += amount;
        _currentAmount = Mathf.Clamp(_currentAmount, 0, maxValue);
        int difference = _currentAmount - oldValue;

        PlayerPrefs.SetInt(PrefsContainer.DIAMONDS_IN_CONTAINER, _currentAmount);

        if (difference > 0)
        {
            for (int i = 0; i < difference; i++)
            {
                GameObject newCrystall = _crFactory.Spawn(_spawnPoint.position + Random.insideUnitSphere / 2f);
                _createdCrystalls.Add(newCrystall);
            }
        }

        UpdateCapacityValues();
    }

    public bool GetAllCrystalls()
    {
        bool hasCrystalls = _currentAmount > 0;

        _resContainer.AddCrystal(_currentAmount);

        for (int i = 0; i < _createdCrystalls.Count; i++)
        {
            Destroy(_createdCrystalls[i]);
        }

        _currentAmount = 0;
        _createdCrystalls.Clear();

        UpdateCapacityValues();

        return hasCrystalls;
    }

    private void UpdateCapacityValues()
    {
        _crystallsAmount.text = _currentAmount + "/" + (
            (int)_upgradesController.GetProgressionValue(UpgradeTypes.DiamondStorage) + (
            _upgradesController.GetCurrentProgressionByType(UpgradeTypes.Manager_DiamondStorage) == 1 ? 0 :
            ((int)_upgradesController.GetProgressionValue(UpgradeTypes.Manager_DiamondStorage))));
    }

    public Vector3 GetSpawnPoint()
    {
        return _spawnPoint.position;
    }
}