using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipWorkerNode : WorkerNode
{
    [SerializeField] private ContainersCollection _containerPrefabs;
    [SerializeField] private List<GameObject> _ships;
    [SerializeField] private Transform _shipStartPosition, _shipLoadPosition, _shipEndPosition;

    private List<GameObject> _containers = new List<GameObject>();
    private GameObject _currentShip;
    private Transform _horizontalContainerPoint, _verticalContainerPoint;
    private CrystallsStorage _crStorage;
    private int _currentShipLevel;

    public int ContainersCapacity
    {
        get => 3 * (int)Mathf.Pow(2, _currentShipLevel + 1);
    }

    private bool _isFullyLoaded;
    private bool _isCanCreateNewShip;

    public override void Init(UpgradesController upgradeController, MoneyFactory moneyFactory, ResourcesContainer resContainer, Tutorial tutorial)
    {
        base.Init(upgradeController, moneyFactory, resContainer, tutorial);
        StartCoroutine(WorkingSequence());
    }

    public void InitCrystalFactory(CrystallsStorage crStorage)
    {
        _crStorage = crStorage;
    }

    private void CreateTutorialContainers()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject newContainer = Instantiate(_containerPrefabs.GetRandomContainerPrefab());

            newContainer.transform.position = new Vector3(
                _horizontalContainerPoint.position.x,
                _verticalContainerPoint.position.y - .15f,
                _horizontalContainerPoint.position.z + .9f * i);

            AddContainer(newContainer);

            newContainer.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        }
    }

    private IEnumerator WorkingSequence()
    {
        while (true)
        {
            _currentShipLevel = (int)_upgradeController.GetProgressionValue(UpgradeTypes.ShipSize) - 1;
            GameObject ship = Instantiate(_ships[_currentShipLevel]);

            _horizontalContainerPoint = ship.GetComponent<Ship>().HorizontalPoint;
            _verticalContainerPoint = ship.GetComponent<Ship>().VerticalPoint;

            ship.transform.position = _shipStartPosition.position;
            _currentShip = ship;

            if (!_tutorial.IsBuiltPortCrane)
            {
                ship.transform.position -= Vector3.right * 50f;

                CreateTutorialContainers();
            }

            StartCoroutine(ShipMovement(ship));

            yield return new WaitWhile(() => !_isCanCreateNewShip);

            _containers.Clear();
            _isFullyLoaded = false;
            _isCanCreateNewShip = false;
        }
    }

    private IEnumerator ShipMovement(GameObject ship)
    {
        bool throwed = false;

        while (true)
        {
            if (!_isFullyLoaded && ship.transform.position.x < _shipLoadPosition.position.x)
            {
                _logicChain.IsNodeReady = true;

                yield return new WaitWhile(() => !_isFullyLoaded);

                yield return new WaitForSeconds(1f);
            }

            if (_isFullyLoaded && ship.transform.position.x < _shipEndPosition.position.x)
            {
                _isCanCreateNewShip = true;
                Destroy(ship);
                yield break;
            }

            if (!_isFullyLoaded && !_logicChain.IsNodeReady)
            {
                ship.transform.position -= Vector3.right * Time.deltaTime * _workingSpeed;
            }

            if (_isFullyLoaded && !_logicChain.IsNodeReady)
            {
                ship.transform.position -= Vector3.right * Time.deltaTime * _workingSpeed;

                if (!throwed && ship.transform.position.x < _crStorage.GetSpawnPoint().x)
                {
                    throwed = true;

                    _crStorage.Spawn((int)_upgradeController.GetProgressionValue(UpgradeTypes.DiamondBonus) + (int)_upgradeController.GetProgressionValue(UpgradeTypes.Manager_DiamondEarning));
                }
            }

            yield return null;
        }
    }

    public void AddContainer(GameObject newContainer)
    {
        LoadToShip(newContainer);
        _containers.Add(newContainer);

        if (_containers.Count == ContainersCapacity)
        {
            _logicChain.IsNodeReady = false;
            _isFullyLoaded = true;
        }
    }

    public Vector3Int GetLastContainerIDPos()
    {
        int rows = (int)Mathf.Pow(2, _currentShipLevel);

        int x = 2 - _containers.Count % 3;
        int y = (_containers.Count / (3 * rows)) % 2;
        int z = (_containers.Count / 3) % rows;

        return new Vector3Int(x, y, z);
    }

    public float GetDividerLength()
    {
        return 1.2f * ((_containers.Count / 6) % 2) * ((_currentShipLevel + 1) < 3 ? 0f : 1f);
    }

    private void LoadToShip(GameObject container)
    {
        container.transform.SetParent(_currentShip.transform);
        container.transform.rotation = Quaternion.Euler(0f, container.transform.eulerAngles.y, 0f);
    }

    public int GetContainersAmount()
    {
        return _containers.Count;
    }

    public Vector3 GetHorizontalContainerPoint()
    {
        return _horizontalContainerPoint.position;
    }

    public Vector3 GetVerticalContainerPoint()
    {
        return _verticalContainerPoint.position;
    }
}
