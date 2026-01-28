using UnityEngine;
using UnityEngine.InputSystem;
using Fibonacci.Event;
using Fibonacci.InGame.Core;

namespace Fibonacci.InGame.Player
{
    /// <summary>
    /// 入力デバイスからの信号をゲーム内コマンドへ変換する管理クラス。
    /// リスタート時には司令塔として、ゲームフェーズ、能力、プレイヤー状態を初期化します。
    /// </summary>
    public class PlayerInputManager : MonoBehaviour
    {
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
        /// 移動入力を検知し、プレイヤーコントローラーへベクトルを伝達します。
        /// </summary>
        public void OnMove(InputAction.CallbackContext context)
        {
            playerController.SetMoveInput(context.ReadValue<Vector2>());
        }

        /// <summary>
        /// ジャンプ入力を検知し、コントローラーへ通知します。
        /// </summary>
        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            
            playerController.OnJumpInput();
        }

        /// <summary>
        /// リスタートボタンの入力を検知し、システム全体へリスタートイベントを発火させます。
        /// </summary>
        public void OnRestart(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                GameEvents.TriggerRestart();
            }
        }

        /// <summary>
        /// 所属エリアの変更を記録します。
        /// </summary>
        public void OnAreaChanged(int newAreaIndex)
        {
            lastAreaIndex = newAreaIndex;
        }

        /// <summary>
        /// システム全体のリスタート要求に応じ、関連するマネージャーとオブジェクトを初期設定に戻します。
        /// </summary>
        private void OnGameRestart()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResetPhase();
            }

            if (AbilityManager.Instance != null)
            {
                AbilityManager.Instance.Reset();
            }

            EffectIdArea0 = "";
            EffectIdArea1 = "";

            playerController.ResetPlayerState();

            lastAreaIndex = -1;
        }
    }
}