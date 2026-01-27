using UnityEngine;

namespace Fibonacci.InGame.Core.MapGimmick
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PushBlock : MonoBehaviour
    {
        [Header("設定")]
        [SerializeField] private float requiredMass = 2.0f; 

        private Rigidbody2D _rb;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            LockBlock();
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            
            if (playerRb != null && playerRb.mass >= requiredMass)
            {
                UnlockBlock();
            }
            else
            {
                LockBlock();
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            LockBlock();
        }

        private void LockBlock()
        {
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;
            _rb.linearVelocity = Vector2.zero;
        }

        private void UnlockBlock()
        {
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
}