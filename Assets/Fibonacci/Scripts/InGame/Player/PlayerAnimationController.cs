using System;
using UnityEngine;
using Fibonacci.InGame;

namespace Fibonacci.Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        // ★名前を "IsRunning" に統一
        //private readonly int speedHash = Animator.StringToHash("IsRunning");

        void Awake()
        {
            if (animator == null)
                animator = GetComponent<Animator>();
        }

        void Start()
        {
            // 開始時に強制的にアニメーションを待機(0)にする
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
                // 強制的に立ちポーズ（0）にする
                if (animator != null) animator.SetFloat("IsRunning", 0f);
                return;
            }

            if (animator == null) return;

            // 横移動の絶対値を計算
            float speed = Mathf.Abs(moveInput.x);
            // Animatorのパラメーターを更新
            animator.SetFloat("IsRunning", speed);
        }
    }
}