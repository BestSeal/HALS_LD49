using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Tests;
using Tests.TestPlatformSettings;
using Tests.TestScripts;
using TMPro;
using UnityEngine;

public class PlatformRotator : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private List<PlatformStage> platformStages;
    [SerializeField] private float rotateDuration = 2;

    //!FOR TEST ONLY!
    [SerializeField] private TextMeshProUGUI currentWeight;
    [SerializeField] private TextMeshProUGUI currentStage;

    private float _totalWeightOfObjects = 0;
    private int _objectsOnPlatform = 0;
    private int _objectsOnRightSide = 0;
    private Quaternion _initialRotation;
    private Collider _collider;
    private List<GameObject> _objectOnPlatform;
    private int _currentStageNumber = 0;
    private float _currentWeightLimit = 0;
    private float enemiesCheckTimer;

    void Start()
    {
        _objectOnPlatform = new List<GameObject>();
        _collider = GetComponent<Collider>();
        _initialRotation = transform.rotation;
        _currentWeightLimit = GetStageWeightLimit(_currentStageNumber);
        UpdateStageInfo();
    }

    private void Update()
    {
        enemiesCheckTimer += Time.deltaTime;
    }

    private void OnCollisionStay(Collision other)
    {
        if (enemiesCheckTimer > rotateDuration)
        {
            if (_currentWeightLimit <= _totalWeightOfObjects)
            {
                if (platformStages.Count - 1 > _currentStageNumber)
                {
                    ++_currentStageNumber;
                }

                _currentWeightLimit = GetStageWeightLimit(_currentStageNumber);
                var rotationValue = -platformStages[_currentStageNumber].stageRotation;
                StartCoroutine(RotateCoroutine(new Vector3(0, 0, rotationValue)));
            }
            else if (_currentStageNumber > 0 &&
                     _totalWeightOfObjects < _currentWeightLimit -
                     platformStages[_currentStageNumber].criticalWeightUntilProceedToNextStage)
            {
                var rotationValue = platformStages[_currentStageNumber].stageRotation;
                StartCoroutine(RotateCoroutine(new Vector3(0, 0, rotationValue)));
                --_currentStageNumber;
                _currentWeightLimit = GetStageWeightLimit(_currentStageNumber);
            }

            enemiesCheckTimer = 0;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (UnityHelper.IsObjectInLayerMask(other.gameObject, layerMask))
        {
            _objectOnPlatform.Add(other.gameObject);
            _totalWeightOfObjects += other.gameObject.GetComponent<WeightInfo>()?.GetWeight ?? 0;

            UpdateStageInfo();

            // mb lose game somewhere here C:
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (UnityHelper.IsObjectInLayerMask(other.gameObject, layerMask))
        {
            _objectOnPlatform.Remove(other.gameObject);
            _totalWeightOfObjects -= other.gameObject.GetComponent<WeightInfo>()?.GetWeight ?? 0;


            UpdateStageInfo();
        }
    }

    private float GetStageWeightLimit(int stageNumber)
    {
        var weight = 0f;
        for (int i = 0; i <= stageNumber; i++)
        {
            weight += platformStages[i].criticalWeightUntilProceedToNextStage;
        }

        return weight;
    }

    private void UpdateStageInfo()
    {
        currentWeight.text = $"{_totalWeightOfObjects}/{_currentWeightLimit}";
        currentStage.text = $"{_currentStageNumber + 1}/{platformStages.Count}";
    }

    IEnumerator RotateCoroutine(Vector3 rotationValue)
    {
        yield return transform.DORotate(rotationValue, rotateDuration, RotateMode.LocalAxisAdd).WaitForCompletion();
    }
}