using UnityEngine;
using UnityEngine.InputSystem;
using Fibonacci.Event;

namespace Fibonacci
{
    /// <summary>
    /// Rキーでリスタートするシンプルなクラス
    /// GameJam用の軽量実装
    /// </summary>
    public class ResetInputManager : MonoBehaviour
    {
        [Header("入力設定")]
        [SerializeField] private InputActionReference restartAction;

        private void OnEnable()
        {
            if (restartAction != null)
            {
                restartAction.action.Enable();
                restartAction.action.performed += OnRestartPressed;
            }
        }

        private void OnDisable()
        {
            if (restartAction != null)
            {
                restartAction.action.performed -= OnRestartPressed;
                restartAction.action.Disable();
            }
        }

        /// <summary>
        /// Rキーが押された時の処理
        /// </summary>
        private void OnRestartPressed(InputAction.CallbackContext context)
        {
            // リスタートイベントを発火
            GameEvents.TriggerRestart();
        }
    }
}
