using UnityEngine;

namespace Fibonacci.InGame.Player
{
    /// <summary>
    /// プレイヤーキャラクターのアニメーション状態を制御するクラス。
    /// 移動入力の値に基づいて Animator のパラメーターを更新し、
    /// 待機状態と走行状態のアニメーション遷移を管理します。
    /// </summary>
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private void Awake()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        private void Start()
        {
            if (animator != null)
            {
                animator.SetFloat("IsRunning", 0f);
            }
        }

        /// <summary>
        /// 移動入力ベクトルを受け取り、その入力強度（横方向の絶対値）をアニメーターに反映させます。
        /// ゲームプレイ中でない場合は、強制的に停止状態のアニメーションへとリセットします。
        /// </summary>
        public void UpdateMoveAnimation(Vector2 moveInput)
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentPhase != GamePhase.Playing)
            {
                if (animator != null)
                {
                    animator.SetFloat("IsRunning", 0f);
                }
                return;
            }

            if (animator == null) return;

            float speed = Mathf.Abs(moveInput.x);
            animator.SetFloat("IsRunning", speed);
        }
    }
}