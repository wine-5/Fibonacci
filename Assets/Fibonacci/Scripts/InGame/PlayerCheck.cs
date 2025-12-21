using UnityEngine;
using Fibonacci.InGame.BorderLine;
using Fibonacci.Player;
using Fibonacci.Audio; // ★これが必要！名前空間を追加
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

        void Update()
        {
            if (drawBorderLine == null || playerController == null) return;

            var colorMap = drawBorderLine.GetColorMap();
            if (colorMap == null) return;

            int currentAreaIndex = colorMap.GetAreaIndex(transform.position);

            if (currentAreaIndex != lastAreaIndex)
            {
                // ラインを越えた瞬間に音を鳴らす
                PlaySoundByName("Border");

                if (currentAreaIndex == 0 || currentAreaIndex == 1)
                {
                    playerController.OnAreaChanged(currentAreaIndex);
                }
                else
                {
                    playerController.ResetGravity();
                }

                lastAreaIndex = currentAreaIndex;
            }
        }

        private void PlaySoundByName(string targetName)
        {
            if (audioData == null || audioSource == null) return;

            // ★ AudioDataSO内のプロパティ名「AudioDataList」に修正
            // ★ AudioData内のプロパティ名「AudioName」と「AudioClip」に修正
            var data = audioData.AudioDataList.FirstOrDefault(x => x.AudioName == targetName);

            if (data != null && data.AudioClip != null)
            {
                // VolumeMultiplierも反映させるとより良くなります
                audioSource.PlayOneShot(data.AudioClip, data.VolumeMultiplier);
            }
            else
            {
                Debug.LogWarning($"AudioDataSOの中に '{targetName}' という名前の音が見つかりませんでした。");
            }
        }
    }
}