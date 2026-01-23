using UnityEngine;

namespace Fibonacci.InGame.Core.MapGimmick
{
    [RequireComponent(typeof(Collider2D))]
    public class GlobalWarpPoint : MonoBehaviour
    {
        private const float DEFAULT_COOLDOWN = 2.0f;

        [Header("ワープ設定")]
        [SerializeField] private Transform targetLocation;
        [SerializeField] private float globalCooldownTime = DEFAULT_COOLDOWN;
        [SerializeField] private bool isExitOnly = false;

        [Header("演出設定")]
        [SerializeField] private bool maintainVelocity = false;
        [SerializeField] private ParticleSystem warpEffect;

        private static float _nextWarpAllowedTime = 0f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isExitOnly) return;

            if (other.CompareTag("Player") && Time.time >= _nextWarpAllowedTime)
            {
                PerformWarp(other.gameObject);
            }
        }

        private void PerformWarp(GameObject player)
        {
            if (targetLocation == null) return;

            _nextWarpAllowedTime = Time.time + globalCooldownTime;

            player.transform.position = targetLocation.position;

            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null && !maintainVelocity)
            {
                rb.linearVelocity = Vector2.zero;
            }

            if (warpEffect != null)
            {
                Instantiate(warpEffect, targetLocation.position, Quaternion.identity);
            }
        }
    }
}