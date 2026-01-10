using UnityEngine;
using UnityEngine.InputSystem;
using Fibonacci.Event;

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

        public void OnMove(InputAction.CallbackContext context)
        {
            if (playerController != null)
            {
                playerController.SetMoveInput(context.ReadValue<Vector2>());
            }
        }

        public void OnRestart(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                GameEvents.TriggerRestart();
            }
        }
    }
}