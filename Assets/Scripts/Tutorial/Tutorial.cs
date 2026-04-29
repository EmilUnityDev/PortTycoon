using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public bool IsBuiltPortCrane => _currentPoint > 0 || _isTutorialPassed;
    public bool IsPowerStationUpgrade => _currentPoint == 2;
    public bool IsStorageSquareBuilt => _currentPoint > 3 || _isTutorialPassed;
    public bool IsStorageCraneBuilt => _currentPoint > 4 || _isTutorialPassed;
    public bool IsGarageBuilt => _currentPoint > 5 || _isTutorialPassed;
    public bool IsGarageUpgrade => _currentPoint == 6;
    public bool IsTapOnBank => _currentPoint == 9 || _isTutorialPassed;

    [SerializeField] private List<TutorialPoint> _tutorialPoints;
    [SerializeField] private GameObject _fingerPrefab;

    private GameObject _createdFingerPrefab;
    private UI _ui;
    private CameraMovement _camera;
    private int _currentPoint = 0;
    private bool _isStopBuildingWaiting, _isStopTapping;
    private bool _isTutorialPassed;

    public void Init(CameraMovement camera, UI ui, int tutorialProgression)
    {
        _currentPoint = tutorialProgression;

        _ui = ui;
        _camera = camera;

        _isTutorialPassed = PlayerPrefs.GetInt(PrefsContainer.IS_TUTORIAL_PASSED) == 1;

        if (!_isTutorialPassed)
        {
            RecreateTutorial(_currentPoint);
            ContinueTutorial(_currentPoint);
        }
    }

    private void RecreateTutorial(int endTutorialPoint)
    {
        for (int i = endTutorialPoint; i < _tutorialPoints.Count; i++)
        {
            SetPlaceToBuild(_tutorialPoints[i], false);
        }
    }

    public void DestroyFinger()
    {
        Destroy(_createdFingerPrefab);
    }

    private void ContinueTutorial(int tutorialPointID)
    {
        TutorialPoint tPoint = _tutorialPoints[tutorialPointID];

        SetPlaceToBuild(tPoint, true);

        PointCamera(tPoint.FingerPoint == null ? tPoint.BuildingObjects[0].transform : tPoint.FingerPoint);

        _isStopBuildingWaiting = false;

        StartCoroutine(WaitingForBuilding(tPoint));
    }

    private void PointCamera(Transform point)
    {
        _createdFingerPrefab = Instantiate(_fingerPrefab);
        _createdFingerPrefab.transform.position = point.transform.position;

        _camera.StartTutorialSnap(point);
    }

    private IEnumerator WaitingForBuilding(TutorialPoint tPoint)
    {
        if (tPoint.Type == TutorialPointType.Build)
        {
            yield return new WaitWhile(() => !_isStopBuildingWaiting);

            _isStopBuildingWaiting = false;

            Build(tPoint);
        }

        if (tPoint.Type == TutorialPointType.Tap)
        {
            yield return new WaitWhile(() => !_isStopTapping);

            DestroyFinger();
        }

        if (_currentPoint + 1 == _tutorialPoints.Count)
        {
            PassTutorial();
            yield break;
        }

        if (tPoint.Type == TutorialPointType.Build)
        {
            _currentPoint++;
            PlayerPrefs.SetInt(PrefsContainer.TUTORIAL_PROGRESSION, _currentPoint);

            if (_currentPoint != 2 && _currentPoint != 6)
            {
                yield return new WaitForSeconds(5f);
            }

            ContinueTutorial(_currentPoint);
        }
        else if (tPoint.Type == TutorialPointType.Touch)
        { 
            yield return new WaitWhile(() => _ui.GetCurrentPanelType() != Panels.UpgradePanel);

            DestroyFinger();

            yield return new WaitWhile(() => _ui.GetCurrentPanelType() != Panels.HudPanel);

            yield return new WaitForSeconds(1f);

            _currentPoint++;
            PlayerPrefs.SetInt(PrefsContainer.TUTORIAL_PROGRESSION, _currentPoint);
            ContinueTutorial(_currentPoint);
        }
    }

    private void PassTutorial()
    {
        _isTutorialPassed = true;
        PlayerPrefs.SetInt(PrefsContainer.IS_TUTORIAL_PASSED, 1);
    }

    public void StopBuildWaiting()
    {
        _isStopBuildingWaiting = true;
    }

    public void StopTapping()
    {
        _isStopTapping = true;
    }

    private void Build(TutorialPoint tPoint)
    {
        SetPlaceBuilded(tPoint);
    }

    private void SetPlaceToBuild(TutorialPoint tPoint, bool isActivateTrigger)
    {
        for (int i = 0; i < tPoint.BuildingObjects.Count; i++)
        {
            tPoint.BuildingObjects[i].gameObject.SetActive(true);
            if (tPoint.BuildingObjects[i].TryGetComponent(out Collider collider))
            {
                collider.enabled = isActivateTrigger;
            }
        }

        for (int i = 0; i < tPoint.ToBuildObject.Count; i++)
        {
            tPoint.ToBuildObject[i].gameObject.SetActive(false);
        }
    }

    private void SetPlaceBuilded(TutorialPoint tPoint)
    {
        for (int i = 0; i < tPoint.BuildingObjects.Count; i++)
        {
            tPoint.BuildingObjects[i].transform.DOScale(Vector3.zero, .2f).SetEase(Ease.InBack).OnComplete(() => tPoint.BuildingObjects[i].gameObject.SetActive(false));
        }

        for (int i = 0; i < tPoint.ToBuildObject.Count; i++)
        {
            Vector3 endScale = tPoint.ToBuildObject[i].transform.localScale;
            tPoint.ToBuildObject[i].transform.localScale = Vector3.zero;
            
            tPoint.ToBuildObject[i].gameObject.SetActive(true);
            tPoint.ToBuildObject[i].transform.DOScale(endScale, .5f).SetEase(Ease.OutBack);
        }
    }
}

public enum TutorialPointType
{ 
    Build, Touch, Tap
}

[System.Serializable]
public class TutorialPoint
{
    public string TutorialPointName = "PLEASE PRINT HERE NAME";
    public TutorialPointType Type;
    public List<GameObject> BuildingObjects;
    public List<GameObject> ToBuildObject;
    public Transform FingerPoint;
}