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

            // 1. ログを出力
            Debug.Log($"<color=yellow>【UI選択】エリア {regionId} に効果「{def.Id}」が設定されました</color>");

            // 2. 効果を保持
            if (regionId == 0) EffectIdArea0 = def.Id;
            else if (regionId == 1) EffectIdArea1 = def.Id;

            // 3. UI側に「選択したよ」と伝えてアイコンを表示させる
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
            string currentEffect = (newAreaIndex == 0) ? EffectIdArea0 : EffectIdArea1;

            if (currentEffect == "ZeroGravity")
            {
                if (playerGravity != null)
                {
                    playerGravity.ReverseGravity();
                    Debug.Log($"<color=cyan>効果発動：{currentEffect} により重力を反転しました</color>");
                }
            }
        }
    }
}