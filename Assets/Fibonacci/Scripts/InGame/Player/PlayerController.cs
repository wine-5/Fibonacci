using UnityEngine;
using Fibonacci.InGame.Core;
using Fibonacci.InGame.Core.AreaGimmick;
using Fibonacci.Event;

namespace Fibonacci.InGame.Player
{
    /// <summary>
    /// プレイヤーの全体制御と状態管理を司る司令塔クラス。
    /// ゲームフェーズに応じた物理演算の切り替えや、エリア移動に伴う重力変化の実行、
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
        /// ゲームフェーズの変更通知を受け取り、物理シミュレーションの有効・無効を切り替えます。
        /// プレイ開始時には位置判定を強制更新し、最新のエリア効果を即座に反映させます。
        /// </summary>
        private void HandlePhaseChanged(GamePhase newPhase)
        {
            if (newPhase == GamePhase.Playing)
            {
                rb.simulated = true;

                if (playerCheck != null)
                {
                    playerCheck.ForceCheck();
                }
            }
            else if (newPhase == GamePhase.Drawing)
            {
                rb.simulated = false;

                if (abilityDisplayRenderer != null) abilityDisplayRenderer.enabled = false;
                isMovementLocked = false;

                AbilityManager.Instance.Reset();
            }
        }

        /// <summary>
        /// 所属エリアのインデックスに基づいて、プレイヤーに適用される重力反転などの能力を実行します。
        /// </summary>
        /// <param name="areaIndex">現在のプレイヤー位置に対応するエリア番号</param>
        public void ChangeAreaEffect(int areaIndex)
        {
            if (GameManager.Instance.CurrentPhase != GamePhase.Playing) return;

            rb.mass = 1.0f;
            playerMove.IsSlippery = false;

            AbilityType ability = AbilityManager.Instance.GetAbilityAt(areaIndex);

            bool isGravityInverted = ability == AbilityType.GravityInvert;
            gravityLogic.Apply(rb, transform, isGravityInverted);

            bool isMoveLocked = ability == AbilityType.MoveLock;
            isMovementLocked = isMoveLocked;
            playerMove.IsSlippery = isMoveLocked;
            moveLockLogic.Apply(rb, isMoveLocked, areaIndex, abilityDisplayRenderer);

            heavyLogic.Apply(rb, playerMove, ability == AbilityType.Heavy);

            lowGravityLogic.Apply(rb, ability == AbilityType.LowGravity);

            jumpLogic.Apply(ability == AbilityType.Jump);

            powerUpLogic.Apply(rb, ability == AbilityType.PowerUp);

            fireLogic.Apply(ability == AbilityType.Fire, areaIndex, abilityDisplayRenderer);

            if (abilityDisplayRenderer != null)
            {
                Sprite s = AbilityManager.Instance.GetAbilitySprite(ability);
                if (s != null)
                {
                    abilityDisplayRenderer.sprite = s;
                    abilityDisplayRenderer.enabled = true;
                }
                else
                {
                    abilityDisplayRenderer.enabled = false;
                }
            }
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

            if (playerCheck != null)
            {
                fireLogic.Tick(playerCheck.CurrentAreaIndex);
            }
        }

        /// <summary>
        /// 外部の入力管理クラスから移動ベクトルを受け取り、移動ロジックとアニメーションに反映させます。
        /// </summary>
        /// <param name="input">移動方向と強さを示すベクトル</param>
        public void SetMoveInput(Vector2 input)
        {
            playerMove.MoveInput = input;
            playerMove.UpdateAnimation();
        }

        public void OnJumpInput()
        {
            if (GameManager.Instance.CurrentPhase == GamePhase.Playing && jumpLogic.CanJump && playerMove.IsGrounded())
                playerMove.ExecuteJump(jumpPower);
        }

        private void OnDrawGizmos()
        {
            if (playerMove != null)
            {
                playerMove.DrawGizmos();
            }
        }

        /// <summary>
        /// プレイヤーの位置、速度、重力方向、および入力状態を初期状態にリセットします。
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

            if (GameManager.Instance.CurrentPhase != GamePhase.Playing)
            {
                rb.simulated = false;
            }
        }
    }
}