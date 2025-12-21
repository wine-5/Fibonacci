using UnityEngine;

namespace Fibonacci.Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField]private Animator animator;
        // ★名前を "IsRunning" に統一
        //private readonly int speedHash = Animator.StringToHash("IsRunning");
        void Awake()
        {
            if (animator == null)
                animator = GetComponent<Animator>();
        }

        public void UpdateMoveAnimation(Vector2 moveInput)
        {
            if (animator == null) return;

            // 横移動の絶対値を計算
            float speed = Mathf.Abs(moveInput.x);
            // Animatorのパラメーターを更新
            animator.SetFloat("IsRunning", speed);
        }
    }
}
