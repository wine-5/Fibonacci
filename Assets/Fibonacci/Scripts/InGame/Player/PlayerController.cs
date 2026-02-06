using UnityEngine;
using Fibonacci.InGame.Core;
using Fibonacci.InGame.Core.AreaGimmick;
using Fibonacci.Event;

namespace Fibonacci.InGame.Player
{
    /// <summary>
    /// プレイヤーの全体制御と状態管理を司る司令塔クラス。
    /// 各種アビリティから受け取った計算結果を物理演算や移動ロジックに適用します。
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
        private const float DEFAULT_GRAVITY_SCALE = 1.0f;

        private PlayerMove playerMove;
        private Rigidbody2D rb;
        private PlayerCheck playerCheck;

        private readonly GravityAbility gravityLogic = new();
        private readonly MoveLockAbility moveLockLogic = new();
        private readonly HeavyAbility heavyLogic = new();
        private readonly LowGravityAbility lowGravityLogic = new();
        private readonly FireAbility fireLogic = new();
        private readonly PowerUpAbility powerUpLogic = new();
        private readonly JumpAbility jumpLogic = new();

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

        private void HandlePhaseChanged(GamePhase newPhase)
        {
            if (newPhase == GamePhase.Paused) return;

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
                fireLogic.Reset();
                if (AbilityManager.Instance != null)
                {
                    AbilityManager.Instance.ResetAbilities();
                }
            }
        }

        /// <summary>
        /// 進入したエリアの効果をプレイヤーに適用する。
        /// </summary>
        public void ChangeAreaEffect(int areaIndex)
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentPhase != GamePhase.Playing) return;

            ResetToDefaultState();

            if (AbilityManager.Instance == null) return;
            AbilityType ability = AbilityManager.Instance.GetAbilityAt(areaIndex);

            ApplyAbilities(ability);
            UpdateAbilityVisual(ability);
        }

        private void ResetToDefaultState()
        {
            rb.mass = DEFAULT_MASS;
            rb.linearDamping = DEFAULT_DAMPING;
            rb.gravityScale = DEFAULT_GRAVITY_SCALE;

            Vector3 scale = transform.localScale;
            scale.y = Mathf.Abs(scale.y);
            transform.localScale = scale;

            playerMove.ResetSpeed();
            playerMove.IsSlippery = false;
            isMovementLocked = false;
        }

        private void ApplyAbilities(AbilityType ability)
        {
            ApplyHeavy(ability);
            ApplyLowGravity(ability);
            ApplyPowerUp(ability);
            ApplyGravityInvert(ability);
            ApplyMoveLock(ability);
            jumpLogic.Apply(ability == AbilityType.Jump);
        }

        private void ApplyHeavy(AbilityType ability)
        {
            var effect = heavyLogic.GetAppliedValues(ability == AbilityType.Heavy);
            if (effect.HasValue)
            {
                rb.mass = effect.Value.mass;
                playerMove.SetCurrentSpeed(effect.Value.speed);
            }
        }

        private void ApplyLowGravity(AbilityType ability)
        {
            var drag = lowGravityLogic.GetAppliedDrag(ability == AbilityType.LowGravity);
            if (drag.HasValue) rb.linearDamping = drag.Value;
        }

        private void ApplyPowerUp(AbilityType ability)
        {
            var mass = powerUpLogic.GetAppliedMass(ability == AbilityType.PowerUp);
            if (mass.HasValue) rb.mass = mass.Value;
        }

        private void ApplyGravityInvert(AbilityType ability)
        {
            var effect = gravityLogic.GetAppliedScales(DEFAULT_GRAVITY_SCALE, ability == AbilityType.GravityInvert);
            rb.gravityScale = effect.gravityScale;

            Vector3 localScale = transform.localScale;
            localScale.y = Mathf.Abs(localScale.y) * effect.visualScaleY;
            transform.localScale = localScale;
        }

        private void ApplyMoveLock(AbilityType ability)
        {
            var damping = moveLockLogic.GetAppliedDamping(ability == AbilityType.MoveLock);
            if (damping.HasValue)
            {
                rb.linearDamping = damping.Value;
                isMovementLocked = true;
                playerMove.IsSlippery = true;
            }
        }

        private void UpdateAbilityVisual(AbilityType ability)
        {
            if (AbilityManager.Instance == null) return;
            Sprite abilitySprite = AbilityManager.Instance.GetAbilitySprite(ability);
            bool hasSprite = abilitySprite != null;

            abilityDisplayRenderer.sprite = abilitySprite;
            abilityDisplayRenderer.enabled = hasSprite;
        }

        private void FixedUpdate()
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentPhase != GamePhase.Playing)
            {
                rb.simulated = false;
                return;
            }

            rb.simulated = true;

            if (!isMovementLocked)
            {
                playerMove.ExecutePhysicsUpdate();
            }

            if (AbilityManager.Instance != null)
            {
                AbilityType currentAbility = AbilityManager.Instance.GetAbilityAt(playerCheck.CurrentAreaIndex);
                if (fireLogic.Tick(currentAbility == AbilityType.Fire, Time.fixedDeltaTime))
                {
                    GameEvents.TriggerRestart();
                }
            }
        }

        /// <summary>
        /// 入力系から受け取った移動ベクトルをセットする。
        /// </summary>
        public void SetMoveInput(Vector2 input)
        {
            playerMove.MoveInput = input;
            playerMove.UpdateAnimation();
        }

        /// <summary>
        /// ジャンプ入力実行時の処理。
        /// </summary>
        public void OnJumpInput()
        {
            if (GameManager.Instance == null) return;

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
        /// プレイヤーの位置、物理、および状態を初期状態にリセットする。
        /// </summary>
        public void ResetPlayerState()
        {
            playerMove.ResetPosition();
            playerMove.MoveInput = Vector2.zero;
            playerMove.UpdateAnimation();

            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;

            ResetToDefaultState();
        }
    }
}