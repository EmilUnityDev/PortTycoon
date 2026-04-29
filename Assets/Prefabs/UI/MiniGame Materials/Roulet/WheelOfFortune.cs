using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WheelOfFortune : MonoBehaviour
{
    public float Speed;
    public GameObject Wheel;
    [SerializeField] private Button StartWheel;
    [SerializeField] private GameObject WinPanel;
    [SerializeField] private Image WinImage;
    [SerializeField] private List<Sprite> Winsprt = new List<Sprite>();

    public void RotateTheWheelStart() =>
        StartCoroutine(RotateTheWheel());

    private void Start()
    {
        StartWheel.onClick.AddListener(RotateTheWheelStart);
        int randomSpeed = Random.Range(10, 15);
        Speed = randomSpeed;
    }

    IEnumerator RotateTheWheel()
    {
        while (Speed > 0)
        {
            Debug.Log(Wheel.transform.eulerAngles);
            Speed -= 0.01f;
            Wheel.transform.Rotate(0, 0, -Speed + Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        StartWheel.interactable = false;
        int youWin = CheckYourWinnings();
        StartCoroutine(NextStep(youWin));
        Debug.Log("You Win: " + youWin);
    }

    private IEnumerator NextStep(int win)
    {
        yield return new WaitForSeconds(0.8f);
        WinImage.sprite = Winsprt[win];
        WinPanel.transform.DOScale(Vector3.one, 0.3f);
    }
    private int CheckYourWinnings()
    {
        int win = 0;
        float angle = Wheel.transform.eulerAngles.z;
        win = angle switch
        {
            > 0 and < 36 => 0,
            > 36 and < 80 => 1,
            > 80 and < 125 => 0,
            > 125 and < 170 => 1,
            > 170 and < 215 => 0,
            > 215 and < 260 => 1,
            > 260 and < 305 => 0,
            > 305 and < 350 => 1,
            > 350 and < 360 => 0,
            _ => win
        };
        return win;
    }
}