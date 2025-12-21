using UnityEngine;
using UnityEngine.InputSystem;
using Fibonacci.Event;

namespace Fibonacci
{
    /// <summary>
    /// リスタート入力を管理するクラス
    /// PlayerInputコンポーネントのInvokeUnityEventsモードを使用
    /// </summary>
    public class ResetInputManager : MonoBehaviour
    {
        /// <summary>
        /// PlayerInputから呼び出されるリスタート入力のコールバック
        /// Input Action の名前が "Restart" の場合に自動的に呼ばれる
        /// </summary>
        public void OnRestart(InputAction.CallbackContext context)
        {
            //PlayerControllerのリセットと被って2個リセットが走る可能性
            if (context.performed)
                GameEvents.TriggerRestart();
        }
    }
}