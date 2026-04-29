using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Curtain : MonoBehaviour
{
    [SerializeField] private Image _curtain;

    public void SetCurtainInstantly(bool value)
    {
        _curtain.gameObject.SetActive(value);
    }

    public void SetSmoothly(bool value, float speed)
    {
        if (value)
        {

        }
        else
        {
            StartCoroutine(SmoothCurtainHiding(speed));
        }
    }

    private IEnumerator SmoothCurtainHiding(float speed)
    {
        _curtain.color = new Color(_curtain.color.r, _curtain.color.g, _curtain.color.b, 1f);

        while (_curtain.color.a >= 0f)
        {
            _curtain.color -= new Color(0f, 0f, 0f, Time.deltaTime * speed);

            yield return null;
        }

        _curtain.gameObject.SetActive(false);
    }
}