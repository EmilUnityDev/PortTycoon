using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;

public class Analytics : Singleton<Analytics>
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        GameAnalytics.Initialize();


    }

    public void LevelEvent(int levelID, LevelState state)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("levelID", levelID);
        parameters.Add("state", state.ToString());

        

        GameAnalytics.NewDesignEvent("level:" + levelID + ":" + state.ToString());
    }
}

public enum LevelState
{
    start = 0,
    complete = 1,
    fail = 2
}

