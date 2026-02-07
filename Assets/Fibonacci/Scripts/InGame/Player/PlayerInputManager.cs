using UnityEngine;
using UnityEngine.InputSystem;
using Fibonacci.Event;
using Fibonacci.InGame.Core;

namespace Fibonacci.InGame.Player
{
    /// <summary>
    /// 入力デバイスからの信号をゲーム内コマンドへ変換し、プレイヤーやゲーム進行の制御を行う管理クラス。
    /// </summary>
    public class PlayerInputManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController playerController;

        public string EffectIdArea0 { get; private set; } = "";
        public string EffectIdArea1 { get; private set; } = "";

        private int lastAreaIndex = -1;

        private void OnEnable()
        {
            GameEvents.OnRestart += OnGameRestart;
        }

        private void OnDisable()
        {
            GameEvents.OnRestart -= OnGameRestart;
        }

        /// <summary>
        /// 移動入力を検知し、プレイヤーコントローラーへベクトルを伝達する。
        /// </summary>
        public void OnMove(InputAction.CallbackContext context)
        {
            if (playerController == null) return;
            playerController.SetMoveInput(context.ReadValue<Vector2>());
        }

        /// <summary>
        /// ジャンプ入力を検知し、コントローラーへ通知する。
        /// </summary>
        public void OnJump(InputAction.CallbackContext context)
        {
            if (playerController == null || !context.started) return;
            
            playerController.OnJumpInput();
        }

        /// <summary>
        /// リスタートボタンの入力を検知し、システム全体へリスタートイベントを発火させる。
        /// </summary>
        public void OnRestart(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            
            GameEvents.TriggerRestart();
        }

        /// <summary>
        /// メニューボタン（Esc等）の入力を検知し、ポーズ状態を切り替える。
        /// </summary>
        public void OnMenu(InputAction.CallbackContext context)
        {
            if (!context.started) return;

            if (GameManager.HasInstance)
            {
                GameManager.Instance.TogglePause();
            }
        }

        /// <summary>
        /// 所属エリアの変更を記録する。
        /// </summary>
        public void OnAreaChanged(int newAreaIndex)
        {
            lastAreaIndex = newAreaIndex;
        }

        /// <summary>
        /// リスタート要求に応じ、関連するマネージャーとオブジェクトを初期状態に戻す。
        /// </summary>
        private void OnGameRestart()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.ResetPhase();
            }

            if (AbilityManager.HasInstance)
            {
                AbilityManager.Instance.ResetAbilities();
            }

            EffectIdArea0 = "";
            EffectIdArea1 = "";
            lastAreaIndex = -1;

            if (playerController != null)
            {
                playerController.ResetPlayerState();
            }
        }
    }
}