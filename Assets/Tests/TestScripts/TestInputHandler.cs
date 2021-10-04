using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tests.TestScripts
{
    public class TestInputHandler : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float baseVolume = 0.2f;
        
        [SerializeField] private AnimationReferenceAsset moveAnimation;
        [SerializeField] private AnimationReferenceAsset idleAnimation;
        [SerializeField] private AnimationReferenceAsset pushAnimation;
        [SerializeField] private AnimationReferenceAsset hitAnimation;
        [SerializeField] private AudioClip hitSound;
        

        [SerializeField] private InputAction moveInput;
        [SerializeField] private InputAction hitAction;

        [SerializeField] private float moveSpeed = 3;
        [SerializeField] private float attackRange = 3;
        [SerializeField] private float attackDelay = 1;
        [SerializeField] private float attackForce = 3;
        [SerializeField] private float attackDamage = 3;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private GameObject attackZone;


        private SkeletonAnimation _skeletonAnimation;
        private Rigidbody _rigidbody;
        private Vector3 _initScale;
        private Vector3 _moveDirection;
        private bool _notLoopingAnimationFinished = true;

        // enum better C:
        private bool _isPushing;

        private void Awake()
        {
            _skeletonAnimation = GetComponent<SkeletonAnimation>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            moveInput.Enable();
            hitAction.Enable();
            _initScale = transform.localScale;
            _moveDirection = Vector3.right;
        }

        private void Update()
        {
            if (hitAction.triggered)
            {
                SetAnimation(hitAnimation, false);
                _notLoopingAnimationFinished = false;
            }
        }

        void FixedUpdate()
        {
            
            var moveValue = moveInput.ReadValue<Vector2>();
            if (moveValue != Vector2.zero && _notLoopingAnimationFinished)
            {
                SetAnimation(moveAnimation);
                _moveDirection = new Vector3(moveValue.x, 0, moveValue.y);
                transform.localScale =
                    new Vector3(_initScale.x * (moveValue.x > 0 ? 1 : -1), _initScale.y, _initScale.z);
                _rigidbody.MovePosition(transform.position + _moveDirection * moveSpeed * Time.deltaTime);
            }
            else
            {
                SetAnimation(idleAnimation);
            }
        }

        private void SetAnimation(AnimationReferenceAsset animationAsset, bool loop = true, float timeScale = 1f)
        {
            if (animationAsset != null)
            {
                if (_skeletonAnimation.AnimationName == animationAsset.name || !_notLoopingAnimationFinished ||_isPushing)
                {
                    return;
                }
                var animationEntry = _skeletonAnimation.state.SetAnimation(0, animationAsset, loop);
                animationEntry.TimeScale = timeScale;
                animationEntry.Complete += AnimationEntryOnComplete;
                animationEntry.Event += CustomEventFired;
            }
        }

        private void AnimationEntryOnComplete(TrackEntry trackentry)
        {
            if (trackentry.Animation.Name == hitAnimation.name)
            {
                _notLoopingAnimationFinished = true;
            }
        }

        /*private void OnCollisionStay(Collision other)
        {
            if (UnityHelper.IsObjectInLayerMask(other.gameObject, enemyLayer))
            {
                SetAnimation(pushAnimation);
                _isPushing = true;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (UnityHelper.IsObjectInLayerMask(other.gameObject, enemyLayer))
            {
                _isPushing = false;
            }
        }*/

        private void CustomEventFired(TrackEntry trackEntry, Spine.Event e)
        {
            if (e.Data.Name == "attack_event")
            {
                Attack();
            }
        }

        private void Attack()
        {
            var hittedEntites = Physics.OverlapSphere(attackZone.transform.position, attackRange, enemyLayer);
            audioSource.PlayOneShot(hitSound, baseVolume);
            foreach (var entity in hittedEntites)
            {
                entity.GetComponent<IAttackable>()?.ReceiveAttack(attackDamage, _moveDirection + Vector3.up, attackForce);
            }
        }

        public void DisableInput()
        {
            moveInput.Disable();
            hitAction.Disable();
        }
    }
}