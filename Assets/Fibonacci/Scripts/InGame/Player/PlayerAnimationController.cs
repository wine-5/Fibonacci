using UnityEngine;

namespace Fibonacci.InGame.Player
{
    /// <summary>
    /// プレイヤーの移動アニメーションを制御するクラス。
    /// ゲームフェーズの状態を確認し、移動入力（Vector2）に基づいて
    /// Animatorのパラメーター（IsRunning）を更新する役割を担います。
    /// </summary>
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        void Awake()
        {
            if (animator == null)
                animator = GetComponent<Animator>();
        }

        void Start()
        {
            if (animator != null)
            {
                animator.SetFloat("IsRunning", 0f);
            }
        }

        void Update()
        {
            if (GameManager.Instance.CurrentPhase != GamePhase.Playing) return;
        }

        public void UpdateMoveAnimation(Vector2 moveInput)
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentPhase != GamePhase.Playing)
            {
                if (animator != null) animator.SetFloat("IsRunning", 0f);
            }

            if (animator == null) return;

            float speed = Mathf.Abs(moveInput.x);
            animator.SetFloat("IsRunning", speed);
        }
    }
}