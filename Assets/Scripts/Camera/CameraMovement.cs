using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CnControls;

public class CameraMovement : MonoBehaviour
{
    private const string VERTICAL_AXIS = "Vertical";
    private const string HORIZONTAL_AXIS = "Horizontal";

    [SerializeField] private BoxCollider _cameraBounds;
    [SerializeField] private float _movementSpeed = 40f, _minMovementSpeed = 20f;
    [Range(.1f, 1f), SerializeField] private float _cameraMaxSizeOffset = .6f;
    [SerializeField] private float _camerSizeChangingSpeed = 25f;
    [SerializeField] private float _cameraSnappingSpeed = 5f;
    [SerializeField] private LayerMask _groundMask;

    private GameObject _cameraOnPlaneCenterPoint;
    private Transform _snappingPoint;
    private Camera _camera;
    private Vector3 _offsetToCheckPosition;
    private Vector2 _offset;
    private float _startMovementSpeed;
    private float _magnitudeOnStart;
    private float _minCameraSize, _maxCameraSize;
    private float _startCameraSize;
    private float _cameraSizeOnScaling;
    private bool _isZooming;
    private bool _isDoubleTouched;
    private bool _isSnapping, _isTutorialSnapping;

    private void Start()
    {
        _camera = GetComponent<Camera>();

        _startCameraSize = _camera.orthographicSize;
        _cameraSizeOnScaling = _startCameraSize;
        _minCameraSize = _startCameraSize - _startCameraSize * _cameraMaxSizeOffset;
        _maxCameraSize = _startCameraSize + _startCameraSize * _cameraMaxSizeOffset;

        RaycastHit outHit;
        Physics.Raycast(_camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f)), out outHit, Mathf.Infinity, _groundMask);

        _cameraOnPlaneCenterPoint = new GameObject();

        _cameraOnPlaneCenterPoint.transform.position = outHit.point;
        _offsetToCheckPosition = outHit.point - transform.position;
        _startMovementSpeed = _movementSpeed;
    }

    public void StartTutorialSnap(Transform cameraSnapPoint)
    {
        _isTutorialSnapping = true;
        _snappingPoint = cameraSnapPoint;
    }

    public void StartSnap(Transform _cameraSnapPoint)
    {
        _isSnapping = true;
        _snappingPoint = _cameraSnapPoint;
    }

    public void StopSnap()
    {
        _isSnapping = false;
    }

    private void Update()
    {
        if (_isSnapping)
        {
            _cameraOnPlaneCenterPoint.transform.position = Vector3.Lerp(
                _cameraOnPlaneCenterPoint.transform.position, 
                new Vector3(_snappingPoint.position.x - _offsetToCheckPosition.x / 5f, _cameraOnPlaneCenterPoint.transform.position.y, _snappingPoint.position.z - _offsetToCheckPosition.z / 5f),
                _cameraSnappingSpeed * Time.deltaTime);
        }

        if (_isTutorialSnapping)
        {
            _cameraOnPlaneCenterPoint.transform.position = Vector3.Lerp(
                _cameraOnPlaneCenterPoint.transform.position,
                new Vector3(_snappingPoint.position.x, _cameraOnPlaneCenterPoint.transform.position.y, _snappingPoint.position.z),
                _cameraSnappingSpeed * Time.deltaTime);

            Vector3 direction = (_snappingPoint.position - _cameraOnPlaneCenterPoint.transform.position);
            direction.y = 0f;
            float distance = direction.magnitude;

            if (distance <= 2f)
            {
                _isTutorialSnapping = false;
            }
        }

        if (Input.touches.Length == 2 && !_isDoubleTouched)
        {
            _isDoubleTouched = true;
            _isZooming = true;
            _magnitudeOnStart = GetTouchesMagnitude();
        }

        if (Input.touches.Length < 2 && _isDoubleTouched)
        {
            _isDoubleTouched = false;
            _isZooming = false;
            _cameraSizeOnScaling = _camera.orthographicSize;
        }

        if (_isZooming)
        {
            float magnitude = GetTouchesMagnitude();
            float magnitudeDifference = (_magnitudeOnStart - magnitude);
            float newSize = _cameraSizeOnScaling + magnitudeDifference * _camerSizeChangingSpeed;

            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, Mathf.Clamp(newSize, _minCameraSize, _maxCameraSize), 2.5f * Time.deltaTime);

            _movementSpeed = Mathf.Lerp(_minMovementSpeed, _startMovementSpeed, Mathf.InverseLerp(_minCameraSize, _maxCameraSize, newSize));
        }
        else
        {
            _offset = new Vector2(CnInputManager.GetAxis(HORIZONTAL_AXIS) / Screen.width, CnInputManager.GetAxis(VERTICAL_AXIS) / Screen.height);

            _cameraOnPlaneCenterPoint.transform.position -= 
                transform.rotation * 
                Quaternion.Euler(-transform.rotation.eulerAngles.x, 0f, 0f) * 
                new Vector3(_offset.x / 1.5f, 0f, _offset.y) * 
                _movementSpeed;

            float x = Mathf.Clamp(_cameraOnPlaneCenterPoint.transform.position.x, _cameraBounds.bounds.min.x, _cameraBounds.bounds.max.x);
            float y = Mathf.Clamp(_cameraOnPlaneCenterPoint.transform.position.y, _cameraBounds.bounds.min.y, _cameraBounds.bounds.max.y);
            float z = Mathf.Clamp(_cameraOnPlaneCenterPoint.transform.position.z, _cameraBounds.bounds.min.z, _cameraBounds.bounds.max.z);

            _cameraOnPlaneCenterPoint.transform.position = new Vector3(x, y, z);
            transform.position = _cameraOnPlaneCenterPoint.transform.position - _offsetToCheckPosition;
        }
    }

    private float GetTouchesMagnitude()
    {
        float xOffset = (Input.touches[1].position - Input.touches[0].position).x / Screen.width;
        float yOffset = (Input.touches[1].position - Input.touches[0].position).y / Screen.height;

        return new Vector2(xOffset, yOffset).magnitude;
    }
}