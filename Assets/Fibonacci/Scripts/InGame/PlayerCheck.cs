using UnityEngine;
using Fibonacci.InGame.BorderLine;
using Fibonacci.InGame.Player;
using Fibonacci.Audio;

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
            UpdateAreaIndex();
        }

        private void LateUpdate()
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentPhase != GamePhase.Playing) return;
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
                string areaColor = currentAreaIndex == 1 ? "緑 (1)" : "青 (0)";
                
                playerController.ChangeAreaEffect(currentAreaIndex);

                ApplyEffect(currentAreaIndex, isInitializedOnStart);

                if (playerInputManager != null)
                {
                    playerInputManager.OnAreaChanged(currentAreaIndex);
                }

                lastAreaIndex = currentAreaIndex;
                isInitializedOnStart = true;
            }
        }

        private void ApplyEffect(int areaIndex, bool playSound)
        {
            if (playSound && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySE(SeType.Border);
            }
        }

        private void UpdateAreaIndex()
        {
            if (drawBorderLine == null) return;
            var borderData = drawBorderLine.GetBorderLineData();
            if (borderData != null && borderData.IsActive)
            {
                lastAreaIndex = BorderLineCalculator.DetermineAreaIndex(
                    borderData.P0, borderData.P1, transform.position
                );
            }
        }
    }
}