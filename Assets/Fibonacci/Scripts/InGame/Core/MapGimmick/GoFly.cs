using UnityEngine;
using Fibonacci.InGame.Player;

namespace Fibonacci.InGame.Core.MapGimmick
{
    /// <summary>
    /// プレイヤーを特定の角度と強さで吹き飛ばすジャンプ台ギミックを制御するクラス。
    /// 入力された角度に基づき、プレイヤーの進行方向を維持したまま上空へ射出します。
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class GoFly : MonoBehaviour
    {
        private const float DIRECTION_THRESHOLD = 0f;

        [Header("吹っ飛ばし設定")]
        [SerializeField] private float launchForce = 15f; 

        [Header("角度設定 (45度なら斜め45度)")]
        [Range(0f, 90f)]
        [SerializeField] private float launchAngle = 45f;

        private Vector2 cachedBaseVector;
        private bool isVectorCached = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(GameConstants.TAG_PLAYER)) return;

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

            float moveDirection = rb.linearVelocity.x >= DIRECTION_THRESHOLD 
                ? GameConstants.DIRECTION_RIGHT 
                : GameConstants.DIRECTION_LEFT;

            Vector2 finalLaunchVector = new Vector2(cachedBaseVector.x * moveDirection, cachedBaseVector.y);

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(finalLaunchVector * launchForce, ForceMode2D.Impulse);
        }

        private void OnValidate()
        {
            isVectorCached = false;
        }
    }
}