using UnityEngine;

namespace Fibonacci.InGame.Core.MapGimmick
{
    [RequireComponent(typeof(Collider2D))]
    public class GoFly : MonoBehaviour
    {
        [Header("吹っ飛ばし設定")]
        [SerializeField] private float launchForce = 15f; 
        [SerializeField] private ParticleSystem launchEffect;
        [Range(0f, 1f)]
        [SerializeField] private float upwardBias = 0.7f; // 上方向への強さの割合

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
                float moveDirection = rb.linearVelocity.x >= 0 ? 1f : -1f;

                Vector2 launchVector = new Vector2(moveDirection, upwardBias).normalized;

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