using System.Collections.Generic;
using System.Globalization;
using Tests;
using Tests.TestPlatformSettings;
using Tests.TestScripts;
using TMPro;
using UnityEngine;

public class PlatformRotator : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private List<PlatformStage> platformStages;
    
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
    private float  _currentWeightLimit = 0;

    private 
    
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
        
    }

    private void OnCollisionStay(Collision other)
    {
        if (UnityHelper.IsObjectInLayerMask(other.gameObject, layerMask))
        {
            if (_collider.bounds.center.x > other.gameObject.transform.position.x)
            {
                ++_objectsOnRightSide;
            }
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (UnityHelper.IsObjectInLayerMask(other.gameObject, layerMask))
        {
            _objectOnPlatform.Add(other.gameObject);
            _totalWeightOfObjects += other.gameObject.GetComponent<WeightInfo>()?.GetWeight ?? 0;
            if (_currentWeightLimit <= _totalWeightOfObjects)
            {
                if (platformStages.Count - 1 > _currentStageNumber)
                {
                    ++_currentStageNumber;
                }

                _currentWeightLimit = GetStageWeightLimit(_currentStageNumber);
            }
            
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

            if (_totalWeightOfObjects < _currentWeightLimit && _currentStageNumber > 0)
            {
                --_currentStageNumber;
                _currentWeightLimit = GetStageWeightLimit(_currentStageNumber);
            }

            UpdateStageInfo();
        }
    }

    private float GetStageWeightLimit(int stageNumber)
    {
        var weight = 0f;
        for (int i = 0; i <= stageNumber; i++)
        {
            weight += platformStages[i].criticalWeightUntilProceedToNextStage ;
        }

        return weight;
    }

    private void UpdateStageInfo()
    {
        currentWeight.text = $"{_totalWeightOfObjects}/{_currentWeightLimit}";
        currentStage.text = $"{_currentStageNumber + 1}/{platformStages.Count}";
    }
}
