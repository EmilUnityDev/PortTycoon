using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorkerNode : MonoBehaviour
{
    [SerializeField] protected GameObject _prefab;
    [SerializeField] protected float _workingSpeed = 1f;
    [SerializeField] protected Transform _moneySpawnPoint;

    protected List<GameObject> _workingObjects = new List<GameObject>();
    protected Tutorial _tutorial;
    protected UpgradesController _upgradeController;
    protected MoneyFactory _factory;
    protected ResourcesContainer _resContainer;
    protected LogicNode _logicChain;
    [SerializeField] protected NodeTypes _currentNodeType;

    public virtual void Init(UpgradesController upgradeController, MoneyFactory factory, ResourcesContainer resContainer, Tutorial tutorial)
    {
        _tutorial = tutorial;
        _resContainer = resContainer;
        _logicChain = GetComponent<LogicNode>();
        _logicChain.ApplyWorkerNode(this);
        _factory = factory;
        _upgradeController = upgradeController;
    }

    protected virtual void CreateWorkingObect()
    {

    }

    public virtual GameObject GetContainer()
    {
        return null;
    }

    public virtual void DestroyFirstWorkingObject()
    {
        Destroy(_workingObjects[0]);
        _workingObjects.RemoveAt(0);
    }

    public virtual void ContinueWork()
    {

    }

    protected void SpawnMoney(Transform point)
    {
        int managervalue = _upgradeController.GetCurrentProgressionByType(UpgradeTypes.Manager_ContainerEarning) == 1 ? 0 : (int)_upgradeController.GetProgressionValue(UpgradeTypes.Manager_ContainerEarning);

        int moneyAmount = (int)_upgradeController.GetProgressionValue(UpgradeTypes.ContainerEarning) + managervalue;
        _resContainer.AddMovey(moneyAmount);
        _factory.CreateMoneyParticle(point.position, moneyAmount, Camera.main.transform);
    }

    protected enum NodeTypes
    {
        Start,
        Middle,
        End
    }
}