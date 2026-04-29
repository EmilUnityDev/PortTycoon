using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortCraneWorkerNode : WorkerNode
{
    public Transform CranePosition;
    [SerializeField] private Transform _containerMover;
    [SerializeField] private Transform _containerPlace, _scalingRope;
    [SerializeField] private Transform _crane;
    [SerializeField] private Transform _containerPrefab;
    [SerializeField] private float _ropeScalingSpeed = 3.8f;

    private ShipWorkerNode _shipWorker;
    private GameObject _movingContainer;
    private Vector3 _startCranePosition;
    private Vector3 _startContainerPlacePosition, _startRopeScale;
    private bool _isContinueWork;

    public override void Init(UpgradesController upgradeController, MoneyFactory moneyFactory, ResourcesContainer resContainer, Tutorial tutorial)
    {
        base.Init(upgradeController, moneyFactory, resContainer, tutorial);
        _startContainerPlacePosition = _containerPlace.position;
        _startRopeScale = _scalingRope.localScale;

        StartCoroutine(WorkingSequence());
    }

    private IEnumerator WorkingSequence()
    {
        yield return null;

        _startCranePosition = _containerMover.position;

        _shipWorker = (ShipWorkerNode)_logicChain.NextNode.WorkerNode;

        while (true)
        {
            Vector3Int containerPosition = _shipWorker.GetLastContainerIDPos();

            yield return new WaitWhile(() => !_logicChain.NextNode.IsNodeReady);

            yield return new WaitWhile(() => !_logicChain.PreviousNode.IsNodeReady);

            yield return new WaitWhile(() => _shipWorker.GetContainersAmount() >= _shipWorker.ContainersCapacity);

            float x_offset = (.1f + _containerPrefab.localScale.z) * containerPosition.z + _shipWorker.GetDividerLength();
            float y_offset = (.05f + _containerPrefab.localScale.y) * containerPosition.y;
            float craneOffset = _crane.transform.position.x - _containerMover.position.x;

            float target = _shipWorker.GetHorizontalContainerPoint().x + x_offset;

            bool isMoveLeft = _containerMover.position.x > target;

            while ((isMoveLeft ? _containerMover.position.x : target) > (isMoveLeft ? target : _containerMover.position.x))
            {
                _crane.transform.position += Vector3.right * Time.deltaTime * _workingSpeed * (isMoveLeft ? -1f : 1f) * GetUpgradedCraneSpeed();

                yield return null;
            }

            _crane.transform.position = new Vector3(_shipWorker.GetHorizontalContainerPoint().x + x_offset + craneOffset, _crane.transform.position.y, _crane.transform.position.z);

            _logicChain.IsNodeReady = true;

            yield return new WaitWhile(() => !_isContinueWork);

            _logicChain.IsNodeReady = false;

            _isContinueWork = false;

            yield return StartCoroutine(MoveCraneHorizontally(true));

            float t = 0f;

            Vector3 startPos = _containerPlace.position;
            Vector3 endPos = new Vector3(_containerPlace.transform.position.x, _shipWorker.GetVerticalContainerPoint().y + _containerPrefab.transform.localScale.y / 2f + y_offset, _containerPlace.transform.position.z);

            while (t <= 1f)
            {
                t += Time.deltaTime * _workingSpeed / 2f * GetUpgradedCraneSpeed();

                _containerPlace.transform.position = Vector3.Lerp(startPos, endPos, t);
                _scalingRope.transform.localScale += Vector3.up * Time.deltaTime * _workingSpeed * _ropeScalingSpeed;

                yield return null;
            }

            SpawnMoney(_moneySpawnPoint);

            _shipWorker.AddContainer(_movingContainer);

            yield return StartCoroutine(MoveCrane(null, false));
            _isContinueWork = false;

            yield return new WaitForSeconds(.5f);

            yield return StartCoroutine(MoveCraneHorizontally(false));

            yield return null;
        }
    }

    private IEnumerator MoveCraneHorizontally(bool IsForward)
    {
        Vector3 startPosition = _containerMover.position;
        Vector3 endPosition = IsForward ? new Vector3(
            _containerMover.position.x, 
            _containerMover.position.y, 
            _shipWorker.GetHorizontalContainerPoint().z + (.1f + _containerPrefab.localScale.x) * _shipWorker.GetLastContainerIDPos().x) : 
            new Vector3(_containerMover.position.x, _containerMover.position.y, _startCranePosition.z);

        float t = 0f;

        while (t <= 1f)
        {
            t += Time.deltaTime / 12f * GetUpgradedCraneSpeed();

            _containerMover.position = Vector3.Lerp(startPosition, endPosition, t);

            yield return null;
        }

        _containerMover.position = endPosition;
    }

    public IEnumerator MoveCrane(GameObject container, bool isDown = false)
    {
        _isContinueWork = false;

        _movingContainer = container;

        if (_movingContainer != null)
        {
            _movingContainer.transform.SetParent(null);
        }

        if (isDown)
        {
            float t = 0f;

            Vector3 startPos = new Vector3(_movingContainer.transform.position.x, _startContainerPlacePosition.y, _movingContainer.transform.position.z);
            Vector3 endPos = container.transform.position + Vector3.up * container.transform.localScale.y * .6f;

            while (t <= 1f)
            {
                t += Time.deltaTime * _workingSpeed / 2f * GetUpgradedCraneSpeed();

                _containerPlace.transform.position = Vector3.Lerp(startPos, endPos, t);
                _scalingRope.transform.localScale += Vector3.up * Time.deltaTime * _workingSpeed * _ropeScalingSpeed;

                yield return null;
            }
        }
        else
        {
            if (container != null)
            { 
                container.transform.localEulerAngles = new Vector3(0f, -90f, 0f);
                _movingContainer.transform.SetParent(_containerPlace);
            }

            float t = 0f;

            Vector3 startPos = _containerPlace.transform.position;
            Vector3 endPos = new Vector3(_containerPlace.transform.position.x, _startContainerPlacePosition.y, _containerPlace.transform.position.z);

            while (t <= 1f)
            {
                t += Time.deltaTime * _workingSpeed / 2f * GetUpgradedCraneSpeed();

                _containerPlace.transform.position = Vector3.Lerp(startPos, endPos, t);
                _scalingRope.transform.localScale -= Vector3.up * Time.deltaTime * _workingSpeed * _ropeScalingSpeed;

                yield return null;
            }

            _scalingRope.transform.localScale = _startRopeScale;
        }

        if (isDown)
        {
            StartCoroutine(MoveCrane(container, false));
        }
        else
        {
            _isContinueWork = true;
        }
    }

    private float GetUpgradedCraneSpeed()
    {
        return _upgradeController.GetProgressionValue(UpgradeTypes.CraneSpeed) + (_upgradeController.GetProgressionValue(UpgradeTypes.Manager_CraneSpeed) - 1);
    }
}