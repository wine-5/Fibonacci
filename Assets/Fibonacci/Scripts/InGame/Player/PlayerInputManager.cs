using UnityEngine;
using UnityEngine.InputSystem;
using Fibonacci.Event;
using Fibonacci.InGame.BorderLine.UI;
using Fibonacci.Scene;  

namespace Fibonacci.InGame.Player
{
    /// <summary>
    /// 入力デバイスとゲームロジックの仲介（Input Bridge）を担当するクラス。
    /// Unity Input System からの物理的な入力を受け取り、それを「移動値のセット」や
    /// 「リスタートの実行」といった具体的なアクションとして、コントローラーや
    /// イベントシステムへ配信する責任を持ちます。
    /// </summary>
    public class PlayerInputManager : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private BorderLineEffectUI effectUI;
        [SerializeField] private PlayerGravity playerGravity;

        public string EffectIdArea0 { get; private set; } = "";
        public string EffectIdArea1 { get; private set; } = "";


        public void OnMove(InputAction.CallbackContext context)
        {
            if (playerController != null)
            {
                playerController.SetMoveInput(context.ReadValue<Vector2>());
            }
        }




        public void OnRestart(InputAction.CallbackContext context)
        {
            Debug.Log("Restart Input Received");
            if (context.started)
            {
                GameEvents.TriggerRestart();
            }
        }

        private void OnGameRestart()
        {
            EffectIdArea0 = "";
            EffectIdArea1 = "";

            if (playerController != null)
            {
                playerController.ResetPlayerState();
            }
        }

        private void OnEnable()
        {
            GameEvents.OnRestart += OnGameRestart;
            //if (effectUI != null) effectUI.EffectClicked += OnEffectClicked;
        }

        private void OnDisable()
        {
            GameEvents.OnRestart -= OnGameRestart;
            //if (effectUI != null) effectUI.EffectClicked -= OnEffectClicked;
        }




        public void OnAreaChanged(int newAreaIndex)
        {
            string currentEffect = (newAreaIndex == 0) ? EffectIdArea0 : EffectIdArea1;
            if (currentEffect == "ZeroGravity") playerGravity.SetGravityScale(-1f);
            else playerGravity.SetNormalGravity();
        }

    }
}