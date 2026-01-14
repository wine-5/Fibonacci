using UnityEngine;
using UnityEngine.InputSystem;
using Fibonacci.Event;
using Fibonacci.InGame.BorderLine.UI;
using Fibonacci.InGame;
using Fibonacci.Audio;
using Fibonacci.Scene;

namespace Fibonacci.InGame.Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private BorderLineEffectUI effectUI;

        public string EffectIdArea0 { get; private set; } = "";
        public string EffectIdArea1 { get; private set; } = "";

        private int lastAreaIndex = -1;

        public void OnMove(InputAction.CallbackContext context)
        {
            if (playerController != null)
            {
                playerController.SetMoveInput(context.ReadValue<Vector2>());
            }
        }

        public void OnRestart(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                GameEvents.TriggerRestart();
            }
        }

        /// <summary>
        /// リスタート時の処理。
        /// 入力に関する状態と、選択中のエフェクト情報を初期化します。
        /// </summary>
        private void OnGameRestart()
        {
            EffectIdArea0 = "";
            EffectIdArea1 = "";

            if (playerController != null)
            {
                playerController.ResetPlayerState();
            }

            lastAreaIndex = -1;           

        }

        private void OnEnable()
        {
            GameEvents.OnRestart += OnGameRestart;
        }

        private void OnDisable()
        {
            GameEvents.OnRestart -= OnGameRestart;
        }

        public void OnAreaChanged(int newAreaIndex)
        {
            lastAreaIndex = newAreaIndex;
        }
    }
}