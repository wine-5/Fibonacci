using UnityEngine;
using Fibonacci.InGame.BorderLine;
using Fibonacci.InGame.Player;
using Fibonacci.Audio;
using System.Linq;

namespace Fibonacci.InGame
{
    /// <summary>
    /// プレイヤーの現在座標を監視し、BorderLineData を参照してエリアの切り替わりを検知するセンサー。
    /// エリア番号に変更があった場合のみ、PlayerInputManager などの上位コンポーネントへ
    /// イベントを通知する「観測」の役割に特化しています。
    /// </summary>
    public class PlayerCheck : MonoBehaviour
    {
        [Header("参考コンポーネント")]
        [SerializeField] private DrawBorderLine drawBorderLine;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerInputManager playerInputManager;

        [Header("オーディオ設定")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioDataSO audioData;

        private int lastAreaIndex = -1;
        private bool isInitializedOnStart = false;

        private void Start()
        {
            UpdateAreaIndex();
        }

        private void LateUpdate()
        {
            if (GameManager.Instance.CurrentPhase != GamePhase.Playing) return;

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
            {
                string areaColor = currentAreaIndex == 1 ? "緑 (1)" : "青 (0)";

                if (!isInitializedOnStart)
                {
                    Debug.Log($"<color=white>【初期判定】</color> 境界線が有効になりました。現在 <color=yellow>{areaColor}</color> にいます。");
                }
                else
                {
                    Debug.Log($"<color=cyan>【エリア変更】</color> 境界線を越えました！ <color=yellow>{areaColor}</color> に入ります。");
                }

                if (playerInputManager != null)
                {
                    playerInputManager.OnAreaChanged(currentAreaIndex);
                }

                lastAreaIndex = currentAreaIndex;
                isInitializedOnStart = true;
            }
        }



        private void UpdateAreaIndex()
        {
            if (drawBorderLine != null)
            {
                var borderData = drawBorderLine.GetBorderLineData();

                if (borderData != null && borderData.IsActive)
                {
                    lastAreaIndex = BorderLineCalculator.DetermineAreaIndex(
                        borderData.P0,
                        borderData.P1,
                        transform.position
                    );
                }
                else
                {
                    lastAreaIndex = -1;
                }
            }
        }
    }
}