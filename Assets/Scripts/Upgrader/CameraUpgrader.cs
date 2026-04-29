using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUpgrader : MonoBehaviour
{
    [SerializeField] private LayerMask _layer;
    [SerializeField] private float _maxDistanceToAction;

    private Camera _camera;
    private UI _ui;
    private UpgraderRaycastTouch _upgrader;
    private Vector2 _startInputPosition;
    private bool _isTwoTouched;

    public void Init(Camera camera, UI ui)
    {
        _camera = camera;
        _ui = ui;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _ui != null && _ui.GetCurrentPanelType() == Panels.HudPanel)
        {
            _startInputPosition = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
        }

        if (Input.GetMouseButtonUp(0) && _ui.GetCurrentPanelType() == Panels.HudPanel)
        {
            _upgrader = GetTouchUpgrader(Input.mousePosition);

            Vector2 distance = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height) - _startInputPosition;

            bool isInTouchRange = distance.magnitude < _maxDistanceToAction;

            if (_upgrader != null && isInTouchRange && !_isTwoTouched)
            {
                _upgrader.OpenUpgrader();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            _isTwoTouched = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            _isTwoTouched = false;
        }
    }

    private UpgraderRaycastTouch GetTouchUpgrader(Vector3 inputPosition)
    {
        Ray cameraTouchRay = _camera.ScreenPointToRay(inputPosition);
        RaycastHit hit;
        Physics.Raycast(cameraTouchRay, out hit, Mathf.Infinity, _layer);

        if (hit.transform == null)
        {
            return null;
        }

        return hit.transform.GetComponent<UpgraderRaycastTouch>();
    }
}
