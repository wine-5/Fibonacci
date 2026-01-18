using UnityEngine;
using UnityEngine.InputSystem;
using Fibonacci.Event;
using Fibonacci.InGame.Core;

namespace Fibonacci.InGame.Player
{
    /// <summary>
    /// 入力デバイスからの信号をゲーム内コマンドへ変換する管理クラス。
    /// リスタート時には司令塔として、ゲームフェーズの差し戻し、能力のリセット、
    /// およびプレイヤー状態の初期化を適切な順序で実行します。
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
        /// 移動入力（スティック/キーボード）を検知し、プレイヤーコントローラーへベクトルを伝達します。
        /// </summary>
        /// <param name="context">InputSystemのアクションコンテキスト</param>
        public void OnMove(InputAction.CallbackContext context)
        {
            if (playerController != null)
            {
                playerController.SetMoveInput(context.ReadValue<Vector2>());
            }
        }

        /// <summary>
        /// リスタートボタンの入力を検知し、システム全体へリスタートイベントを発火させます。
        /// </summary>
        /// <param name="context">InputSystemのアクションコンテキスト</param>
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
        /// <param name="newAreaIndex">進入したエリアのインデックス</param>
        public void OnAreaChanged(int newAreaIndex)
        {
            lastAreaIndex = newAreaIndex;
        }

        /// <summary>
        /// システム全体のリスタート要求に応じ、依存する各マネージャーとオブジェクトを初期化します。
        /// 物理演算の予期せぬ挙動を防ぐため、最初にゲームフェーズを停止状態（Drawing）へリセットします。
        /// </summary>
        private void OnGameRestart()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResetPhase();
            }

            AbilityManager.Instance.Reset();

            EffectIdArea0 = "";
            EffectIdArea1 = "";

            if (playerController != null)
            {
                playerController.ResetPlayerState();
            }

            lastAreaIndex = -1;
        }
    }
}