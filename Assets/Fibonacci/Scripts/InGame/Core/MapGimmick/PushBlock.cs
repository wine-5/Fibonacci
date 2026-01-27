using UnityEngine;

namespace Fibonacci.InGame.Core.MapGimmick
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PushBlock : MonoBehaviour
    {
        [Header("設定")]
        [SerializeField] private float requiredMass = 2.0f; // 押すのに必要な重さ

        private Rigidbody2D _rb;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            LockBlock();
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            // 衝突相手がRigidbody2Dを持っており、そのmassが基準値以上かチェック
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            
            if (playerRb != null && playerRb.mass >= requiredMass)
            {
                UnlockBlock();
            }
            else
            {
                // 重さが足りない、または離れたら即座にロック
                LockBlock();
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            // 離れたら動かないように固定
            LockBlock();
        }

        private void LockBlock()
        {
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;
            _rb.linearVelocity = Vector2.zero;
        }

        private void UnlockBlock()
        {
            // 回転だけ固定し、位置は自由に（押せる状態）
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
}