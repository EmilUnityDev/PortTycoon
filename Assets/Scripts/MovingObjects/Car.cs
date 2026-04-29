using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public bool HasAnotherCarInRange;

    [SerializeField] private bool _isGenerateContainerOnStart;
    [SerializeField] private Transform _containersPoint;
    [SerializeField] private ContainersCollection _containers;

    private void Start()
    {
        if (_isGenerateContainerOnStart)
        {
            GameObject containerPrefab = _containers.GetRandomContainerPrefab();
            Vector3 scale = containerPrefab.transform.localScale;
            GameObject newContainer = Instantiate(containerPrefab);
            newContainer.transform.SetParent(_containersPoint);
            newContainer.transform.localEulerAngles = Vector3.zero;
            newContainer.transform.localPosition = Vector3.zero;
            newContainer.transform.localScale = scale;
        }
    }
}