using UnityEngine;

namespace Fibonacci.InGame.Core.MapGimmick
{
    [RequireComponent(typeof(Collider2D))]
    public class GoFly : MonoBehaviour
    {
        [Header("吹っ飛ばし設定")]
        [SerializeField] private float launchForce = 15f; 
        [SerializeField] private ParticleSystem launchEffect;

        [Header("角度設定 (45度なら斜め45度)")]
        [Range(0f, 90f)]
        [SerializeField] private float launchAngle = 45f; 

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Launch(other.gameObject);
            }
        }

        private void Launch(GameObject player)
        {
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // プレイヤーが進んでいる方向（右なら1, 左なら-1）
                float moveDirection = rb.linearVelocity.x >= 0 ? 1f : -1f;

                // 角度をラジアンに変換してベクトルを計算
                float angleRad = launchAngle * Mathf.Deg2Rad;
                Vector2 launchVector = new Vector2(Mathf.Cos(angleRad) * moveDirection, Mathf.Sin(angleRad));

                // 既存の縦方向の速度をリセットして、上書きする力を安定させる
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

                rb.AddForce(launchVector * launchForce, ForceMode2D.Impulse);
            }

            if (launchEffect != null)
            {
                launchEffect.Play();
            }
        }
    }
}