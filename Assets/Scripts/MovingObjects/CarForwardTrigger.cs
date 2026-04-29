using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarForwardTrigger : MonoBehaviour
{
    private int _carsInRange = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            _carsInRange++;

            if (_carsInRange == 1)
            {
                GetComponentInParent<Car>().HasAnotherCarInRange = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            _carsInRange--;

            if (_carsInRange == 0)
            {
                GetComponentInParent<Car>().HasAnotherCarInRange = false;
            }
        }
    }
}