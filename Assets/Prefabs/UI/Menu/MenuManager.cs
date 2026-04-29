using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _startPanel;
    void Start()
    {
        _startPanel.DOFade(0, 1).OnComplete((() =>
        {
            _startPanel.gameObject.SetActive(false);
        }));
    }


    public void NextScene()
    {
        _startPanel.gameObject.SetActive(true);
        {
            _startPanel.DOFade(1, 1).OnComplete((() =>
            {
                SceneManager.LoadScene("_Loading");
            }));
        }
    }
}
