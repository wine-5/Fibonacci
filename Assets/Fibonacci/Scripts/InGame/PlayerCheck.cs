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

            if (currentAreaIndex != lastAreaIndex)
            {
                if (currentAreaIndex == 0 || currentAreaIndex == 1)
                {
                    LogAreaChange(currentAreaIndex);
                    playerController.OnAreaChanged(currentAreaIndex);
                }
                else
                {
                    // ★追加点：エリア外（-1）になったらリセットを呼ぶ
                    playerController.ResetGravity();
                }

                lastAreaIndex = currentAreaIndex;
            }
        }

        private void LogAreaChange(int index)
        {
            if (index == 1)
            {
                
            }
            else if (index == 0)
            {
                
            }
        }
    }
}