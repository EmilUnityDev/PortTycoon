using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPanel : UIPanel
{
    public override void SetActive(bool value, Upgraders type = Upgraders.None)
    {
        base.SetActive(value);
    }

    protected override void Open()
    {
        base.Open();
    }

    protected override void Close()
    {
        base.Close();
    }

    public void BuildButtonPressed()
    {
        _ui.BuildButtonPressed();
    }
}