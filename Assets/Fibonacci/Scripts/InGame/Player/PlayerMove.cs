using UnityEngine;

namespace Fibonacci.Player
{
    /// <summary>
    /// プレイヤーの移動処理を管理するクラス
    /// 物理的な移動、向き変更、接地判定のみを担当
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMove : MonoBehaviour
    {
        [Header("移動設定")]
        [SerializeField] private float moveSpeed = 5f;

        [Header("接地判定")]
        [Tooltip("足元からの判定開始位置のオフセット")]
        [SerializeField] private float groundCheckOffset = 0.5f;
        [Tooltip("足元から地面までの判定距離")]
        [SerializeField] private float rayDistance = 0.6f;
        [Tooltip("接地判定のボックスサイズ")]
        [SerializeField] private Vector2 groundCheckSize = new Vector2(0.8f, 0.1f);
        [SerializeField] private LayerMask groundLayer;

        // === プライベート変数 ===
        private Rigidbody2D rb;
        private Vector2 moveInput = Vector2.zero;
        private bool isGrounded;
        private Vector3 initialPosition;

        public bool IsGrounded => isGrounded;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            initialPosition = transform.position;
        }

        void Update()
        {
            isGrounded = CheckIsGrounded();
        }

        void FixedUpdate()
        {
            HandleMovement();
        }

        /// <summary>
        /// 入力を受け取って移動処理を行う
        /// PlayerControllerから直接呼び出される
        /// </summary>
        public void OnMoveInput(Vector2 input)
        {
            moveInput = input;
        }

        /// <summary>
        /// 横移動処理と向きの制御を行います。
        /// </summary>
        private void HandleMovement()
        {
            Vector2 targetVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
            rb.linearVelocity = targetVelocity;

            // キャラクターの向きを制御
            if (moveInput.x != 0)
            {
                float yRotation = moveInput.x > 0 ? 0f : 180f;
                transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            }
        }

        /// <summary>
        /// BoxCastを使用して接地判定を行います。
        /// </summary>
        private bool CheckIsGrounded()
        {
            Vector2 boxCenter = (Vector2)transform.position + Vector2.down * groundCheckOffset;
            RaycastHit2D hit = Physics2D.BoxCast(boxCenter, groundCheckSize, 0f, Vector2.down, rayDistance, groundLayer);
            return hit.collider != null;
        }

        /// <summary>
        /// 初期位置にリセットし、速度をゼロにする
        /// PlayerControllerから呼び出される
        /// </summary>
        public void ResetPosition()
        {
            transform.position = initialPosition;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            moveInput = Vector2.zero;
        }
    }
}