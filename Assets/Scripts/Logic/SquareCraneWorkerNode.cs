using Dreamteck.Splines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LogicNode))]
public class SquareCraneWorkerNode : WorkerNode
{
    public int MaxHeight = 3;
    public int ContainersCapacity = 9;

    [SerializeField] private ContainersCollection _containerPrefabs;
    [SerializeField] private Transform _movingCrane, _craneMover;
    [SerializeField] private Transform _startPosition, _endPosition;
    [SerializeField] private Transform _square;
    [SerializeField] private float _newLevelLength = .3f;

    private List<GameObject> _containers = new List<GameObject>();
    private float _startSquareXScale;
    private float _startSquareXPosition;
    private float _startMoverPosition;
    private float _containerScale;
    private float _containerStartYTPosition;
    private float _groundPosition = .15f;

    public override void Init(UpgradesController upgradeController, MoneyFactory moneyFactory, ResourcesContainer resContainer, Tutorial tutorial)
    {
        base.Init(upgradeController, moneyFactory, resContainer, tutorial);
        _startMoverPosition = _craneMover.position.y;
        StartCoroutine(WorkingSequence());

        upgradeController.OnSquareUpgraded += OnSquareUpgraded;

        _startSquareXPosition = _square.transform.localPosition.z;
        _startSquareXScale = _square.transform.localScale.x;

        int containersToCreate = _tutorial.IsStorageSquareBuilt ? UnityEngine.Random.Range(0, 6) : 9;

        for (int i = 0; i < containersToCreate; i++)
        {
            GameObject randomContainer = _containerPrefabs.GetRandomContainerPrefab();
            GameObject container = Instantiate(randomContainer);
            container.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            container.transform.SetParent(_square.transform.parent.parent);
            container.transform.localScale = randomContainer.transform.localScale;

            _containerScale = container.transform.localScale.y * 1.1f;

            _containers.Add(container);
        }

        if (containersToCreate != 0)
        {
            RecalculateContainersPositions();
        }
    }

    private IEnumerator ContainerUpMovement()
    {
        float destination = _startMoverPosition;

        while (_craneMover.position.y < destination)
        {
            _craneMover.position += Vector3.up * Time.deltaTime * 2f * _workingSpeed * GetUpgradedCraneSpeed();

            yield return null;
        }
    }

    public void OnSquareUpgraded()
    {
        _square.transform.localScale = 
            new Vector3(
            _startSquareXScale + _newLevelLength * (_upgradeController.GetProgressionValue(UpgradeTypes.StorageCapacity) - 1), 
            _square.transform.localScale.y, 
            _square.transform.localScale.z);

        _square.transform.localPosition = 
            new Vector3(
                _square.transform.localPosition.x, 
                _square.transform.localPosition.y, 
                _startSquareXPosition + _newLevelLength * (_upgradeController.GetProgressionValue(UpgradeTypes.StorageCapacity) - 1) / 2f);
    }

    private IEnumerator ContainerMovement(Transform container, bool isDownMovement)
    {
        if (!isDownMovement)
        {
            float endYPosition = _containerStartYTPosition - _groundPosition + GetContainerPlaceID().y * _containerScale;
            Vector3 endContainerPosition = new Vector3(_startPosition.position.x, endYPosition, container.transform.position.z);

            while (container.position.y > endContainerPosition.y)
            {
                endYPosition = _containerStartYTPosition - _groundPosition + GetContainerPlaceID().y * _containerScale;
                endContainerPosition = new Vector3(_startPosition.position.x, endYPosition, container.transform.position.z);

                _craneMover.position += Vector3.down * Time.deltaTime * 2f * _workingSpeed * GetUpgradedCraneSpeed();

                yield return null;
            }

            //_craneMover.position = endContainerPosition;
        }
        else
        {
            float destination = container.position.y + container.localScale.y / 2f;

            while (_craneMover.position.y > destination)
            {
                _craneMover.position += Vector3.down * Time.deltaTime * 2f * _workingSpeed * GetUpgradedCraneSpeed();

                yield return null;
            }

            _craneMover.position = new Vector3(_craneMover.position.x, destination, _craneMover.position.z);
        }
    }

