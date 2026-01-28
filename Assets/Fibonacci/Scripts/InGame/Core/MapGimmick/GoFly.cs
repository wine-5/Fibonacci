using UnityEngine;

namespace Fibonacci.InGame.Core.MapGimmick
{
    /// <summary>
    /// プレイヤーを特定の角度と強さで吹き飛ばすジャンプ台ギミックを制御するクラス。
    /// 入力された角度に基づき、プレイヤーの進行方向を維持したまま上空へ射出します。
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class GoFly : MonoBehaviour
    {
        private const string PLAYER_TAG = "Player";
        private const float DIRECTION_THRESHOLD = 0f;

        [Header("吹っ飛ばし設定")]
        [SerializeField] private float launchForce = 15f; 

        [Header("角度設定 (45度なら斜め45度)")]
        [Range(0f, 90f)]
        [SerializeField] private float launchAngle = 45f;

        private Vector2 cachedBaseVector;
        private bool isVectorCached = false;

        /// <summary>
        /// プレイヤーの接触を検知し、射出処理を開始します。
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(PLAYER_TAG)) return;

            Launch(other.gameObject);
        }

        /// <summary>
        /// プレイヤーの進行方向に基づいて射出ベクトルを計算し、瞬間的な力を加えます。
        /// </summary>
        private void Launch(GameObject player)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb == null) return;

            if (!isVectorCached)
            {
                float angleRad = launchAngle * Mathf.Deg2Rad;
                cachedBaseVector = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
                isVectorCached = true;
            }

            float moveDirection = rb.linearVelocity.x >= DIRECTION_THRESHOLD ? 1f : -1f;
            Vector2 finalLaunchVector = new Vector2(cachedBaseVector.x * moveDirection, cachedBaseVector.y);

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(finalLaunchVector * launchForce, ForceMode2D.Impulse);
        }

        /// <summary>
        /// インスペクターで値が変更された際、キャッシュされたベクトルをリセットします。
        /// </summary>
        private void OnValidate()
        {
            isVectorCached = false;
        }
    }
}