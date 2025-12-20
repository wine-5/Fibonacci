using UnityEngine;
using UnityEngine.InputSystem;

namespace Fibonacci.Player
{
    [RequireComponent(typeof(Rigidbody2D))] // 2D用に変更

    public class PlayerMove : MonoBehaviour
    {
        // === 設定パラメータ ===
        [Header("移動設定")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 7f;

        [Header("接地判定")]
        [Tooltip("足元からRayを飛ばす距離")]
        [SerializeField] private float rayDistance = 1.1f;
        [SerializeField] private LayerMask groundLayer;

        [Header("Input System設定")]
        [SerializeField] private InputActionReference moveActionRef;
        [SerializeField] private InputActionReference jumpActionRef;

        // === プライベート変数 ===
        private Rigidbody2D rb; // 2D用に変更
        private Vector2 moveInput;
        private bool isGrounded;

        void Awake()
        {
            // Rigidbody2Dコンポーネントの参照を取得
            rb = GetComponent<Rigidbody2D>();
        }

        // === Input System の有効化/無効化 ===
        void OnEnable()
        {
            moveActionRef.action.Enable();
            jumpActionRef.action.Enable();

            // ジャンプアクションの登録をコメントアウト
            // jumpActionRef.action.performed += OnJump;
        }

        void OnDisable()
        {
            // イベント解除をコメントアウト
            // jumpActionRef.action.performed -= OnJump;
            
            moveActionRef.action.Disable();
            jumpActionRef.action.Disable();
        }

        // === 毎フレームの処理 ===
        void Update()
        {
            // Input Systemから移動入力値 (Vector2) を読み取る
            moveInput = moveActionRef.action.ReadValue<Vector2>();

            // 接地判定 (Raycastを真下に飛ばす) - 2Dでも継続
            isGrounded = CheckIsGrounded();
        }

        // === 物理演算の処理 ===
        void FixedUpdate()
        {
            HandleMovement();
        }

        // === メインロジック ===

        /// <summary>
        /// 横移動処理と向きの制御を行います。
        /// </summary>
        private void HandleMovement()
        {
            // 2D用にVector2を使用。Z軸の考慮が不要になります
            Vector2 targetVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

            // 速度を設定
            rb.linearVelocity = targetVelocity;

            // キャラクターの向きを制御
            if (moveInput.x != 0)
            {
                // 2Dの場合、Y軸を90度回転させると画像が消えてしまうため、
                // 一般的な「右向き(0度)」「左向き(180度)」に調整しています
                float yRotation = moveInput.x > 0 ? 0f : 180f;
                transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            }
        }

        /* /// <summary>
        /// ジャンプアクションが実行されたときに呼び出されます。
        /// </summary>
        private void OnJump(InputAction.CallbackContext context)
        {
            // 接地している場合のみジャンプ実行
            if (isGrounded)
            {
                // 既存のY速度をリセット（0にする）してから力を加える
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

                // 瞬間的な力 (Impulse) を上方向に追加
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
        */

        /// <summary>
        /// Raycast2Dを使用して接地判定を行います。
        /// </summary>
        /// <returns>地面に接地していれば true</returns>
        private bool CheckIsGrounded()
        {
            // Physics2D.Raycastを使用
            // 2DのRaycastは「衝突結果」を返すため、コライダーが存在するかで判定します
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayDistance, groundLayer);
            return hit.collider != null;
        }
    }
}