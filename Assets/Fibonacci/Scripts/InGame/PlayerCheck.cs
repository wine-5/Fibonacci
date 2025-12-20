using UnityEngine;
using Fibonacci.InGame.BorderLine;
using Fibonacci.Player;

namespace Fibonacci.InGame.Player
{
    public class PlayerCheck : MonoBehaviour
    {
        [SerializeField] private DrawBorderLine drawBorderLine;
        [SerializeField] private PlayerController playerController;

        // 前回のエリア番号を保持（-1: 未確定, 0: 青, 1: 緑）
        private int lastAreaIndex = -1;

        void Update()
        {
            if (drawBorderLine == null || playerController == null) return;

            var colorMap = drawBorderLine.GetColorMap();
            if (colorMap == null) return;

            int currentAreaIndex = colorMap.GetAreaIndex(transform.position);

            if (currentAreaIndex == -1) return;

            if (currentAreaIndex != lastAreaIndex)
            {
                // 1. デバッグログ（エリア変更の通知）
                LogAreaChange(currentAreaIndex);

                // 2. ★ Controllerに「エリアが変わったよ」と伝え、重力判定を行わせる
                playerController.OnAreaChanged(currentAreaIndex);

                lastAreaIndex = currentAreaIndex;
            }
        }

        private void LogAreaChange(int index)
        {
            if (index == 1)
            {
                Debug.Log("<color=green>【エリア変更】緑エリアに入りました</color>");
            }
            else if (index == 0)
            {
                Debug.Log("<color=blue>【エリア変更】青エリアに入りました</color>");
            }
        }
    }
}