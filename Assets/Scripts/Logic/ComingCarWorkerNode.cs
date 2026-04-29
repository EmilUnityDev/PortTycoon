using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LogicNode))]
public class ComingCarWorkerNode : WorkerNode
{
    [SerializeField] private int _carsAmount;
    [SerializeField] protected SplineComputer _spline;
    [SerializeField] protected float _readySplinePosition = .5f;
    [SerializeField] protected float _notReadySplinePosition = .4f;
    [SerializeField] protected float _barrierSplinePosition = .1f;
    [SerializeField] protected float _splineQueueOffset = .05f;
    [SerializeField] protected float _timeBeforeStart = 5f;
    [SerializeField] protected float _timeOnBarrier = 5f;
    [SerializeField] private Transform _barrier;
    [SerializeField] private Barrier _barrierTrigger;

    private List<QueueSubject> _queue = new List<QueueSubject>();
    private GameObject _fisrtObjProcessingObj;

    private bool _isCanProcess;

    public override void Init(UpgradesController upgradeController, MoneyFactory moneyFactory, ResourcesContainer resContainer, Tutorial tutorial)
    {
        base.Init(upgradeController, moneyFactory, resContainer, tutorial);
        StartCoroutine(WorkingSequence());
    }

    private IEnumerator WorkingSequence()
    {
        if (!_tutorial.IsGarageBuilt)
        {
            CreateWorkingObect();

            StartCoroutine(ObjectMovement(
                _workingObjects[_workingObjects.Count - 1],
                _workingObjects[_workingObjects.Count - 1].GetComponent<Car>(),
                _queue[_queue.Count - 1], 
                null, 
                true));
        }

        while (true)
        {
            if (!(_currentNodeType == NodeTypes.Start))
            {
                yield return new WaitWhile(() => _logicChain.PreviousNode != null && !_logicChain.PreviousNode.IsNodeReady);
            }

            yield return new WaitWhile(() => _workingObjects.Count >= _carsAmount + _upgradeController.GetProgressionValue(UpgradeTypes.OneMoreCar) - 1);

            if (_queue.Count > 0)
            {
                yield return new WaitWhile(() => (_queue[_queue.Count - 1].StoppedPosition - _splineQueueOffset) <= 0f);
            }

            CreateWorkingObect();

            StartCoroutine(ObjectMovement(_workingObjects[_workingObjects.Count - 1], _workingObjects[_workingObjects.Count - 1].GetComponent<Car>(), _queue[_queue.Count - 1], _workingObjects.Count > 1 ? _queue[_queue.Count - 2] : null));

            yield return new WaitForSeconds(_timeBeforeStart);
        }
    }

    private IEnumerator BarrierRotation()
    {
        float t = 0f;

        while (t <= 1f)
        {
            t += Time.deltaTime * _workingSpeed * 300f;

            _barrier.transform.localRotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(0f, 0f, -90f), t);

            yield return null;
        }

        yield return new WaitWhile(() => _barrierTrigger.IsHaveTrigger);

        while (t > 0f)
        {
            t -= Time.deltaTime * _workingSpeed * 300f;

            _barrier.transform.localRotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(0f, 0f, -90f), t);

            yield return null;
        }
    }

    private IEnumerator ObjectMovement(GameObject objToMove, Car car, QueueSubject subj, QueueSubject nextSubj, bool isTutorialCar = false)
    {
        float t = 0f;

        bool isStopped = false;
        bool isEntered = false;

        if (isTutorialCar)
        {
            t = _notReadySplinePosition;
            objToMove.transform.position = _spline.Evaluate(t).position;
            objToMove.transform.rotation = _spline.Evaluate(t).rotation;
            isEntered = true;
        }

        while (t <= 1f)
        {
            subj.StoppedPosition = t;

            if (t >= _barrierSplinePosition && !isEntered)
            {
                yield return new WaitWhile(() => !_tutorial.IsGarageBuilt);

                isEntered = true;

                yield return new WaitForSeconds(_timeOnBarrier - GetPostCheckSpeed());

                SpawnMoney(_moneySpawnPoint);

                StopCoroutine("BarrierRotation");
                StartCoroutine("BarrierRotation");
            }

            if (car.HasAnotherCarInRange)
            {
                yield return new WaitWhile(() => car.HasAnotherCarInRange);
                yield return new WaitForSeconds(1f);
            }
            else
            {
                t += Time.deltaTime * _workingSpeed * GetCarSpeed();
            }

            if (_logicChain.NextNode.IsNodeReady && _tutorial.IsStorageCraneBuilt)
            {
                if (t >= _readySplinePosition && !isStopped)
                {
                    _fisrtObjProcessingObj = objToMove;

                    _logicChain.IsNodeReady = true;
                    isStopped = true;

                    yield return new WaitWhile(() => !_isCanProcess);

                    _logicChain.IsNodeReady = false;
                    _isCanProcess = false;
                }
            }
            else
            {
                if (t >= _notReadySplinePosition && !isStopped)
                {
                    yield return new WaitWhile(() => !_logicChain.NextNode.IsNodeReady || !_tutorial.IsStorageCraneBuilt);
                }
            }

            objToMove.transform.position = _spline.Evaluate(t).position;
            objToMove.transform.rotation = _spline.Evaluate(t).rotation;

            yield return null;
        }

        DestroyFirstWorkingObject();
    }

    public override GameObject GetContainer()
    {
        return _fisrtObjProcessingObj.transform.GetChild(1).gameObject;
    }

    public override void ContinueWork()
    {
        base.ContinueWork();
        _isCanProcess = true;
    }

    public override void DestroyFirstWorkingObject()
    {
        _queue[0] = null;
        _queue.RemoveAt(0);
        base.DestroyFirstWorkingObject();
    }

    protected override void CreateWorkingObect()
    {
        GameObject newObj = Instantiate(_prefab);
        newObj.transform.position = _spline.EvaluatePosition(0);
        _workingObjects.Add(newObj);
        QueueSubject subj = new QueueSubject();
        subj.Obj = newObj;
        _queue.Add(subj);
    }

    private float GetCarSpeed()
    {
        return _upgradeController.GetProgressionValue(UpgradeTypes.CarSpeed) + (_upgradeController.GetProgressionValue(UpgradeTypes.Manager_CarSpeed) - 1);
    }

    private float GetPostCheckSpeed()
    {
        return _upgradeController.GetProgressionValue(UpgradeTypes.CheckPostSpeed) + (_upgradeController.GetProgressionValue(UpgradeTypes.Manager_CheckPostSpeed) - 1);
    }
}

[System.Serializable]
public class QueueSubject
{
    public GameObject Obj;
    public float StoppedPosition;
}