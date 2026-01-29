using UnityEngine;

namespace Fibonacci.InGame.Player
{
    /// <summary>
    /// プレイヤーの移動、物理挙動、および向きの制御に関する純粋な計算ロジックを管理するクラス。
    /// </summary>
    public class PlayerMove
    {
        private const float ROTATION_RIGHT = 0f;
        private const float ROTATION_LEFT = 180f;
        private const float MOVE_THRESHOLD = 0.01f;
        private const float GROUND_CHECK_OFFSET_Y = 0.9f;
        private const float GROUND_CHECK_WIDTH = 0.5f;
        private const float GROUND_CHECK_HEIGHT = 0.1f;

        private readonly LayerMask groundLayer;
        private readonly float defaultSpeed;
        private readonly Rigidbody2D rb;
        private readonly Transform transform;
        private readonly PlayerAnimationController anim;
        private readonly Vector3 initialPosition;
        
        private float moveSpeed;

        public Vector2 MoveInput { get; set; }
        public bool IsSlippery { get; set; }

        public PlayerMove(Rigidbody2D rb, Transform transform, PlayerAnimationController anim, float speed, LayerMask groundLayer)
        {
            this.rb = rb;
            this.transform = transform;
            this.anim = anim;
            this.groundLayer = groundLayer;
            defaultSpeed = speed;
            moveSpeed = speed;
            initialPosition = transform.position;
        }

        /// <summary>
        /// 物理更新のメイン処理。各責務ごとのサブメソッドを呼び出します。
        /// </summary>
        public void ExecutePhysicsUpdate()
        {
            ApplyHorizontalMovement();
            ApplyRotation();
        }

        /// <summary>
        /// 水平方向の速度を適用します。入力がないかつ滑り状態でなければ停止させます。
        /// </summary>
        private void ApplyHorizontalMovement()
        {
            bool hasInput = Mathf.Abs(MoveInput.x) > MOVE_THRESHOLD;

            if (hasInput)
            {
                rb.linearVelocity = new Vector2(MoveInput.x * moveSpeed, rb.linearVelocity.y);
            }
            else if (!IsSlippery)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }
        }

        /// <summary>
        /// 移動入力に基づいてキャラクターの向きを更新します。
        /// </summary>
        private void ApplyRotation()
        {
            if (Mathf.Abs(MoveInput.x) <= MOVE_THRESHOLD) return;

            float yRotation = MoveInput.x > 0 ? ROTATION_RIGHT : ROTATION_LEFT;
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }

        public void ExecuteJump(float force)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
        }

        public bool IsGrounded()
        {
            Vector2 footPosition = GetFootPosition();
            return Physics2D.OverlapBox(footPosition, new Vector2(GROUND_CHECK_WIDTH, GROUND_CHECK_HEIGHT), 0f, groundLayer);
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(GetFootPosition(), new Vector2(GROUND_CHECK_WIDTH, GROUND_CHECK_HEIGHT));
        }

        private Vector2 GetFootPosition()
        {
            return (Vector2)transform.position + Vector2.down * GROUND_CHECK_OFFSET_Y;
        }

        public void SetCurrentSpeed(float speed) => moveSpeed = speed;
        public void ResetSpeed() => moveSpeed = defaultSpeed;
        public void UpdateAnimation() => anim.UpdateMoveAnimation(MoveInput);

        public void ResetPosition()
        {
            transform.position = initialPosition;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            MoveInput = Vector2.zero;
        }
    }
}