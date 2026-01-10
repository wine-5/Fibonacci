using UnityEngine;
using Fibonacci.Event;
using Fibonacci.InGame.BorderLine.UI;

namespace Fibonacci.InGame.Player
{
    /// <summary>
    /// プレイヤーの全体制御と状態管理を司る司令塔（Controller）クラス。
    /// ゲームフェーズに応じた動作の許可判定、PlayerMove（移動ロジック）のインスタンス管理、
    /// エリア変更に伴う重力操作やUIとの連携など、プレイヤーに関する各コンポーネントの仲介役として機能します。
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private PlayerGravity playerGravity;
        [SerializeField] private PlayerAnimationController animationController;
        [SerializeField] private BorderLineEffectUI effectUI;

        private PlayerMove playerMove;
        private Rigidbody2D rb;

        public string EffectIdArea0 { get; private set; } = "";
        public string EffectIdArea1 { get; private set; } = "";

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (playerGravity == null) playerGravity = GetComponent<PlayerGravity>();
            
            playerMove = new PlayerMove(rb, transform, animationController, moveSpeed);
        }

        void FixedUpdate()
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentPhase != GamePhase.Playing)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                return;
            }

            playerMove.ExecutePhysicsUpdate();
        }

        public void SetMoveInput(Vector2 input)
        {
            playerMove.MoveInput = input;
            playerMove.UpdateAnimation();
        }

        #region GameLogic (Gravity/Effects)
        // private void OnEnable()
        // {
        //     GameEvents.OnRestart += OnGameRestart;
        //     if (effectUI != null) effectUI.EffectClicked += OnEffectClicked;
        // }

        // private void OnDisable()
        // {
        //     GameEvents.OnRestart -= OnGameRestart;
        //     if (effectUI != null) effectUI.EffectClicked -= OnEffectClicked;
        // }

        private void OnGameRestart()
        {
            playerMove.ResetPosition();
            EffectIdArea0 = "";
            EffectIdArea1 = "";
        }

        public void OnAreaChanged(int newAreaIndex)
        {
            string currentEffect = (newAreaIndex == 0) ? EffectIdArea0 : EffectIdArea1;
            if (currentEffect == "ZeroGravity") playerGravity.SetGravityScale(-1f);
            else playerGravity.SetNormalGravity();
        }
        #endregion
    }
}