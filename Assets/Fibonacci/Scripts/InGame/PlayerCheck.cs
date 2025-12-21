using UnityEngine;
using Fibonacci.InGame.BorderLine;
using Fibonacci.Player;
using Fibonacci.Audio;
using System.Linq;

namespace Fibonacci.InGame.Player
{
    public class PlayerCheck : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DrawBorderLine drawBorderLine;
        [SerializeField] private PlayerController playerController;

        [Header("Audio Settings")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioDataSO audioData;

        private int lastAreaIndex = -1;
        private bool isInitializedOnStart = false;

        private void Start()
        {
            // 開始時のインデックスを記録
            UpdateAreaIndex();
        }

        // ★ Update ではなく LateUpdate を使う
        // これにより、他の全てのスクリプトのUpdate（データの確定）が終わった後に判定が走ります
        void LateUpdate()
        {
            if (GameManager.Instance.CurrentPhase != GamePhase.Playing) return;

            if (drawBorderLine == null || playerController == null) return;
            var colorMap = drawBorderLine.GetColorMap();
            if (colorMap == null) return;

            int currentAreaIndex = colorMap.GetAreaIndex(transform.position);

            // 初回判定、またはエリアが変更された場合のみ実行
            if (!isInitializedOnStart || currentAreaIndex != lastAreaIndex)
            {
                ApplyEffect(currentAreaIndex, isInitializedOnStart);
                lastAreaIndex = currentAreaIndex;
                isInitializedOnStart = true;
            }
        }

        private void ApplyEffect(int areaIndex, bool playSound)
        {
            // エリア移動時のみ音を鳴らす（初回判定時は鳴らさない）
            if (playSound && areaIndex != lastAreaIndex && lastAreaIndex != -1)
            {
                PlaySoundByName("Border");
            }

            // 重力状態の反映
            if (areaIndex == 0 || areaIndex == 1)
            {
                playerController.OnAreaChanged(areaIndex);
            }
            else
            {
                playerController.ResetGravity();
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

        private void PlaySoundByName(string targetName)
        {
            if (audioData == null || audioSource == null) return;
            var data = audioData.AudioDataList.FirstOrDefault(x => x.AudioName == targetName);
            if (data != null && data.AudioClip != null)
            {
                audioSource.PlayOneShot(data.AudioClip, data.VolumeMultiplier);
            }
        }
    }
}