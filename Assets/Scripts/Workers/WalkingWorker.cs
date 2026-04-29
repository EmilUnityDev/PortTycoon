using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class WalkingWorker : MonoBehaviour
{
    [SerializeField] private SplineComputer _spline;
    [SerializeField] private WorkerType _workerType;
    [SerializeField] private float _movementSpeed = .025f;

    private Animator _animator;
    private float _splineTimer = 0f;

    private void Start()
    {
        _animator = GetComponent<Animator>();

        _splineTimer = Random.Range(0f, 1f);

        if (_workerType == WorkerType.Walker)
        {
            _animator.SetBool("IsWalking", true);
        }
        else if(_workerType == WorkerType.Waver)
        {
            _animator.SetBool("IsWaving", true);
        }
        else if(_workerType == WorkerType.Carrier)
        {
            _animator.SetLayerWeight(1, 1f);
            _animator.SetBool("IsWalking", true);
        }
    }

    private void Update()
    {
        if (_workerType == WorkerType.Walker || _workerType == WorkerType.Carrier)
        {
            _splineTimer += Time.deltaTime * _movementSpeed;

            transform.position = _spline.EvaluatePosition(_splineTimer);
            transform.rotation = Quaternion.LookRotation(_spline.EvaluatePosition(_splineTimer + .001f) - transform.position, Vector3.up);

            if (_splineTimer >= 1f)
            {
                _splineTimer = 0f;
            }
        }
    }

    public enum WorkerType
    {
        Walker, Idle, Waver, Carrier
    }
}