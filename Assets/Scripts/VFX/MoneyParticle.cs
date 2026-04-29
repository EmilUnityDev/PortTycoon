using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoneyParticle : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _text;
    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private float _upFlyingHeight = 3f;
    [SerializeField] private float _upFlyingSpeed = 1f;

    public void PlayParticle(int moneyAmount)
    {
        _canvas.sortingOrder = Random.Range(-100, 100);
        _text.text = "+" + moneyAmount.ConvertToString() + " $";
        _particle.Play();

        _upFlyingHeight *= Random.Range(.8f, 1.2f);
        _upFlyingSpeed *= Random.Range(.8f, 1.2f);
        _text.transform.localScale *= Random.Range(.7f, 1f);

        StartCoroutine(UpMovement());
    }

    private IEnumerator UpMovement()
    {
        float t = 0f;

        while (t <= 1f)
        {
            t += Time.deltaTime * _upFlyingSpeed;

            _text.transform.localPosition += Vector3.up * Time.deltaTime * _upFlyingHeight;

            yield return null;
        }

        _text.transform.DOScale(Vector3.zero, .5f).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
    }
}