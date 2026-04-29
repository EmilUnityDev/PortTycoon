using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour
{
    private const string UI_SCENE_NAME = "UI";

    private void Start()
    {
        Application.targetFrameRate = 60;

        StartCoroutine(Loading());
    }

    private IEnumerator Loading()
    {
        Curtain curtain = FindObjectOfType<Curtain>();

        if (!SceneManager.GetSceneByName(UI_SCENE_NAME).isLoaded)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(UI_SCENE_NAME, LoadSceneMode.Additive);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        int tutorialProgression = PlayerPrefs.GetInt(PrefsContainer.TUTORIAL_PROGRESSION);

        int timeBetweenSessions = GetTimeBetweenSessionsInSeconds();

        if (PlayerPrefs.GetInt(PrefsContainer.IS_TUTORIAL_PASSED) != 1)
        {
            timeBetweenSessions = 0;
        }

        MoneyFactory moneyFactory = new MoneyFactory();
        CrystalsFactory crystalFactory = new CrystalsFactory();
        CrystallsStorage crStorage = FindObjectOfType<CrystallsStorage>(true);
        UI ui = FindObjectOfType<UI>(true);
        UpgradesController upgradesController = FindObjectOfType<UpgradesController>(true);
        ResourcesContainer resContainer = FindObjectOfType<ResourcesContainer>(true);
        Tutorial tutorial = FindObjectOfType<Tutorial>(true);

        tutorial.Init(Camera.main.GetComponent<CameraMovement>(), ui, tutorialProgression);

        FindObjectOfType<ShipWorkerNode>().InitCrystalFactory(crStorage);
        upgradesController.InitUpgradesData(resContainer);

        int moneyAmountForMinute = 100;
        int diamondAmountForMinute = 1;
        int offlineEarning = (int)(timeBetweenSessions / 60f * moneyAmountForMinute);
        int offlineDiamondEarning = (int)(timeBetweenSessions / 60f * diamondAmountForMinute);

        crStorage.Init(crystalFactory, upgradesController, resContainer, offlineDiamondEarning);

        resContainer.AddMovey(offlineEarning);
        ui.Init(upgradesController, resContainer, tutorial, offlineEarning);

        resContainer.Init();
        Camera.main.GetComponent<CameraUpgrader>().Init(Camera.main, ui);

        InitWorkers(upgradesController, moneyFactory, resContainer, tutorial);
        InitUpgraders(ui, moneyFactory, resContainer, tutorial);

        upgradesController.InvokeActions();

        yield return new WaitForSeconds(1f);

        if (curtain != null)
        {
            curtain.SetSmoothly(false, 1f);
        }
    }

    private void InitWorkers(UpgradesController upgradesController, MoneyFactory moneyFactory, ResourcesContainer resContainer, Tutorial tutorial)
    {
        foreach (var item in FindObjectsOfType<WorkerNode>(true))
        {
            item.Init(upgradesController, moneyFactory, resContainer, tutorial);
        }
    }

    private void InitUpgraders(UI ui, MoneyFactory moneyFactory, ResourcesContainer resContainer, Tutorial tutorial)
    {
        foreach (var item in FindObjectsOfType<Upgrader>(true))
        {
            item.Init(ui, moneyFactory, resContainer, tutorial);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = Time.timeScale == 15f ? 1f : 15f;
        }
    }

    private void SaveLastLoginTime()
    {
        PlayerPrefs.SetString(PrefsContainer.LAST_LOGIN_TIME, System.DateTime.Now.ToString());
    }

    private int GetTimeBetweenSessionsInSeconds()
    {
        if (!PlayerPrefs.HasKey(PrefsContainer.LAST_LOGIN_TIME))
        {
            return 0;
        }

        System.DateTime lastLogin = System.DateTime.Parse(PlayerPrefs.GetString(PrefsContainer.LAST_LOGIN_TIME));
        System.TimeSpan interval = System.DateTime.Now - lastLogin;
        int daysSeconds = interval.Days * 24 * 60 * 60;
        int hoursSeconds = interval.Hours * 60 * 60;
        int minutesSeconds = interval.Minutes * 60;

        int notAbsTotalSeconds = daysSeconds + hoursSeconds + minutesSeconds + interval.Seconds;

        int totalSeconds = Mathf.Abs(notAbsTotalSeconds);

        int secondsInWeek = 302400;

        totalSeconds = Mathf.Clamp(totalSeconds, 0, secondsInWeek);

        return notAbsTotalSeconds > 0 ? totalSeconds : 0;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SaveLastLoginTime();
        }
    }
}