using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using Tests;
using Tests.TestPlatformSettings;
using Tests.TestScripts;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlatformRotator : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private List<PlatformStage> platformStages;
    [SerializeField] private float rotateDuration = 2;
    [SerializeField] private int startTimerValue;
    [SerializeField] private Slider weightSlider;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private UnityEvent timeUpEvent;
    
    private float _totalWeightOfObjects = 0;
    private int _objectsOnPlatform = 0;
    private int _objectsOnRightSide = 0;
    private Quaternion _initialRotation;
    private Collider _collider;
    private Rigidbody _rigidbody;
    private List<GameObject> _objectOnPlatform;
    private int _currentStageNumber = 0;
    private float _currentWeightLimit = 0;
    private float _enemiesCheckTimer;
    private float _currentTimerValue;

    void Start()
    {
        _objectOnPlatform = new List<GameObject>();
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
        _initialRotation = transform.rotation;
        _currentWeightLimit = GetStageWeightLimit(_currentStageNumber);
        weightSlider.maxValue = GetStageWeightLimit(platformStages.Count - 1);
        weightSlider.value = 0;
        _currentTimerValue = startTimerValue;
        UpdateStageInfo();
        
    }

    private void Update()
    {
        _currentTimerValue -= Time.deltaTime;
        if (_currentTimerValue <= 0)
        {
            timeUpEvent.Invoke();
        }
        timer.text = (_currentTimerValue > 0 ? (int)_currentTimerValue : 0).ToString(CultureInfo.InvariantCulture);
        _enemiesCheckTimer += Time.deltaTime;
    }

    private void OnCollisionStay(Collision other)
    {
        if (_enemiesCheckTimer > rotateDuration)
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

            _enemiesCheckTimer = 0;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (UnityHelper.IsObjectInLayerMask(other.gameObject, layerMask))
        {
            _objectOnPlatform.Add(other.gameObject);
            _totalWeightOfObjects += other.gameObject.GetComponent<WeightInfo>()?.GetWeight ?? 0;

            UpdateStageInfo();
            
            if (_totalWeightOfObjects >= GetStageWeightLimit(platformStages.Count - 1) && _rigidbody.isKinematic)
            {
                _rigidbody.isKinematic = false;
                _rigidbody.useGravity = true;
            }
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
        weightSlider.value = _totalWeightOfObjects;
    }

    public float GetCurrentWeightOnPlatform()
        => _totalWeightOfObjects;

    public float GetCurrentTimerValue()
        => _currentTimerValue;
    
    IEnumerator RotateCoroutine(Vector3 rotationValue)
    {
        yield return transform.DORotate(rotationValue, rotateDuration, RotateMode.WorldAxisAdd).WaitForCompletion();
    }
}