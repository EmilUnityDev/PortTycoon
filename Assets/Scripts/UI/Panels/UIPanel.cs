using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public abstract class UIPanel : MonoBehaviour
{
    protected Canvas _canvas;
    protected GraphicRaycaster _raycaster;
    protected UI _ui;
    protected UpgradesController _upgradesController;
    protected ResourcesContainer _resourcesContainer;

    public void Init(UI ui, UpgradesController upgradesController, ResourcesContainer resourcesContainer)
    {
        _resourcesContainer = resourcesContainer;
        _upgradesController = upgradesController;
        _ui = ui;
        _canvas = GetComponent<Canvas>();
        _raycaster = GetComponent<GraphicRaycaster>();
    }

    public virtual void SetActive(bool value, Upgraders upgraderType = Upgraders.None)
    {
        if (value)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    protected virtual void Open()
    {
        if (_canvas != null)
        {
            _canvas.enabled = true;
        }

        if (_raycaster != null)
        {
            _raycaster.enabled = true;
        }
    }

    protected virtual void Close()
    {
        if (_canvas != null)
        {
            _canvas.enabled = false;
        }

        if (_raycaster != null)
        {
            _raycaster.enabled = false;
        }
    }
}