using Spine.Unity;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tests.TestScripts
{
    public class TestInputHandler : MonoBehaviour
    {
        [SerializeField] private InputAction moveInput;
        [SerializeField] private float moveSpeed = 3;
        [SerializeField] private AnimationReferenceAsset moveAnimation;
        [SerializeField] private AnimationReferenceAsset idleAnimation;

        private SkeletonAnimation _skeletonAnimation;
        private Vector3 _initScale;

        private void Awake()
        {
            _skeletonAnimation = GetComponent<SkeletonAnimation>();
        }

        private void Start()
        {
            moveInput.Enable();
            _initScale = transform.localScale;
        }

        void Update()
        {
            var moveValue = moveInput.ReadValue<Vector2>();
            if (moveValue != Vector2.zero)
            {
                SetAnimation(moveAnimation);
                transform.localScale = new Vector3(_initScale.x * (moveValue.x > 0 ? 1 : -1), _initScale.y, _initScale.z);
                transform.position += new Vector3(moveValue.x, 0, moveValue.y) * moveSpeed * Time.deltaTime;
            }
            else
            {
                SetAnimation(idleAnimation);
            }
        }

        // устанавливаем анимацию на ассет
        private void SetAnimation(AnimationReferenceAsset animationAsset, bool loop = true, float timeScale = 1f)
        {
            // предотвращаем 
            if (_skeletonAnimation.AnimationName == animationAsset.name)
            {
                return;
            }

            var animationEntry = _skeletonAnimation.state.SetAnimation(0, animationAsset, loop);
            animationEntry.TimeScale = timeScale;
        }
    }
}