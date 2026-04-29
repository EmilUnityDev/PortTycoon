using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotsRoll : MonoBehaviour
{
    public Image image1;
    public Image image2;
    public Image image3;
    public Sprite[] sprites;
    [SerializeField] private Button _startSlots;
    [SerializeField] private TMP_Text _scoreEnd;
    [SerializeField] private GameObject _winPanel;

    public void StartSlots()
    {
        _startSlots.interactable = false;
        StartCoroutine(ChangeImages());
    }

    private IEnumerator ChangeImages()
    {
        float duration = 2f; 
        float elapsedTime = 0f; 
        while (elapsedTime < duration)
        {
            image1.sprite = sprites[Random.Range(0, sprites.Length)];
            image2.sprite = sprites[Random.Range(0, sprites.Length)];
            image3.sprite = sprites[Random.Range(0, sprites.Length)];

            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        yield return new WaitForSeconds(0.5f);

        if (image1.sprite.name==image2.sprite.name && image1.sprite.name==image3.sprite.name) 
            _scoreEnd.text = "1000";
        else
            _scoreEnd.text = "0";
        _winPanel.transform.DOScale(Vector3.one, 0.3f);
    }
}
