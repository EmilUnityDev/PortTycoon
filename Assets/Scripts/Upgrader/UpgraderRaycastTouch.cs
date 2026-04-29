using UnityEngine;

public class UpgraderRaycastTouch : MonoBehaviour
{
    private IUIOpener _uiOpener;

    public void OpenUpgrader()
    {
        _uiOpener.OpenUI();
    }

    public void Init(IUIOpener uiOpener)
    {
        _uiOpener = uiOpener;
    }
}