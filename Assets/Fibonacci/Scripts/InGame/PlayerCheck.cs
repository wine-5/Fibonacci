using UnityEngine;
using Fibonacci.InGame.BorderLine;

namespace Fibonacci.InGame.Player
{
    public class PlayerCheck : MonoBehaviour
    {
        [SerializeField] private DrawBorderLine drawBorderLine;

        // 前回のエリア番号を保持（-1: 未確定, 0: 青, 1: 緑）
        private int lastAreaIndex = -1;

        void Update()
        {
            if (drawBorderLine == null) return;

            var colorMap = drawBorderLine.GetColorMap();
            if (colorMap == null) return;

            // 現在のエリアを取得
            int currentAreaIndex = colorMap.GetAreaIndex(transform.position);

            // 判定不能（エリア外）の場合は無視
            if (currentAreaIndex == -1) return;

            // 初回判定時、または前回とエリアが変わった場合のみ実行
            if (currentAreaIndex != lastAreaIndex)
            {
                // ログを出力
                LogAreaChange(currentAreaIndex);

                // 現在の状態を保存
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