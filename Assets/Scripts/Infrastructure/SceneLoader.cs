using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    private Curtain _curtain;

    private void Start()
    {
        Application.targetFrameRate = 60;

        InitCurtain();
        LoadScenes();
    }

    private void InitCurtain()
    {
        _curtain = FindObjectOfType<Curtain>();
        DontDestroyOnLoad(_curtain.gameObject);
    }

    private void LoadScenes()
    {
        StartCoroutine(LoadingEvent());
    }

    private IEnumerator LoadingEvent()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene");

        asyncLoad.completed += ((asyncLoad) => _curtain.SetCurtainInstantly(true));

        while (!asyncLoad.isDone)
        {
            _slider.value = Mathf.Lerp(_slider.value, asyncLoad.progress, 10f * Time.deltaTime);

            yield return null;
        }
    }
}