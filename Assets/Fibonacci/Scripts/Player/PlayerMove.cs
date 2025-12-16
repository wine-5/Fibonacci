using UnityEngine;
using UnityEngine.InputSystem;



namespace Fibonacci.Player
{
    [RequireComponent(typeof(Rigidbody))]

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
        private Rigidbody rb;
        private Vector2 moveInput;
        private bool isGrounded;

        void Awake()
        {
            // Rigidbodyコンポーネントの参照を取得
            rb = GetComponent<Rigidbody>();
        }

        // === Input System の有効化/無効化 ===
        void OnEnable()
        {
            moveActionRef.action.Enable();
            jumpActionRef.action.Enable();

            // ジャンプアクションが実行された時のイベントを登録
            jumpActionRef.action.performed += OnJump;
        }

        void OnDisable()
        {
            // イベント解除と無効化
            jumpActionRef.action.performed -= OnJump;
            moveActionRef.action.Disable();
            jumpActionRef.action.Disable();
        }

        // === 毎フレームの処理 ===
        void Update()
        {
            // Input Systemから移動入力値 (Vector2) を読み取る
            moveInput = moveActionRef.action.ReadValue<Vector2>();

            // 接地判定 (Raycastを真下に飛ばす)
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
            // 入力のX成分 (左右) のみを使用し、Y成分 (ジャンプ/落下速度) はRigidbodyの現在の速度を維持し、Zは0に固定
            Vector3 targetVelocity = new Vector3(moveInput.x * moveSpeed, rb.linearVelocity.y, 0f);

            // 速度を直接設定して移動
            rb.linearVelocity = targetVelocity;

            // キャラクターの向きを制御
            if (moveInput.x != 0)
            {
                // 入力方向に合わせてY軸を回転させる（左右の振り向き）
                float yRotation = moveInput.x > 0 ? 90f : -90f;
                transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            }
        }

        /// <summary>
        /// ジャンプアクションが実行されたときに呼び出されます。
        /// </summary>
        private void OnJump(InputAction.CallbackContext context)
        {
            // 接地している場合のみジャンプ実行
            if (isGrounded)
            {
                // 既存のY速度をリセット（0にする）してから力を加えることで、ジャンプの挙動を安定させる
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0f);

                // 瞬間的な力 (Impulse) を上方向に追加
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }

        /// <summary>
        /// Raycastを使用して接地判定を行います。
        /// </summary>
        /// <returns>地面に接地していれば true</returns>
        private bool CheckIsGrounded()
        {
            // プレイヤーの中心から下方向にRayを飛ばし、指定されたLayerMaskのオブジェクトに当たっているかを判定
            return Physics.Raycast(transform.position, Vector3.down, rayDistance, groundLayer);
        }
    }
}