using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlace : MonoBehaviour
{
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        RaycastHit hit;

        if (Input.GetMouseButtonUp(0) && Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hit))
        {
            if (hit.transform != null)
            {
                if (hit.transform == transform)
                {
                    GetComponent<Collider>().enabled = false;
                    FindObjectOfType<Tutorial>().DestroyFinger();
                    FindObjectOfType<UI>().EnablePanel(Panels.BuildingPanel);
                }
            }
        }
    }
}