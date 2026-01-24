using UnityEngine;

namespace Fibonacci.InGame.Core.MapGimmick
{
    [RequireComponent(typeof(Collider2D))]
    public class GoFly : MonoBehaviour
    {
        [Header("吹っ飛ばし設定")]
        [SerializeField] private float launchForce = 15f; 
        [SerializeField] private ParticleSystem launchEffect;

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
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

                rb.AddForce(transform.up * launchForce, ForceMode2D.Impulse);
            }

            if (launchEffect != null)
            {
                launchEffect.Play();
            }
        }
    }
}