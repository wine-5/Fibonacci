using UnityEngine;
using Fibonacci.InGame.BorderLine;
using Fibonacci.InGame.Player;
using Fibonacci.Audio;
using Fibonacci.Event;

namespace Fibonacci.InGame
{
    public class PlayerCheck : MonoBehaviour
    {
        [Header("参考コンポーネント")]
        [SerializeField] private DrawBorderLine drawBorderLine;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerInputManager playerInputManager;

        private int lastAreaIndex = -1;
        private bool isInitializedOnStart = false;

        private void Start()
        {
            // 初回のエリア判定
            ExecuteAreaCheck(false);
        }

        private void OnEnable()
        {
            GameEvents.OnRestart += OnGameRestart;
            GameEvents.OnAbilitiesUpdated += OnAbilitiesUpdated;
        }

        private void OnDisable()
        {
            GameEvents.OnRestart -= OnGameRestart;
            GameEvents.OnAbilitiesUpdated -= OnAbilitiesUpdated;
        }

        private void LateUpdate()
        {
            // プレイ中以外は判定を行わない（選択中の意図しない反転を防止）
            if (GameManager.Instance == null || GameManager.Instance.CurrentPhase != GamePhase.Playing) return;
            
            ExecuteAreaCheck(true);
        }

        /// <summary>
        /// PlayerControllerから呼ばれる強制再判定。
        /// プレイ開始の瞬間に呼び出すことで、ラグなしで効果を適用します。
        /// </summary>
        public void ForceCheck()
        {
            lastAreaIndex = -1;
            isInitializedOnStart = false;
            ExecuteAreaCheck(false); // 強制適用時はSEを鳴らさない場合はfalse
        }

        /// <summary>
        /// エリア判定と効果適用のコアロジック
        /// </summary>
        private void ExecuteAreaCheck(bool canPlaySound)
        {
            if (drawBorderLine == null || playerController == null) return;

            var borderData = drawBorderLine.GetBorderLineData();
            if (borderData == null || !borderData.IsActive)
            {
                isInitializedOnStart = false;
                return;
            }

            int currentAreaIndex = BorderLineCalculator.DetermineAreaIndex(
                borderData.P0,
                borderData.P1,
                transform.position
            );

            // 初回、またはエリアが変わった場合のみ適用
            if (!isInitializedOnStart || currentAreaIndex != lastAreaIndex)
            {
                // Controller側に通知（Controller内のガードによりPlaying中のみ反映される）
                playerController.ChangeAreaEffect(currentAreaIndex);

                // 音やその他の演出
                ApplyEffect(currentAreaIndex, canPlaySound && isInitializedOnStart);

                if (playerInputManager != null)
                {
                    playerInputManager.OnAreaChanged(currentAreaIndex);
                }

                lastAreaIndex = currentAreaIndex;
                isInitializedOnStart = true;
            }
        }

        private void OnGameRestart()
        {
            lastAreaIndex = -1;
            isInitializedOnStart = false;
        }

        private void OnAbilitiesUpdated()
        {
            // 能力更新時、プレイ中であれば即座に再評価する
            if (GameManager.Instance != null && GameManager.Instance.CurrentPhase == GamePhase.Playing)
            {
                ExecuteAreaCheck(false);
            }
        }

        private void ApplyEffect(int areaIndex, bool playSound)
        {
            if (playSound && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySE(SeType.Border);
            }
        }
    }
}