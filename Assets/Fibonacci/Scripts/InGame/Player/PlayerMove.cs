using UnityEngine;

namespace Fibonacci.InGame.Player
{
    /// <summary>
    /// プレイヤーの移動、物理挙動、および向きの制御に関する純粋な計算ロジックを管理するクラス。
    /// Rigidbody2D を直接操作して速度を適用し、入力方向に基づいたキャラクターの回転処理を行います。
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

        /// <summary>
        /// 現在の移動入力ベクトルを取得または設定します。
        /// </summary>
        public Vector2 MoveInput { get; set; }

        /// <summary>
        /// 滑りやすい床の状態かどうかを取得または設定します。
        /// </summary>
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
        /// 水平方向の移動速度を適用し、入力方向に応じてキャラクターの向きを更新します。
        /// </summary>
        public void ExecutePhysicsUpdate()
        {
            if (Mathf.Abs(MoveInput.x) > MOVE_THRESHOLD)
            {
                rb.linearVelocity = new Vector2(MoveInput.x * moveSpeed, rb.linearVelocity.y);

                float yRotation = MoveInput.x > 0 ? ROTATION_RIGHT : ROTATION_LEFT;
                transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
                return;
            }

            if (!IsSlippery)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }
        }

        /// <summary>
        /// 指定された力でジャンプ処理を実行します。
        /// </summary>
        public void ExecuteJump(float force)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
        }

        /// <summary>
        /// 接地判定用のデバッグギズモを描画します。
        /// </summary>
        public void DrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector2 footPosition = (Vector2)transform.position + Vector2.down * GROUND_CHECK_OFFSET_Y;
            Gizmos.DrawWireCube(footPosition, new Vector2(GROUND_CHECK_WIDTH, GROUND_CHECK_HEIGHT));
        }

        /// <summary>
        /// プレイヤーの移動速度を動的に変更します。
        /// </summary>
        public void SetCurrentSpeed(float speed)
        {
            moveSpeed = speed;
        }

        /// <summary>
        /// 現在の移動入力状態をアニメーションコントローラーに通知します。
        /// </summary>
        public void UpdateAnimation()
        {
            anim.UpdateMoveAnimation(MoveInput);
        }

        /// <summary>
        /// 足元のコライダを検知して接地状態を判定します。
        /// </summary>
        public bool IsGrounded()
        {
            Vector2 footPosition = (Vector2)transform.position + Vector2.down * GROUND_CHECK_OFFSET_Y;
            return Physics2D.OverlapBox(footPosition, new Vector2(GROUND_CHECK_WIDTH, GROUND_CHECK_HEIGHT), 0f, groundLayer);
        }

        /// <summary>
        /// 移動速度をデフォルト値にリセットします。
        /// </summary>
        public void ResetSpeed()
        {
            moveSpeed = defaultSpeed;
        }

        /// <summary>
        /// プレイヤーの位置、物理速度、入力を初期状態にリセットします。
        /// </summary>
        public void ResetPosition()
        {
            transform.position = initialPosition;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            MoveInput = Vector2.zero;
        }
    }
}