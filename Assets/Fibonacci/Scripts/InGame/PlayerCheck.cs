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
            UpdateAreaIndex();
        }

        private void OnEnable()
        {
            GameEvents.OnRestart -= OnGameRestart;
            GameEvents.OnRestart += OnGameRestart;
            GameEvents.OnAbilitiesUpdated -= OnAbilitiesUpdated;
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


    private void OnGameRestart()
    {
        // リスタート時は前回の領域情報をクリアして再評価を許可する
        lastAreaIndex = -1;
        isInitializedOnStart = false;
    }

    private void OnAbilitiesUpdated()
    {
        // 能力が設定された直後に現在領域の効果を再適用する
        Debug.Log("[PlayerCheck] OnAbilitiesUpdated: re-evaluating current area");
        if (drawBorderLine == null || playerController == null) return;

        var borderData = drawBorderLine.GetBorderLineData();
        if (borderData == null || !borderData.IsActive) return;

        int currentAreaIndex = BorderLineCalculator.DetermineAreaIndex(borderData.P0, borderData.P1, transform.position);
        Debug.Log($"[PlayerCheck] OnAbilitiesUpdated determined area={currentAreaIndex}");
        // force apply
        playerController.ChangeAreaEffect(currentAreaIndex);
        if (playerInputManager != null) playerInputManager.OnAreaChanged(currentAreaIndex);
        lastAreaIndex = currentAreaIndex;
        isInitializedOnStart = true;
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