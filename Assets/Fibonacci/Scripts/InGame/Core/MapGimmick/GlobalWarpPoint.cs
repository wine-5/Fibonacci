using UnityEngine;

namespace Fibonacci.InGame.Core.MapGimmick
{
    /// <summary>
    /// マップ上の特定地点間でプレイヤーを転送するワープギミックを管理するクラス。
    /// 全ワープポイント共通のクールタイムを静的に保持し、連続移動による無限ループを防止します。
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class GlobalWarpPoint : MonoBehaviour
    {
        private const float DEFAULT_COOLDOWN = 2.0f;
        private const string PLAYER_TAG = "Player";

        [Header("ワープ設定")]
        [SerializeField] private Transform targetLocation;
        [SerializeField] private float globalCooldownTime = DEFAULT_COOLDOWN;
        [SerializeField] private bool isExitOnly = false;
        [SerializeField] private bool maintainVelocity = false;

        private static float nextWarpAllowedTime = 0f;

        /// <summary>
        /// プレイヤーの進入を検知し、クールタイム中でなければワープ処理を実行します。
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isExitOnly) return;
            if (!other.CompareTag(PLAYER_TAG) || Time.time < nextWarpAllowedTime) return;

            PerformWarp(other.gameObject);
        }

        /// <summary>
        /// プレイヤーの座標をターゲット地点へ移動させ、クールタイムを更新します。
        /// </summary>
        private void PerformWarp(GameObject player)
        {
            nextWarpAllowedTime = Time.time + globalCooldownTime;
            player.transform.position = targetLocation.position;

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null && !maintainVelocity)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}