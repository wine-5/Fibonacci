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

        void Start()
        {
            // テスト用：開始時に一度反転させてみる
            ReverseGravity();
        }

        /// <summary>
        /// 重力を反転させる（外からこれを呼ぶだけでOK）
        /// </summary>
        public void ReverseGravity()
        {
            if (rb != null)
            {
                // 現在の重力スケールをプラスマイナス逆転させる
                rb.gravityScale *= -1;

                // プレイヤーの向き（上下）も反転させる
                Vector3 scale = transform.localScale;
                scale.y *= -1;
                transform.localScale = scale;

                Debug.Log($"重力を反転しました。現在のScale: {rb.gravityScale}");
            }
        }
    }
}