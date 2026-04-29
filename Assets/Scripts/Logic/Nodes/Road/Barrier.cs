using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public bool IsHaveTrigger
    {
        get => _collidersInRange.Count > 0;
    }

    private List<Collider> _collidersInRange = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        _collidersInRange.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        _collidersInRange.Remove(other);
    }
}