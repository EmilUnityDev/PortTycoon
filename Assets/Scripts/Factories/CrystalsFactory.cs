using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalsFactory
{
    private const string PATH = "Prefabs/Crystal/Crystal";

    public GameObject Spawn(Vector3 startPoint)
    {
        GameObject newCrystal = Object.Instantiate(Resources.Load(PATH)) as GameObject;
        newCrystal.transform.position = startPoint;
        newCrystal.GetComponent<Rigidbody>().AddForce((Vector3.up - Vector3.forward * Random.Range(1f, 2f) + Vector3.right * Random.Range(-1f, 1f)) * Random.Range(10f, 20f), ForceMode.Impulse);
        return newCrystal;
    }
}