using UnityEngine;
using UnityEngine.InputSystem;
using Fibonacci.Event;
using Fibonacci.InGame.BorderLine.UI;

namespace Fibonacci.Player
{
    /// <summary>
    /// プレイヤーのメインコントローラー
    /// PlayerInputコンポーネントからの入力を受け取り、各コンポーネントに配信
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private BorderLineEffectUI effectUI;
        [SerializeField] private PlayerGravity playerGravity;
        private PlayerMove playerMove;

        public string EffectIdArea0 { get; private set; } = "";
        public string EffectIdArea1 { get; private set; } = "";

        void Awake()
        {
            playerMove = GetComponent<PlayerMove>();
            if (playerGravity == null) playerGravity = GetComponent<PlayerGravity>();
        }

        void OnEnable()
        {
            GameEvents.OnRestart += OnGameRestart;

            if (effectUI != null)
            {
                effectUI.EffectClicked += OnEffectClicked;
            }
        }

        void OnDisable()
        {
            GameEvents.OnRestart -= OnGameRestart;

            if (effectUI != null)
            {
                effectUI.EffectClicked -= OnEffectClicked;
            }
        }

        private void OnEffectClicked(int frameIndex, int regionId, BorderLineEffectDefinition def)
        {
            if (def == null) return;

            // ★ 修正：regionId ではなく frameIndex を使ってみる（UIの枠番号 = エリア番号のはず）
            // また、実際にどんなIDがセットされたかログを出す
            if (frameIndex == 0)
            {
                EffectIdArea0 = def.Id;
                Debug.Log($"<color=cyan>EffectIdArea0 に '{def.Id}' をセットしました (frameIndex: {frameIndex})</color>");
            }
            else if (frameIndex == 1)
            {
                EffectIdArea1 = def.Id;
                Debug.Log($"<color=cyan>EffectIdArea1 に '{def.Id}' をセットしました (frameIndex: {frameIndex})</color>");
            }

            effectUI.ApplySelection(frameIndex, def.Id, def.Icon);
        }
        /// <summary>
        /// PlayerInputから呼び出される移動入力のコールバック
        /// Input Action の名前が "Move" の場合に自動的に呼ばれる
        /// </summary>
        public void OnMove(InputAction.CallbackContext context)
        {
            if (playerMove != null)
            {
                Vector2 moveInput = context.ReadValue<Vector2>();
                playerMove.OnMoveInput(moveInput);
            }
        }

        /// <summary>
        /// PlayerInputから呼び出されるリスタート入力のコールバック
        /// Input Action の名前が "Restart" の場合に自動的に呼ばれる
        /// </summary>
        public void OnRestart(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                GameEvents.TriggerRestart();
            }
        }

        /// <summary>
        /// ゲームリスタート時の処理
        /// </summary>
        private void OnGameRestart()
        {
            if (playerMove != null)
                playerMove.ResetPosition();

            EffectIdArea0 = "";
            EffectIdArea1 = "";
        }

        public void OnAreaChanged(int newAreaIndex)
        {
            // 現在保持しているエフェクトIDをログ出力
            string currentEffect = (newAreaIndex == 0) ? EffectIdArea0 : EffectIdArea1;
            Debug.Log($"<color=orange>エリア {newAreaIndex} のエフェクト判定中: 現在のIDは '{currentEffect}' です</color>");

            // ★ 文字列が完全に一致しているかチェック
            if (currentEffect == "ZeroGravity")
            {
                Debug.Log("<color=red>重力反転を実行します！</color>");
                playerGravity.SetGravityScale(-1f);
            }
            else
            {
                Debug.Log("<color=white>重力を通常に戻します</color>");
                playerGravity.SetNormalGravity();
            }
        }
        public void ResetGravity()
        {
            // もし今、重力が反転（マイナス）しているなら、ReverseGravityを呼んでプラスに戻す
            if (playerGravity != null && playerGravity.GetGravityScale() < 0)
            {
                playerGravity.ReverseGravity();
                Debug.Log("<color=white>エリア外に出たため重力を元に戻しました</color>");
            }
        }
    }
}