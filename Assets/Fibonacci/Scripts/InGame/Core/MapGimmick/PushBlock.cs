using UnityEngine;
using Fibonacci.Event;

namespace Fibonacci.InGame.Core.MapGimmick
{
    /// <summary>
    /// 特定の質量を持つオブジェクトが接触している間のみ移動可能になる押しブロック。
    /// 入力イベントを最小限に抑えるため、衝突の開始と終了時のみ状態判定を行います。
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PushBlock : MonoBehaviour
    {
        [Header("設定")]
        [SerializeField] private float requiredMass = 2.0f;

        private Rigidbody2D rb;
        private Vector3 initialPosition;
        private bool isPushable = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            initialPosition = transform.position;
            LockBlock();
        }

        private void OnEnable()
        {
            GameEvents.OnRestart += ResetBlock;
        }

        private void OnDisable()
        {
            GameEvents.OnRestart -= ResetBlock;
        }

        /// <summary>
        /// ブロックの位置と物理状態を初期状態にリセットします。
        /// </summary>
        private void ResetBlock()
        {
            rb.simulated = false;

            transform.position = initialPosition;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            rb.simulated = true;
            isPushable = false;
            LockBlock();
        }

        /// <summary>
        /// オブジェクトが接触した際、質量条件を満たしていれば移動制限を解除します。
        /// </summary>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Rigidbody2D otherRb = collision.gameObject.GetComponent<Rigidbody2D>();
            
            if (otherRb != null && otherRb.mass >= requiredMass)
            {
                isPushable = true;
                UnlockBlock();
            }
        }

        /// <summary>
        /// オブジェクトが離れた際、移動を禁止します。
        /// </summary>
        private void OnCollisionExit2D(Collision2D collision)
        {
            isPushable = false;
            LockBlock();
        }

        /// <summary>
        /// リジッドボディの制約を全固定し、物理的な動きを停止させます。
        /// </summary>
        private void LockBlock()
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        /// <summary>
        /// 回転のみを固定し、水平・垂直移動を許可します。
        /// </summary>
        private void UnlockBlock()
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
}