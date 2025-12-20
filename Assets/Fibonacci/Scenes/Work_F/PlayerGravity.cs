using UnityEngine;

namespace Fibonacci.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerGravity : MonoBehaviour
    {
        private Rigidbody2D rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public float GetGravityScale()
        {
            return rb != null ? rb.gravityScale : 1f;
        }

        /// <summary>
        /// 重力を反転させる（外からこれを呼ぶだけでOK）
        /// </summary>
        public void ReverseGravity()
        {
            if (rb != null)
            {
                // 現在の重力スケールをプラスマイナス逆転させる
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

                // プレイヤーの向き（上下）も反転させる
                Vector3 scale = transform.localScale;
                scale.y *= -1;
                transform.localScale = scale;
            }
        }
        public void SetGravityScale(float scale)
        {
            if (rb == null) return;

            rb.gravityScale = scale;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // 慣性リセット

            // 見た目の向きを重力に合わせる
            Vector3 localScale = transform.localScale;
            // 重力がマイナスならキャラも逆さま(-1)、プラスならそのまま(1)
            localScale.y = (scale < 0) ? -Mathf.Abs(localScale.y) : Mathf.Abs(localScale.y);
            transform.localScale = localScale;
        }

        // 正常な重力に戻す専用のショートカット
        public void SetNormalGravity()
        {
            SetGravityScale(1f);
        }
    }
}