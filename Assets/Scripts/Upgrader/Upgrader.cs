using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrader : MonoBehaviour, IUIOpener
{
    [SerializeField] private Upgraders _type;
    [SerializeField] private Transform _objectToAnimate, _cameraSnapPoint;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private AnimationCurve _elasticScaleCurveHorizontal, _elasticScaleCurveVertical;
    [SerializeField] private float _animationSpeed = 2f;
    [SerializeField] private float _animationPower;

    private Tutorial _tutorial;
    private UI _ui;
    private MoneyFactory _moneyFactory;
    private ResourcesContainer _resContainer;
    private Vector3 _startObjectScale, _startObjectPosition;
    private int _tappes;
    private bool _isPlayerAnimation;

    public void Init(UI ui, MoneyFactory moneyFactory, ResourcesContainer resContainer, Tutorial tutorial)
    {
        _tutorial = tutorial;

        if (_objectToAnimate)
        {
            _startObjectScale = _objectToAnimate.localScale;
            _startObjectPosition = _objectToAnimate.position;
        }

        _ui = ui;
        _moneyFactory = moneyFactory;
        _resContainer = resContainer;

        GetComponentInChildren<UpgraderRaycastTouch>().Init(this);
    }

    public void OpenUI()
    {
        if (_type == Upgraders.Bank)
        {
            _resContainer.AddMovey(10);
            _moneyFactory.CreateMoneyParticle(_spawnPoint.position, 10, Camera.main.transform);
            StartCoroutine(ScaleEvent());
            
            if (_tutorial.IsTapOnBank)
            {
                _tappes++;

                if (_tappes == 3)
                {
                    _tutorial.StopTapping();
                }
            }
        }
        else if (_type == Upgraders.Crystalls)
        {
            if (_objectToAnimate.GetComponent<CrystallsStorage>().GetAllCrystalls())
            {
                StartCoroutine(ScaleEvent());
            }
        }
        else
        {
            Camera.main.GetComponent<CameraMovement>().StartSnap(_cameraSnapPoint);

            _ui.OpenUpgradePanel(_type);
            StartCoroutine(ScaleEvent());
        }
    }

    private IEnumerator ScaleEvent()
    {
        if (_isPlayerAnimation)
        {
            yield break;
        }

        _isPlayerAnimation = true;

        float t = 0f;

        while (t <= 1f)
        {
            t += Time.deltaTime * _animationSpeed;

            _objectToAnimate.transform.localScale = new Vector3(
                _startObjectScale.x * (_animationPower * _elasticScaleCurveHorizontal.Evaluate(t)), 
                _startObjectScale.y * (_animationPower * _elasticScaleCurveVertical.Evaluate(t)), 
                _startObjectScale.z * (_animationPower * _elasticScaleCurveHorizontal.Evaluate(t)));

            _objectToAnimate.position = new Vector3(_objectToAnimate.position.x, _startObjectPosition.y * (_animationPower * _elasticScaleCurveHorizontal.Evaluate(t)), _objectToAnimate.position.z);

            yield return null;
        }

        _isPlayerAnimation = false;
    }
}

public enum Upgraders
{
    None, Bank, Diamonds, Square, Cars, PortCrane, Crystalls
}