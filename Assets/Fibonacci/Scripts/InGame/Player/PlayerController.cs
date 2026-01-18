using UnityEngine;
using Fibonacci.InGame.Core;
using Fibonacci.Event; // GameEvents を使用するために追加

namespace Fibonacci.InGame.Player
{
    /// <summary>
    /// プレイヤーの全体制御と状態管理を司る司令塔クラス。
    /// 効果選択フェーズ中の完全フリーズと、プレイ開始時の即時反映を制御します。
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private PlayerAnimationController animationController;

        private PlayerMove playerMove;
        private Rigidbody2D rb;
        private PlayerCheck playerCheck; // 即時適用の通知用
        private readonly PlayerGravityLogic gravityLogic = new PlayerGravityLogic();

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            playerCheck = GetComponent<PlayerCheck>();
            playerMove = new PlayerMove(rb, transform, animationController, moveSpeed);
        }

        private void OnEnable()
        {
            // フェーズ変更イベントを購読
            GameEvents.OnPhaseChanged += HandlePhaseChanged;
        }

        private void OnDisable()
        {
            // メモリリーク防止のため購読解除
            GameEvents.OnPhaseChanged -= HandlePhaseChanged;
        }

        /// <summary>
        /// フェーズが切り替わった瞬間に呼ばれる
        /// </summary>
        private void HandlePhaseChanged(GamePhase newPhase)
        {
            if (newPhase == GamePhase.Playing)
            {
                // 1. 物理演算を再開（重力計算が有効になる）
                rb.simulated = true;

                // 2. 確定したばかりのAbilityManagerの内容を即座に反映
                if (playerCheck != null)
                {
                    playerCheck.ForceCheck(); 
                }
            }
            else if (newPhase == GamePhase.Drawing)
            {
                // 選択フェーズに戻った場合は再度フリーズ
                rb.simulated = false;
            }
        }

        /// <summary>
        /// 司令塔への「エリアが変わったぞ」という報告窓口
        /// </summary>
        public void ChangeAreaEffect(int areaIndex)
        {
            // 【重要】Playing中以外はAbilityManagerの変更を反映させない
            if (GameManager.Instance == null || GameManager.Instance.CurrentPhase != GamePhase.Playing)
            {
                return;
            }

            AbilityType ability = AbilityManager.Instance.GetAbilityAt(areaIndex);
            int gravityDir = (ability == AbilityType.GravityInvert) ? 1 : 0;
            gravityLogic.Execute(rb, this.transform, gravityDir);
        }

        void FixedUpdate()
        {
            if (GameManager.Instance == null) return;

            // プレイ中かどうかに合わせて物理エンジンの稼働を切り替える
            // これにより、選択フェーズ中は重力も入力も受け付けず完全にフリーズします
            bool isPlaying = GameManager.Instance.CurrentPhase == GamePhase.Playing;
            rb.simulated = isPlaying;

            if (!isPlaying) return;

            playerMove.ExecutePhysicsUpdate();
        }

        public void SetMoveInput(Vector2 input)
        {
            playerMove.MoveInput = input;
            playerMove.UpdateAnimation();
        }

        #region GameLogic (Gravity/Effects)

        /// <summary>
        /// プレイヤーの全状態（座標、速度、重力、向き）を初期化します。
        /// </summary>
        public void ResetPlayerState()
        {
            if (playerMove != null)
            {
                playerMove.ResetPosition();
                playerMove.MoveInput = Vector2.zero;
                playerMove.UpdateAnimation();
            }

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                // リセット時の正位置（上向き）への回転を反映させるため一旦simulatedをONにする
                rb.simulated = true;
            }

            // 初期化時は常に「通常重力(0)」を適用
            gravityLogic.Execute(rb, this.transform, 0);

            // リセット直後はDrawingフェーズのはずなので再度フリーズさせる
            if (GameManager.Instance != null && GameManager.Instance.CurrentPhase != GamePhase.Playing)
            {
                rb.simulated = false;
            }

            Debug.Log("<color=green>【PlayerController】</color> プレイヤーの状態を正位置で初期化・フリーズしました。");
        }

        #endregion 
    }
}