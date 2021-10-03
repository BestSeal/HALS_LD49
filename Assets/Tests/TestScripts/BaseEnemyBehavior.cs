using System;
using Spine;
using Spine.Unity;
using Tests;
using Tests.TestScripts;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SkeletonAnimation))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class BaseEnemyBehavior : MonoBehaviour, IAttackable
{
    [SerializeField] private AnimationReferenceAsset moveAnimation;
    [SerializeField] private AnimationReferenceAsset idleAnimation;
    [SerializeField] private AnimationReferenceAsset fallAnimation;
    [SerializeField] private AnimationReferenceAsset landAnimation;
    [SerializeField] private AnimationReferenceAsset hitReceivedAnimation;
    [SerializeField] private AnimationReferenceAsset deathAnimation;
    [SerializeField] private LayerMask layersToCollideWith;
    [SerializeField] private float roamingDelay = 5;
    [SerializeField] private float roamingStartChance = 0.5f;
    [SerializeField] private float movingSpeed = 1;
    [SerializeField] private float roamingTime = 1;

    //private bool isNotLoopingAnimationFinished = true;
    private SkeletonAnimation _skeletonAnimation;
    private Collider _enemyCollider;
    private Rigidbody _rigidbody;

    private EnemyStateEnum _previousState;
    private EnemyStateEnum _currentState;
    private float roamMoveTimer = 0;
    private float roamDelayTimer = 0;
    private Vector3 _roamingDirection;
    private Vector3 _initialScale;

    private void Awake()
    {
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
        _enemyCollider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
        _initialScale = transform.localScale;
    }

    private void Start()
    {
        ChangeState(EnemyStateEnum.Falling);
    }

    private void Update()
    {
        if (_currentState == EnemyStateEnum.Idling)
        {
            ChangeState(EnemyStateEnum.Idling);
        }
    }

    private void FixedUpdate()
    {
        roamDelayTimer += Time.deltaTime;
        //var state = 
        if (_currentState == EnemyStateEnum.Idling && 
            roamDelayTimer > roamingDelay)
        {
            roamDelayTimer = 0;
            if (Random.Range(0f, 1f) < roamingStartChance)
            {
                roamMoveTimer = 0;
                var x = Random.Range(-1f, 1f);
                var z = Random.Range(-1f, 1f);
                _roamingDirection = new Vector3(x, 0, z);
                transform.localScale = new Vector3(_initialScale.x * (x > 0 ? 1 : -1), _initialScale.y, _initialScale.y);
                ChangeState(EnemyStateEnum.Moving);
            }
        }
        
        if (_currentState == EnemyStateEnum.Moving)
        {
            if (roamMoveTimer < roamingTime)
            {
                roamMoveTimer += Time.deltaTime;
                _rigidbody.MovePosition(transform.position + _roamingDirection * movingSpeed * Time.deltaTime); 
            }
            else
            {
                ChangeState(EnemyStateEnum.Idling);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (UnityHelper.IsObjectInLayerMask(other.gameObject, layersToCollideWith) &&
            _currentState == EnemyStateEnum.Falling)
        {
            ChangeState(EnemyStateEnum.Landed);
        }
    }


    private void SetAnimation(AnimationReferenceAsset animationAsset, bool isLooping = true, float timeScale = 1f)
    {
        if (_skeletonAnimation.AnimationName == animationAsset.name)
        {
            return;
        }

        //isNotLoopingAnimationFinished = isLooping;

        var animationEntry = _skeletonAnimation.state.SetAnimation(0, animationAsset, isLooping);
        animationEntry.TimeScale = timeScale;
        animationEntry.Complete += AnimationEntryOnComplete;
    }

    private void AnimationEntryOnComplete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == landAnimation.name)
        {
            ChangeState(EnemyStateEnum.Idling);
        }
    }

    private void ChangeState(EnemyStateEnum nextState)
    {
        switch (nextState)
        {
            case EnemyStateEnum.Landed:
                SetAnimation(landAnimation, false);
                break;
            case EnemyStateEnum.Spawned:
                break;
            case EnemyStateEnum.Falling:
                break;
            case EnemyStateEnum.ReceivingHit:
                break;
            case EnemyStateEnum.Moving:
                SetAnimation(moveAnimation);
                break;
            case EnemyStateEnum.Pushed:
                break;
            case EnemyStateEnum.Idling:
                SetAnimation(idleAnimation);
                break;
            case EnemyStateEnum.Death:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(nextState), nextState, null);
        }

        _currentState = nextState;
    }

    public void ReceiveAttack(float attackDamage, Vector3 attackDirection, float attackForce)
    {
        _rigidbody.AddForce(attackDirection * attackForce, ForceMode.Impulse);
    }
}

public enum EnemyStateEnum
{
    Landed,
    Spawned,
    Falling,
    ReceivingHit,
    Moving,
    Pushed,
    Idling,
    Death
}