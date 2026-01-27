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

        private readonly LayerMask groundLayer;
        private readonly float checkHeight = 0.1f;
        private float moveSpeed;
        private readonly Rigidbody2D rb;
        private readonly Transform transform;
        private readonly PlayerAnimationController anim;
        private readonly Vector3 initialPosition;


        /// <summary>
        /// 現在の移動入力ベクトルを取得または設定します。
        /// </summary>
        public Vector2 MoveInput { get; set; }
        public bool IsSlippery { get; set; }

        public PlayerMove(Rigidbody2D rb, Transform transform, PlayerAnimationController anim, float speed, LayerMask groundLayer)
        {
            this.rb = rb;
            this.transform = transform;
            this.anim = anim;
            this.groundLayer = groundLayer;
            moveSpeed = speed;
            initialPosition = transform.position;
        }

        /// <summary>
        /// FixedUpdate タイミングで呼ばれ、水平方向の移動速度を適用し、
        /// 入力方向に応じてキャラクターの向き（Y軸回転）を更新します。
        /// </summary>
        public void ExecutePhysicsUpdate()
        {
            if (Mathf.Abs(MoveInput.x) > 0.01f)
            {
                rb.linearVelocity = new Vector2(MoveInput.x * moveSpeed, rb.linearVelocity.y);

                float yRotation = MoveInput.x > 0 ? ROTATION_RIGHT : ROTATION_LEFT;
                transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            }
            else if (!IsSlippery)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }
        }

        public void ExecuteJump(float force)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector2 footPosition = (Vector2)transform.position + Vector2.down * 0.9f;
            Gizmos.DrawWireCube(footPosition, new Vector2(0.5f, checkHeight));
        }

        /// <summary>
        /// プレイヤーの移動速度を動的に変更します。
        /// エリアギミックやアビリティによる鈍化・加速効果を適用する際に使用します。
        /// </summary>
        public void SetCurrentSpeed(float speed)
        {
            moveSpeed = speed;
        }

        /// <summary>
        /// 現在の移動入力状態をアニメーションコントローラーに通知し、歩行・待機アニメーションを更新します。
        /// </summary>
        public void UpdateAnimation()
        {
            if (anim != null)
            {
                anim.UpdateMoveAnimation(MoveInput);
            }
        }

        public bool IsGrounded()
        {
            Vector2 footPosition = (Vector2)transform.position + Vector2.down * 0.9f;
            Collider2D hit = Physics2D.OverlapBox(footPosition, new Vector2(0.5f, checkHeight), 0f, groundLayer);
            return hit != null;
        }

        /// <summary>
        /// プレイヤーを初期配置座標に戻し、物理的な速度、回転、および入力を完全にリセットします。
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