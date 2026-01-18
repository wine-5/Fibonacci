using UnityEngine;
using UnityEngine.InputSystem;
using Fibonacci.Event;
using Fibonacci.InGame.Core;

namespace Fibonacci.InGame.Player
{
    /// <summary>
    /// Unity Input System からの入力を受け取り、プレイヤーの操作やゲームシステムのコマンド（リスタート等）へ変換するクラス。
    /// 選択された能力の識別子保持や、リスタート時のプレイヤーおよび能力マネージャーの初期化も担当します。
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
        /// 移動入力が発生した際に呼ばれ、入力ベクトルを PlayerController へ伝達します。
        /// </summary>
        public void OnMove(InputAction.CallbackContext context)
        {
            if (playerController != null)
            {
                playerController.SetMoveInput(context.ReadValue<Vector2>());
            }
        }

        /// <summary>
        /// リスタート入力が発生した際に呼ばれ、ゲーム全体のリスタートイベントを発火させます。
        /// </summary>
        public void OnRestart(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                GameEvents.TriggerRestart();
            }
        }

        /// <summary>
        /// エリアの変更を検知した際に呼ばれ、内部で保持する現在のエリアインデックスを更新します。
        /// </summary>
        public void OnAreaChanged(int newAreaIndex)
        {
            lastAreaIndex = newAreaIndex;
        }

        private void OnGameRestart()
        {
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