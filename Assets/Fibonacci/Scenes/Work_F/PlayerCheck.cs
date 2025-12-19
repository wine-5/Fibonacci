using UnityEngine;
using Fibonacci.InGame.BorderLine;

namespace Fibonacci.InGame.Player
{
    public class PlayerCheck : MonoBehaviour
    {
        [SerializeField] private DrawBorderLine drawBorderLine;

        void Update()
        {
            if (drawBorderLine == null) return;

            var colorMap = drawBorderLine.GetColorMap();
            if (colorMap == null) return;

            // プレイヤーの現在地の色(エリア)を取得
            int currentAreaIndex = colorMap.GetAreaIndex(transform.position);

            // デバッグログ
            if (currentAreaIndex == 1)
                Debug.Log("<color=green>現在：緑エリア</color>");
            else if (currentAreaIndex == 0)
                Debug.Log("<color=blue>現在：青エリア</color>");
            else
                Debug.Log("エリア外");
        }
    }
}