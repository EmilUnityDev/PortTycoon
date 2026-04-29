using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortRoadWorkerNode : WorkerNode
{
    [SerializeField] private List<GameObject> _carts;
    [SerializeField] private ContainersCollection _containerPrefabs;
    [SerializeField] private GameObject _cartPrefab;
    [SerializeField] private Transform _newCartsSpawnPoint;
    [SerializeField] private SplineComputer _spline;
    [SerializeField] private float _cranePoint, _squareMoverPoint;

    public override void Init(UpgradesController upgradeController, MoneyFactory moneyFactory, ResourcesContainer resContainer, Tutorial tutorial)
    {
        base.Init(upgradeController, moneyFactory, resContainer, tutorial);
        StartCoroutine(WorkingSequence());
        StartCoroutine(CartsUpgradingEvent());
    }

    private void CreateNewCart()
    {
        GameObject newCart = Instantiate(_cartPrefab);
        newCart.transform.position = _newCartsSpawnPoint.position;
        newCart.transform.SetParent(_carts[0].transform.parent);
        newCart.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);

        CartQueueData queueData = new CartQueueData();
        queueData.Obj = newCart;
        _carts.Add(newCart);

        StartCoroutine(CartMovement(newCart, newCart.GetComponent<Car>(), queueData, 0f, false));
    }

    private IEnumerator CartsUpgradingEvent()
    {
        while (true)
        {
            while ((int)_upgradeController.GetProgressionValue(UpgradeTypes.OneMoreCart) - 1 <= _carts.Count - 3)
            {
                yield return null;
            }

            CreateNewCart();

            yield return new WaitForSeconds(3f);
        }
    }

    private IEnumerator WorkingSequence()
    {
        if (_tutorial.IsBuiltPortCrane)
        {
            for (int i = 0; i < _carts.Count; i++)
            {
                CartQueueData queueData = new CartQueueData();
                queueData.Obj = _carts[i];
                StartCoroutine(CartMovement(_carts[i], _carts[i].GetComponent<Car>(), queueData, 1f / ((float)i + 1), true));
            }
        }
        else
        {
            for (int i = 0; i < _carts.Count; i++)
            {
                CartQueueData queueData = new CartQueueData();
                queueData.Obj = _carts[i];

                GameObject randomContainerPrefab = _containerPrefabs.GetRandomContainerPrefab();

                GameObject container = Instantiate(randomContainerPrefab);
                queueData.IsHaveContainer = true;

                container.transform.SetParent(_carts[i].transform.GetChild(0));
                container.transform.localPosition = Vector3.zero;
                container.transform.localRotation = Quaternion.identity;
                container.transform.localScale = new Vector3(
                    randomContainerPrefab.transform.localScale.x / _carts[0].transform.localScale.x,
                    randomContainerPrefab.transform.localScale.y / _carts[0].transform.localScale.y,
                    randomContainerPrefab.transform.localScale.z / _carts[0].transform.localScale.z);

                StartCoroutine(CartMovement(_carts[i], _carts[i].GetComponent<Car>(), queueData, .1f - i * .06f, true));
            }
        }

        yield return null;
    }

    private IEnumerator CartMovement(GameObject cart, Car car, CartQueueData queueData, float startSplinePosition, bool isInCircle)
    {
        Vector3 outCircleDestination = _spline.EvaluatePosition(.685f);

        float t = startSplinePosition;

        while (true)
        {
            if (isInCircle)
            {
                while (t <= 1f)
                {
                    queueData.StoppedPosition = t;

                    t += Time.deltaTime * _workingSpeed * _upgradeController.GetProgressionValue(UpgradeTypes.CartSpeed);

                    cart.transform.position = _spline.Evaluate(t).position;
                    cart.transform.rotation = _spline.Evaluate(t).rotation;

                    float moverPointDistance = Mathf.Abs(t - _squareMoverPoint);
                    float portCraneDistance = Mathf.Abs(t - _cranePoint);

                    if (car.HasAnotherCarInRange)
                    {
                        yield return new WaitWhile(() => car.HasAnotherCarInRange);
                    }

                    if (moverPointDistance < .01f && !queueData.IsHaveContainer)
                    {
                        queueData.IsStopped = true;

                        yield return new WaitWhile(() => !_logicChain.PreviousNode.IsNodeReady);

                        yield return StartCoroutine(((StackerWorkerNode)_logicChain.PreviousNode.WorkerNode).RotatePlacer(-75f));

                        GameObject container = ((StackerWorkerNode)_logicChain.PreviousNode.WorkerNode).PlaceContainer();

                        container.transform.SetParent(cart.transform.GetChild(0));
                        container.transform.localPosition = Vector3.zero;
                        container.transform.localRotation = Quaternion.identity;

                        _logicChain.PreviousNode.WorkerNode.ContinueWork();

                        yield return new WaitForSeconds(.5f);

                        queueData.IsHaveContainer = true;
                        queueData.IsStopped = false;
                    }

                    if (portCraneDistance < .01f && (queueData.IsHaveContainer || !_tutorial.IsBuiltPortCrane))
                    {
                        _logicChain.IsNodeReady = true;

                        yield return new WaitWhile(() => !_logicChain.NextNode.IsNodeReady || !_tutorial.IsBuiltPortCrane);

                        _logicChain.NextNode.IsNodeReady = false;

                        PortCraneWorkerNode crane = _logicChain.NextNode.WorkerNode as PortCraneWorkerNode;

                        while (true)
                        {
                            if (cart.transform.position.x > crane.CranePosition.position.x)
                            {
                                break;
                            }

                            t += Time.deltaTime * _workingSpeed * _upgradeController.GetProgressionValue(UpgradeTypes.CartSpeed);

                            cart.transform.position = _spline.Evaluate(t).position;
                            cart.transform.rotation = _spline.Evaluate(t).rotation;

                            yield return null;
                        }

                        cart.transform.position = new Vector3(crane.CranePosition.position.x, cart.transform.position.y, cart.transform.position.z);

                        yield return StartCoroutine(crane.MoveCrane(cart.transform.GetChild(0).GetChild(0).gameObject, true));

                        queueData.IsStopped = true;

                        yield return new WaitForSeconds(.5f);

                        queueData.IsHaveContainer = false;
                        queueData.IsStopped = false;
                    }

                    yield return null;
                }

                t = 0f;

            }
            else
            {
                while (cart.transform.position.x > outCircleDestination.x)
                {
                    if (car.HasAnotherCarInRange)
                    {
                        yield return new WaitWhile(() => car.HasAnotherCarInRange);
                    }

                    cart.transform.position -= Vector3.right * Time.deltaTime * _workingSpeed * 70f * GetCartSpeed();

                    yield return null;
                }

                t = .685f;

                isInCircle = true;
            }
        }
    }

    private float GetCartSpeed()
    {
        return _upgradeController.GetProgressionValue(UpgradeTypes.CartSpeed) + (_upgradeController.GetProgressionValue(UpgradeTypes.Manager_CartSpeed) - 1f);
    }

    [System.Serializable]
    public class CartQueueData
    {
        public GameObject Obj;
        public Car CarContainer;
        public GameObject ContainerObj;
        public float StoppedPosition;
        public bool IsHaveContainer;
        public bool IsStopped;
    }
}