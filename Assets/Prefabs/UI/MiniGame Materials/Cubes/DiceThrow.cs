using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DiceThrow : MonoBehaviour
{
    [SerializeField] private Image _dice1;
    [SerializeField] private Image _dice2;
    [SerializeField] private TMP_Text _winTxt;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private Button _startButton;
    [SerializeField] private List<Sprite> _dices = new List<Sprite>();

    private void Start()
    {
        _startButton.onClick.AddListener(StartThrow);
    }

    public void StartThrow()
    {
        _startButton.interactable = false;
        StartCoroutine(Throw());
    }

    IEnumerator Throw()
    {
        for (float i = 1; i > 0; i-=Time.deltaTime)
        {
            _dice1.sprite = _dices[Random.Range(0, 6)];
            _dice2.sprite = _dices[Random.Range(0, 6)];
            yield return new WaitForSeconds(0.05f);
        }
        _startButton.interactable = true;
        yield return new WaitForSeconds(1f);
        if (_dice1.sprite.name == _dice2.sprite.name)
            _winTxt.text = "+1000";
        else
            _winTxt.text = "0";

        _winPanel.transform.DOScale(Vector3.one, 0.3f);
    }
}
