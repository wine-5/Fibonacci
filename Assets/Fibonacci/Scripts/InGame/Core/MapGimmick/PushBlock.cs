using UnityEngine;
using Fibonacci.Event;

namespace Fibonacci.InGame.Core.MapGimmick
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PushBlock : MonoBehaviour
    {
        [Header("設定")]
        [SerializeField] private float requiredMass = 2.0f;

        private Rigidbody2D _rb;
        private Vector3 _initialPosition;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            _initialPosition = transform.position;
            LockBlock();
        }

        private void OnEnable()
        {
            GameEvents.OnRestart += ResetBlock;
        }

        private void OnDisable()
        {
            GameEvents.OnRestart -= ResetBlock;
        }

        private void ResetBlock()
        {
            _rb.simulated = false;

            transform.position = _initialPosition;

            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;

            _rb.simulated = true;
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
            _rb.angularVelocity = 0f;
        }

        private void UnlockBlock()
        {
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
}