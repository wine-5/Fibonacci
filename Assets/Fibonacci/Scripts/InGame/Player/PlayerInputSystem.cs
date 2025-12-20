using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace Fibonacci.Player
{
    /// <summary>
    /// プレイヤーの入力処理を管理するクラス
    /// PlayerInputコンポーネントのInvokeUnityEventsモードを使用して入力を配信
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputSystem : MonoBehaviour
    {
        [Header("Movement Events")]
        [SerializeField] private UnityEvent<Vector2> onMoveInput = new UnityEvent<Vector2>();

        /// <summary>
        /// 移動入力イベントへのアクセス
        /// </summary>
        public UnityEvent<Vector2> OnMoveInput => onMoveInput;

        /// <summary>
        /// PlayerInputから呼び出される移動入力のコールバック
        /// InputActionのnameが"Move"の場合、自動的にOnMove(InputAction.CallbackContext)が呼ばれる
        /// </summary>
        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 moveInput = context.ReadValue<Vector2>();
            onMoveInput?.Invoke(moveInput);
        }
    }
}