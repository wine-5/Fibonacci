using UnityEngine;
using Fibonacci.InGame.BorderLine;
using Fibonacci.InGame.Player;
using Fibonacci.Audio;
using Fibonacci.Event;
using Fibonacci.InGame.Core;

namespace Fibonacci.InGame
{
    /// <summary>
    /// プレイヤーの現在位置を監視し、境界線によって分割されたエリア間の移動を検知します。
    /// エリア変更に伴う能力の切り替え、入力設定の更新、および音響演出の発火を制御します。
    /// </summary>
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
            if (GameManager.Instance == null || GameManager.Instance.CurrentPhase != GamePhase.Playing) return;

            ExecuteAreaCheck(true);
        }

        /// <summary>
        /// プレイヤーの状態（位置判定・効果適用）を強制的に再評価します。
        /// 主にゲーム開始時、位置関係をリセットした直後に最新のエリア効果を即時反映させるために使用します。
        /// </summary>
        public void ForceCheck()
        {
            lastAreaIndex = -1;
            isInitializedOnStart = false;
            ExecuteAreaCheck(false);
        }

        /// <summary>
        /// 現在のプレイヤー座標から所属エリアを計算し、前回判定時と異なるエリアにいる場合に効果を適用します。
        /// 境界線の有効状態のチェック、エリアインデックスの算出、各種マネージャーへの通知を一括で行います。
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

            if (!isInitializedOnStart || currentAreaIndex != lastAreaIndex)
            {
                playerController.ChangeAreaEffect(currentAreaIndex);

                ApplyEffect(canPlaySound && isInitializedOnStart);

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
            if (GameManager.Instance != null && GameManager.Instance.CurrentPhase == GamePhase.Playing)
            {
                ExecuteAreaCheck(false);
            }
        }

        private void ApplyEffect(bool playSound)
        {
            if (playSound && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySE(SeType.PowerUp);
            }
        }
    }
}