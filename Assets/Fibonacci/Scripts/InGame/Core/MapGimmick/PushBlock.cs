using UnityEngine;
using Fibonacci.Event;

namespace Fibonacci.InGame.Core.MapGimmick
{
    /// <summary>
    /// 特定の質量を持つオブジェクトが接触している間のみ移動可能になる押しブロック。
    /// 条件を満たさない場合は物理的に固定され、パズルの障害物として機能します。
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PushBlock : MonoBehaviour
    {
        [Header("設定")]
        [SerializeField] private float requiredMass = 2.0f;

        private Rigidbody2D rb;
        private Vector3 initialPosition;

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
            LockBlock();
        }

        /// <summary>
        /// 接触しているオブジェクトの質量を判定し、条件を満たせばロックを解除します。
        /// </summary>
        private void OnCollisionStay2D(Collision2D collision)
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            
            if (playerRb != null && playerRb.mass >= requiredMass)
            {
                UnlockBlock();
                return;
            }

            LockBlock();
        }

        /// <summary>
        /// オブジェクトが離れた際、ブロックを固定します。
        /// </summary>
        private void OnCollisionExit2D(Collision2D collision)
        {
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
        /// 回転のみを固定し、移動を許可します。
        /// </summary>
        private void UnlockBlock()
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
}