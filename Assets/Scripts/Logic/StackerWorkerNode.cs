using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackerWorkerNode : WorkerNode
{
    [SerializeField] private Transform _placer;
    [SerializeField] private Transform _groundPoint, _endPoint;
    [SerializeField] private Transform _verticalRotator, _horizontalRotator;
    [SerializeField] private Transform _mover;
    [SerializeField] private float[] _containersRotationTarget;
    [SerializeField] private float _defaultPlacerRotation;

    private SquareCraneWorkerNode _squareCraneWorkerNode;
    private GameObject _nearest, _pickedUp;
    private Vector3 _startPoint;

    public override void Init(UpgradesController upgradeController, MoneyFactory moneyFactory, ResourcesContainer resContainer, Tutorial tutorial)
    {
        base.Init(upgradeController, moneyFactory, resContainer, tutorial);
        _startPoint = _mover.position;

        StartCoroutine(WorkingSequence());
    }

    private IEnumerator NearestFinding()
    {
        while (true)
        {
            if (_squareCraneWorkerNode.GetContainers().Count > 0)
            {
                _nearest = _squareCraneWorkerNode.GetContainers()[_squareCraneWorkerNode.GetContainers().Count - 1];
            }
            else
            {
                _nearest = null;
            }

            yield return null;
        }
    }

    private IEnumerator WorkingSequence()
    {
        yield return null;

        _squareCraneWorkerNode = (SquareCraneWorkerNode)_logicChain.PreviousNode.WorkerNode;

        StartCoroutine(NearestFinding());
        StartCoroutine(RotateHorizontal(0f));

        while (true)
        {
            yield return new WaitWhile(() => _squareCraneWorkerNode.GetContainers().Count == 0 || !_tutorial.IsStorageSquareBuilt);

            while ((_placer.position.z - 1.6f) >= _nearest.transform.position.z)
            {
                yield return null;

                _mover.position -= Vector3.forward * Time.deltaTime * _workingSpeed * GetStackerSpeed();
            }

            yield return StartCoroutine(RotatePlacer(GetRotationAngle(), true));

            _pickedUp = _nearest;
            _squareCraneWorkerNode.RemoveContainer(_nearest);
            _nearest.transform.SetParent(_placer);
            _pickedUp.transform.eulerAngles = new Vector3(0f, _pickedUp.transform.eulerAngles.y, 0f);
            _pickedUp.transform.localPosition = new Vector3(_pickedUp.transform.localPosition.x, _pickedUp.transform.localPosition.y, 0f);

            yield return StartCoroutine(RotatePlacer(_defaultPlacerRotation));

            StartCoroutine(RotateHorizontal(180f));

            while ((_placer.position.z + 1.6f) <= _endPoint.transform.position.z)
            {
                _mover.position += Vector3.forward * Time.deltaTime * _workingSpeed * GetStackerSpeed();

                yield return null;
            }

            _logicChain.IsNodeReady = true;

            yield return new WaitWhile(() => _logicChain.IsNodeReady);

            StartCoroutine(RotateHorizontal(0f));
            StartCoroutine(RotatePlacer(_defaultPlacerRotation));

            while (_mover.position.z >= _startPoint.z)
            {
                yield return null;

                _mover.position -= Vector3.forward * Time.deltaTime * _workingSpeed * GetStackerSpeed();
            }
        }
    }

    private float GetRotationAngle()
    {
        Vector2Int containerPlaceID = _squareCraneWorkerNode.GetContainerPlaceID();
        int result = 0;

        switch (containerPlaceID.y)
        {
            case 2:
                result = 1;
                break;
            case 0:
                result = 2;
                break;
        }

        return _containersRotationTarget[result];
    }

    private IEnumerator RotateHorizontal(float target)
    {
        float t = 0f;

        Quaternion startRotation = Quaternion.Euler(0f, _horizontalRotator.transform.localEulerAngles.y, 0f);
        Quaternion endRotation = Quaternion.Euler(0f, target, 0f);

        while (t <= 1f)
        {
            t += Time.deltaTime * _workingSpeed / 2f * GetStackerSpeed();

            _horizontalRotator.transform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);

            yield return null;
        }
    }

    public override void ContinueWork()
    {
        base.ContinueWork();
        _logicChain.IsNodeReady = false;
    }

    public GameObject PlaceContainer()
    {
        return _pickedUp;
    }

    public IEnumerator RotatePlacer(int targetID, bool isAlwaysRefreshTarget = false)
    {
        float target = _containersRotationTarget[targetID];
        float t = 0f;

        Quaternion startRotation = Quaternion.Euler(_verticalRotator.transform.localEulerAngles.x, -180f, 0f);
        Quaternion endRotation = Quaternion.Euler(target, -180f, 0f);

        while (t <= 1f)
        {
            if (isAlwaysRefreshTarget)
            {
                endRotation = Quaternion.Euler(GetRotationAngle(), -180f, 0f);
            }

            t += Time.deltaTime * _workingSpeed * GetStackerSpeed();

            _verticalRotator.transform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);

            yield return null;
        }
    }

    public IEnumerator RotatePlacer(float target, bool isAlwaysRefreshTarget = false)
    {
        float t = 0f;

        Quaternion startRotation = Quaternion.Euler(_verticalRotator.transform.localEulerAngles.x, -180f, 0f);
        Quaternion endRotation = Quaternion.Euler(target, -180f, 0f);

        while (t <= 1f)
        {
            if (isAlwaysRefreshTarget)
            {
                endRotation = Quaternion.Euler(GetRotationAngle(), -180f, 0f);
            }

            t += Time.deltaTime * _workingSpeed * GetStackerSpeed();

            _verticalRotator.transform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);

            yield return null;
        }
    }

    private float GetStackerSpeed()
    {
        return _upgradeController.GetProgressionValue(UpgradeTypes.ReachStackerSpeed) + (_upgradeController.GetProgressionValue(UpgradeTypes.Manager_ReachStackerSpeed) - 1);
    }
}