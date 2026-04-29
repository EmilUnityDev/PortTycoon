using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : UIPanel
{
    [SerializeField] private List<UpgradeTypeDataMap> _upgradesData;
    [SerializeField] private GameObject _upgradeBoxPrefab;
    [SerializeField] private GameObject _fingerPrefab;
    [SerializeField] private GameObject _upgradePanel, _managerPanel;
    [SerializeField] private GameObject _closeTutorialFinger;
    [SerializeField] private Transform _objectToAnimate;
    [SerializeField] private AnimationCurve _showCurve, _hideCurve;
    [SerializeField] private Transform[] _layouts;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private float _layoutBoxScale = 225f;
    [SerializeField] private Slider _progressionSlider;
    [SerializeField] private TextMeshProUGUI _percentageProgressionText;

    private List<GameObject> _createdBoxes = new List<GameObject>();
    private UpgradePanelData _currentPanelData;
    private Upgraders _upgraderType;
    private float _startObjectToAnimatePosition;
    private int _tutorialUpgrades = 0;
    private bool _isCanQuit = true;
    private bool _isClosing;
    private bool _isShowing;

    private void Start()
    {
        _startObjectToAnimatePosition = _objectToAnimate.transform.localPosition.y;
    }

    public override void SetActive(bool value, Upgraders upgrader = Upgraders.None)
    {
        base.SetActive(value, upgrader);

        _upgraderType = upgrader;

        if (value)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    protected override void Open()
    {
        _isShowing = true;
        _isClosing = false;

        for (int i = 0; i < _upgradesData.Count; i++)
        {
            if (_upgradesData[i].Type == _upgraderType)
            {
                FillPanelWithData(_upgradesData[i].Data);

                break;
            }
        }

        base.Open();

        OpenUpgradePanel();

        StartCoroutine(ShowingAnimation());
    }

    public void StartClosing()
    {
        if (_isClosing || _isShowing || !_isCanQuit)
        {
            return;
        }

        _isClosing = true;

        StartCoroutine(HidingAnimation());
    }

    protected override void Close()
    {
        _tutorialUpgrades = 0;
        _closeTutorialFinger.SetActive(false);

        base.Close();

        if (_createdBoxes.Count > 0)
        {
            for (int i = 0; i < _createdBoxes.Count; i++)
            {
                Destroy(_createdBoxes[i]);
            }

            _createdBoxes.Clear();
        }
    }

    private IEnumerator ShowingAnimation()
    {
        float height = _objectToAnimate.transform.localPosition.y;
        float hidedPosition = height - Screen.height / 5f;

        float t = 0f;
        
        while (t <= 1f)
        {
            t += Time.deltaTime * 4f;

            _objectToAnimate.transform.localPosition = new Vector3(_objectToAnimate.transform.localPosition.x, Mathf.Lerp(hidedPosition, _startObjectToAnimatePosition, _showCurve.Evaluate(t)), _objectToAnimate.transform.localPosition.z);

            yield return null;
        }

        _isShowing = false;
    }

    private IEnumerator HidingAnimation()
    {
        float height = _objectToAnimate.transform.localPosition.y;
        float hidedPosition = height - Screen.height / 2f;

        float t = 0f;

        while (t <= 1f)
        {
            t += Time.deltaTime * 4f;

            _objectToAnimate.transform.localPosition = new Vector3(_objectToAnimate.transform.localPosition.x, Mathf.Lerp(_startObjectToAnimatePosition, hidedPosition, _hideCurve.Evaluate(t)), _objectToAnimate.transform.localPosition.z);

            yield return null;
        }

        Camera.main.GetComponent<CameraMovement>().StopSnap();

        _ui.EnablePanel(Panels.HudPanel);
    }

    private void FillPanelWithData(UpgradePanelData data)
    {
        _currentPanelData = data;

        for (int j = 0; j < 2; j++)
        {
            _layouts[j].GetComponent<RectTransform>().sizeDelta = new Vector2(_layouts[j].GetComponent<RectTransform>().sizeDelta.x, _layoutBoxScale * data._upgrades[j]._upgradeBoxesArray.Count);
            _layouts[j].transform.localPosition = new Vector3(0f, 0f, 0f);

            for (int i = 0; i < data._upgrades[j]._upgradeBoxesArray.Count; i++)
            {
                UpgradeTypes type = data._upgrades[j]._upgradeBoxesArray[i].Type;

                GameObject newUpgradeBox = Instantiate(_upgradeBoxPrefab);
                newUpgradeBox.transform.SetParent(_layouts[j]);
                newUpgradeBox.transform.localScale = _upgradeBoxPrefab.transform.localScale;
                newUpgradeBox.transform.localPosition = Vector3.zero;
                newUpgradeBox.GetComponent<UpgradeLayoutBoxData>().FillWithData(
                    data._upgrades[j]._upgradeBoxesArray[i],
                    this,
                    _upgradesController.GetUpgradeDataByType(type, j == 0),
                    _resourcesContainer,
                    j == 0);

                _createdBoxes.Add(newUpgradeBox);
            }
        }
    }

    public void CreateFingerAtFirstAndForbidToQuit()
    {
        while (_createdBoxes.Count == 0) { }

        _createdBoxes[0].GetComponent<UpgradeLayoutBoxData>().SetActiveTutorialFinger(true);

        _isCanQuit = false;

        StartCoroutine(TutorialSequence());
    }

    private IEnumerator TutorialSequence()
    {
        yield return new WaitWhile(() => _tutorialUpgrades < 1);

        _createdBoxes[0].GetComponent<UpgradeLayoutBoxData>().SetActiveTutorialFinger(false);
        _isCanQuit = true;
        _closeTutorialFinger.SetActive(true);
    }

    public void UpgradeButtonPressed(UpgradeTypes type, bool isMoneySpend)
    {
        UpgradeBackData backData = _upgradesController.TryToUpgrade(type, isMoneySpend);
        UpdateButtonData(type, backData);
        RecalculateGeneralPanelProgression();
        _tutorialUpgrades++;
    }

    private void UpdateButtonData(UpgradeTypes type, UpgradeBackData data)
    {
        foreach (GameObject box in _createdBoxes)
        {
            if (box.GetComponent<UpgradeLayoutBoxData>().GetBoxType() == type)
            {
                box.GetComponent<UpgradeLayoutBoxData>().UpdateData(data);
            }
        }
    }
    
    private void RecalculateGeneralPanelProgression()
    {
        int totalUpgradedLevels = 0;
        int totalMaxLevel = 0;

        foreach (GameObject box in _createdBoxes)
        {
            UpgradeLayoutBoxData boxData = box.GetComponent<UpgradeLayoutBoxData>();

            totalUpgradedLevels += _upgradesController.GetCurrentProgressionByType(boxData.GetBoxType()) - 1;
            totalMaxLevel += _upgradesController.GetMaxProgressionByType(boxData.GetBoxType()) - 1;
        }

        if (totalUpgradedLevels == 0 && totalMaxLevel == 0)
        {
            _progressionSlider.value = 0f;
        }
        else
        {
            _progressionSlider.value = (float)totalUpgradedLevels / totalMaxLevel;
        }

        _percentageProgressionText.text = (Mathf.RoundToInt(_progressionSlider.value * 100f) + " %");
    }

    public void OpenManagerPanel()
    {
        if (_isCanQuit)
        {
            _upgradePanel.SetActive(false);
            _managerPanel.SetActive(true);

            UpdatePanels();
        }
    }

    public void OpenUpgradePanel()
    {
        if (_isCanQuit)
        {
            _upgradePanel.SetActive(true);
            _managerPanel.SetActive(false);

            UpdatePanels();
        }
    }

    private void UpdatePanels()
    {
        for (int j = 0; j < 2; j++)
        {
            _layouts[j].transform.localPosition = new Vector3(0f, -100f, 0f);
        }

        if (_currentPanelData)
        {
            _titleText.text = _currentPanelData.UpgraderTitleManager;
        }

        RecalculateGeneralPanelProgression();
    }

    [System.Serializable]
    public class UpgradeTypeDataMap
    {
        public Upgraders Type;
        public UpgradePanelData Data;
    }
}