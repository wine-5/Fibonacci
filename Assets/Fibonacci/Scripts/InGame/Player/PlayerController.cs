using UnityEngine;
using UnityEngine.InputSystem;
using Fibonacci.Event;

namespace Fibonacci.Player
{
    /// <summary>
    /// プレイヤーのメインコントローラー
    /// PlayerInputコンポーネントからの入力を受け取り、各コンポーネントに配信
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        private PlayerMove playerMove;
        void Awake()
        {
            playerMove = GetComponent<PlayerMove>();
        }

        void OnEnable()
        {
            GameEvents.OnRestart += OnGameRestart;
        }

        void OnDisable()
        {
            GameEvents.OnRestart -= OnGameRestart;
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
        }
    }
}