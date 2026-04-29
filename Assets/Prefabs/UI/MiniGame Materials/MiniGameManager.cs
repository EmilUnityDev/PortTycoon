using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{

    public void Open(GameObject panel)
    {
        DOTween.Sequence().Append(panel.transform.DOScale(new Vector3(1.3f,1.3f,1.3f), 0.3f)).
            Append(panel.transform.DOScale(Vector3.one, 0.3f));
    }
    
    public void Close(GameObject panel)
    {
        DOTween.Sequence().Append(panel.transform.DOScale(new Vector3(1.3f,1.3f,1.3f), 0.3f)).
            Append(panel.transform.DOScale(Vector3.zero, 0.3f));
    }
}
