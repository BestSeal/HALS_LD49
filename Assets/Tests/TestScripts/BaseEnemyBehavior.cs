using System;
using Spine;
using Spine.Unity;
using Tests;
using Tests.TestScripts;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SkeletonAnimation))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class BaseEnemyBehavior : MonoBehaviour, IAttackable
{
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected float baseVolume = 0.1f;
    [SerializeField] protected AudioClip deathAudioClip;
    [SerializeField] protected AudioClip hitReceivedAudioClip;
    [SerializeField] protected AudioClip landAudioClip;
    
    [SerializeField] protected AnimationReferenceAsset moveAnimation;
    [SerializeField] protected AnimationReferenceAsset idleAnimation;
    [SerializeField] protected AnimationReferenceAsset fallAnimation;
    [SerializeField] protected AnimationReferenceAsset landAnimation;
    [SerializeField] protected AnimationReferenceAsset hitReceivedAnimation;
    [SerializeField] protected AnimationReferenceAsset deathAnimation;
    
    [SerializeField] protected LayerMask layersToCollideWith;
    [SerializeField] protected float roamingDelay = 5;
    [SerializeField] protected float roamingStartChance = 0.5f;
    [SerializeField] protected float movingSpeed = 1;
    [SerializeField] protected float roamingTime = 1;

    //private bool isNotLoopingAnimationFinished = true;
    protected SkeletonAnimation _skeletonAnimation;
    protected Collider _enemyCollider;
    protected Rigidbody _rigidbody;

    protected EnemyStateEnum _previousState;
    protected EnemyStateEnum _currentState;
    protected float roamMoveTimer = 0;
    protected float roamDelayTimer = 0;
    protected Vector3 _roamingDirection;
    protected Vector3 _initialScale;
    protected bool _isDead;

    private void Awake()
    {
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
        _enemyCollider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
        _initialScale = transform.localScale;
    }

    protected void Start()
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
            roamDelayTimer > roamingDelay &&
            moveAnimation != null)
        {
            roamDelayTimer = 0;
            if (Random.Range(0f, 1f) < roamingStartChance)
            {
                roamMoveTimer = 0;
                var x = Random.Range(-1f, 1f);
                var z = Random.Range(-1f, 1f);
                _roamingDirection = new Vector3(x, 0, z);
                transform.localScale =
                    new Vector3(_initialScale.x * (x > 0 ? 1 : -1), _initialScale.y, _initialScale.y);
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

    protected void OnCollisionEnter(Collision other)
    {
        if (UnityHelper.IsObjectInLayerMask(other.gameObject, layersToCollideWith) &&
            _currentState == EnemyStateEnum.Falling)
        {
            ChangeState(EnemyStateEnum.Landed);
        }
    }


    protected void SetAnimation(AnimationReferenceAsset animationAsset, bool isLooping = true, float timeScale = 1f)
    {
        if (animationAsset != null)
        {
            if (_skeletonAnimation.AnimationName == animationAsset.name)
            {
                return;
            }

            var animationEntry = _skeletonAnimation.state.SetAnimation(0, animationAsset, isLooping);
            animationEntry.TimeScale = timeScale;
            animationEntry.Complete += AnimationEntryOnComplete;
            animationEntry.Complete += DeathCheck;
        }
    }

    protected virtual void DeathCheck(TrackEntry trackentry)
    {
        if (trackentry.Animation.Name == deathAnimation?.name)
        {
            gameObject.SetActive(false);
        }
    }

    protected void AnimationEntryOnComplete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == landAnimation.name ||
            trackEntry.Animation.Name == hitReceivedAnimation.name)
        {
            ChangeState(EnemyStateEnum.Idling);
        }

    }

    protected void ChangeState(EnemyStateEnum nextState)
    {
        switch (nextState)
        {
            case EnemyStateEnum.Landed:
                audioSource.PlayOneShot(landAudioClip, baseVolume);
                SetAnimation(landAnimation, false);
                break;
            case EnemyStateEnum.Spawned:
                break;
            case EnemyStateEnum.Falling:
                SetAnimation(fallAnimation);
                break;
            case EnemyStateEnum.ReceivingHit:
                audioSource.PlayOneShot(hitReceivedAudioClip, baseVolume);
                SetAnimation(hitReceivedAnimation, false);
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
                _isDead = true;
                audioSource.PlayOneShot(deathAudioClip, baseVolume);
                SetAnimation(deathAnimation, false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(nextState), nextState, null);
        }

        _currentState = nextState;
    }

    public virtual void ReceiveAttack(float attackDamage, Vector3 attackDirection, float attackForce)
    {
        ChangeState(EnemyStateEnum.ReceivingHit);
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