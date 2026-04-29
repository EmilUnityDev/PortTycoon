using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Containers")]
public class ContainersCollection : ScriptableObject
{
    [SerializeField] private GameObject[] _containersPrefabs;

    public GameObject GetRandomContainerPrefab()
    {
        return _containersPrefabs[Random.Range(0, _containersPrefabs.Length)];
    }
}