using UnityEngine;
using Fibonacci.InGame.BorderLine;
using Fibonacci.InGame.Player;
using Fibonacci.Audio;
using System.Linq;

namespace Fibonacci.InGame
{
    /// <summary>
    /// プレイヤーの座標を監視し、所属するエリアに応じたイベント処理を行うクラス。
    /// 境界線（DrawBorderLine）のデータに基づき、エリアが切り替わった際の
    /// 重力変化の通知（PlayerController）やSEの再生（AudioSource）を統括します。
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
            var colorMap = drawBorderLine.GetColorMap();
            if (colorMap == null) return;

            int currentAreaIndex = colorMap.GetAreaIndex(transform.position);

            if (!isInitializedOnStart || currentAreaIndex != lastAreaIndex)
            {
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
                var colorMap = drawBorderLine.GetColorMap();
                if (colorMap != null)
                {
                    lastAreaIndex = colorMap.GetAreaIndex(transform.position);
                }
            }
        }
    }
}