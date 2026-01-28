using UnityEngine;
using Fibonacci.InGame.Core;
using Fibonacci.InGame.Core.AreaGimmick;
using Fibonacci.Event;

namespace Fibonacci.InGame.Player
{
    /// <summary>
    /// プレイヤーの全体制御と状態管理を司る司令塔クラス。
    /// ゲームフェーズに応じた物理演算の切り替えや、エリア移動に伴うアビリティ効果の実行、
    /// 移動入力の仲介などを一括して管理します。
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerCheck))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpPower = 12f;
        [SerializeField] private PlayerAnimationController animationController;
        [SerializeField] private SpriteRenderer abilityDisplayRenderer;
        [SerializeField] private LayerMask groundLayer;

        private const float DEFAULT_MASS = 1.0f;
        private const float DEFAULT_DAMPING = 0f;

        private PlayerMove playerMove;
        private Rigidbody2D rb;
        private PlayerCheck playerCheck;
        private readonly GravityAbility gravityLogic = new GravityAbility();
        private readonly MoveLockAbility moveLockLogic = new MoveLockAbility();
        private readonly HeavyAbility heavyLogic = new HeavyAbility();
        private readonly LowGravityAbility lowGravityLogic = new LowGravityAbility();
        private readonly FireAbility fireLogic = new FireAbility();
        private readonly PowerUpAbility powerUpLogic = new PowerUpAbility();
        private readonly JumpAbility jumpLogic = new JumpAbility();
        private bool isMovementLocked = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            playerCheck = GetComponent<PlayerCheck>();
            playerMove = new PlayerMove(rb, transform, animationController, moveSpeed, groundLayer);
        }

        private void OnEnable()
        {
            GameEvents.OnPhaseChanged += HandlePhaseChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnPhaseChanged -= HandlePhaseChanged;
        }

        /// <summary>
        /// ゲームフェーズの変更通知を受け取り、プレイヤーの状態を切り替えます。
        /// </summary>
        private void HandlePhaseChanged(GamePhase newPhase)
        {
            if (newPhase == GamePhase.Playing)
            {
                rb.simulated = true;
                playerCheck.ForceCheck();
                return;
            }

            if (newPhase == GamePhase.Drawing)
            {
                rb.simulated = false;
                abilityDisplayRenderer.enabled = false;
                isMovementLocked = false;
                AbilityManager.Instance.Reset();
            }
        }

        /// <summary>
        /// 滞在エリアの能力タイプに基づき、重力、質量、移動制限などの特殊効果を順次適用します。
        /// </summary>
        public void ChangeAreaEffect(int areaIndex)
        {
            if (GameManager.Instance.CurrentPhase != GamePhase.Playing) return;

            rb.mass = DEFAULT_MASS;
            rb.linearDamping = DEFAULT_DAMPING;
            playerMove.ResetSpeed();
            playerMove.IsSlippery = false;

            AbilityType ability = AbilityManager.Instance.GetAbilityAt(areaIndex);

            gravityLogic.Apply(rb, transform, ability == AbilityType.GravityInvert);

            bool isMoveLocked = ability == AbilityType.MoveLock;
            isMovementLocked = isMoveLocked;
            playerMove.IsSlippery = isMoveLocked;
            moveLockLogic.Apply(rb, isMoveLocked);

            heavyLogic.Apply(rb, playerMove, ability == AbilityType.Heavy);
            lowGravityLogic.Apply(rb, ability == AbilityType.LowGravity);
            jumpLogic.Apply(ability == AbilityType.Jump);
            powerUpLogic.Apply(rb, ability == AbilityType.PowerUp);
            fireLogic.Apply(ability == AbilityType.Fire, abilityDisplayRenderer);

            Sprite abilitySprite = AbilityManager.Instance.GetAbilitySprite(ability);
            if (abilitySprite != null)
            {
                abilityDisplayRenderer.sprite = abilitySprite;
                abilityDisplayRenderer.enabled = true;
                return;
            }
            
            abilityDisplayRenderer.enabled = false;
        }

        private void FixedUpdate()
        {
            bool isPlaying = GameManager.Instance.CurrentPhase == GamePhase.Playing;
            rb.simulated = isPlaying;

            if (!isPlaying) return;

            if (!isMovementLocked)
            {
                playerMove.ExecutePhysicsUpdate();
            }

            fireLogic.Tick(playerCheck.CurrentAreaIndex);
        }

        /// <summary>
        /// 入力ベクトルを移動ロジックに反映させます。
        /// </summary>
        public void SetMoveInput(Vector2 input)
        {
            playerMove.MoveInput = input;
            playerMove.UpdateAnimation();
        }

        /// <summary>
        /// ジャンプ入力時の条件判定と実行を行います。
        /// </summary>
        public void OnJumpInput()
        {
            if (GameManager.Instance.CurrentPhase == GamePhase.Playing && jumpLogic.CanJump && playerMove.IsGrounded())
            {
                playerMove.ExecuteJump(jumpPower);
            }
        }

        private void OnDrawGizmos()
        {
            playerMove?.DrawGizmos();
        }

        /// <summary>
        /// プレイヤーの物理状態、位置、重力を初期状態に戻します。
        /// </summary>
        public void ResetPlayerState()
        {
            playerMove.ResetPosition();
            playerMove.MoveInput = Vector2.zero;
            playerMove.UpdateAnimation();

            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;

            gravityLogic.Apply(rb, transform, false);
        }
    }
}