    private IEnumerator CraneMovement(GameObject container, bool isForwardMovement)
    {
        Vector3 positionEmptyContainersPlace = _startPosition.position + new Vector3(0f, 0f, GetContainerPlaceID().x * _containerScale);

        float destination = isForwardMovement ? positionEmptyContainersPlace.z : _endPosition.position.z;

        while ((isForwardMovement ? _movingCrane.position.z : destination) < (isForwardMovement ? destination : _movingCrane.position.z))
        {
            if (isForwardMovement)
            {
                positionEmptyContainersPlace = _startPosition.position + new Vector3(0f, 0f, GetContainerPlaceID().x * _containerScale);
                destination = positionEmptyContainersPlace.z;
            }
            else
            {
                destination = _endPosition.position.z;
            }

            Vector3 movementDirection = new Vector3(_movingCrane.position.x, _movingCrane.position.y, destination) - _movingCrane.position;
            movementDirection = new Vector3(0f, 0f, movementDirection.z);
            movementDirection.Normalize();

            _movingCrane.position += movementDirection * Time.deltaTime * _workingSpeed * GetUpgradedCraneSpeed();

            yield return null;
        }

        _movingCrane.position = new Vector3(_movingCrane.position.x, _movingCrane.position.y, destination);
    }

    private IEnumerator WorkingSequence()
    {
        while (true)
        {
            yield return new WaitWhile(() => !_logicChain.PreviousNode.IsNodeReady || !_tutorial.IsStorageSquareBuilt || !_tutorial.IsStorageCraneBuilt);

            _logicChain.IsNodeReady = false;

            GameObject container = _logicChain.PreviousNode.WorkerNode.GetContainer();
            _containerScale = container.transform.localScale.y * .85f;

            _containerStartYTPosition = container.transform.position.y;

            yield return new WaitForSeconds(.5f);

            yield return StartCoroutine(ContainerMovement(container.transform, true));
            container.transform.SetParent(_craneMover);
            yield return StartCoroutine(ContainerUpMovement());

            if (_logicChain.PreviousNode.IsNodeReady)
            {
                _logicChain.PreviousNode.WorkerNode.ContinueWork();
            }

            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(CraneMovement(container, true));

            yield return StartCoroutine(ContainerMovement(container.transform, false));
            SpawnMoney(_moneySpawnPoint);
            container.transform.SetParent(null);
            _containers.Add(container);
            RecalculateContainersPositions();
            yield return StartCoroutine(ContainerUpMovement());

            yield return StartCoroutine(CraneMovement(container, false));

            _logicChain.IsNodeReady = true;

            yield return new WaitWhile(() => _containers.Count >= ContainersCapacity + (((int)_upgradeController.GetProgressionValue(UpgradeTypes.StorageCapacity) - 1) * 3));
        }
    }

    private void RecalculateContainersPositions()
    {
        for (int i = 0; i < _containers.Count; i++)
        {
            Vector3 containerPos = _startPosition.position + new Vector3(0f,
                _groundPosition + GetContainerPlaceID(i).y * _containerScale, 
                GetContainerPlaceID(i).x * _containerScale);

            _containers[i].transform.position = containerPos;
        }
    }

    public void RemoveContainer(GameObject container)
    {
        _containers.Remove(container);
    }

    public List<GameObject> GetContainers()
    {
        return _containers;
    }

    public Vector2Int GetContainerPlaceID()
    {
        int containerZID = _containers.Count / MaxHeight;
        int containerYID = _containers.Count % MaxHeight;

        return new Vector2Int(containerZID, containerYID);
    }

    public Vector2Int GetContainerPlaceID(int id)
    {
        int containerZID = id / MaxHeight;
        int containerYID = id % MaxHeight;

        return new Vector2Int(containerZID, containerYID);
    }

    private float GetUpgradedCraneSpeed()
    {
        return _upgradeController.GetProgressionValue(UpgradeTypes.StorageCraneSpeed) + (_upgradeController.GetProgressionValue(UpgradeTypes.Manager_StorageCraneSpeed) - 1);
    }
}