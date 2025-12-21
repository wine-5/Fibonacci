using System.Collections.Generic;
using UnityEngine;

namespace Fibonacci.InGame.BorderLine.UI
{
    /// <summary>
    /// 効果の選択ロジック担当。
    /// - BorderLineEffectUI（表示）からクリックイベントを受け取る
    /// - 2領域とも選択したら決定して以後変更不可
    /// - 決定したらDrawBorderLineをロック（再分割不可）
    /// </summary>
    public sealed class BorderLineSelectedEffect : MonoBehaviour
    {
        [SerializeField] private DrawBorderLine drawBorderLine;
        [SerializeField] private BorderLineEffectUI effectUI;

        [Header("Decision")]
        [SerializeField] private bool lockOnDecide = true;

        private bool decided;
        private readonly HashSet<int> selectedFrameIndices = new();

        private void Awake()
        {
            if (drawBorderLine == null)
            {
                drawBorderLine = FindFirstObjectByType<DrawBorderLine>();
            }
            if (effectUI == null)
            {
                effectUI = FindFirstObjectByType<BorderLineEffectUI>();
            }
        }

        private void OnEnable()
        {
            if (effectUI != null)
            {
                effectUI.EffectClicked -= OnEffectClicked;
                effectUI.EffectClicked += OnEffectClicked;
            }

            if (drawBorderLine != null)
            {
                drawBorderLine.PartitionCreated -= OnPartitionCreated;
                drawBorderLine.PartitionCreated += OnPartitionCreated;
            }
        }

        private void OnDisable()
        {
            if (effectUI != null)
            {
                effectUI.EffectClicked -= OnEffectClicked;
            }
            if (drawBorderLine != null)
            {
                drawBorderLine.PartitionCreated -= OnPartitionCreated;
            }
        }

        private void OnPartitionCreated(BorderLineRegionSplitter.PartitionResult _, Camera __)
        {
            if (decided) return;
            selectedFrameIndices.Clear();
            effectUI?.ResetSelectionsAndShowPalettes();
        }

        private void OnEffectClicked(int frameIndex, int _, BorderLineEffectDefinition def)
        {
            if (decided) return;
            if (effectUI == null) return;
            if (def == null) return;

            // 1枠につき1回だけ選択（以後変更不可の仕様に寄せる）
            if (selectedFrameIndices.Contains(frameIndex)) return;

            selectedFrameIndices.Add(frameIndex);

            effectUI.ApplySelection(frameIndex, def.Id, def.Icon);
            effectUI.SetPaletteVisible(frameIndex, false);

            // 全枠（=現在の領域数）選択済みなら決定
            int need = effectUI.ActiveFrameCount;
            if (need <= 0) return;
            if (selectedFrameIndices.Count < need) return;

            decided = true;

            for (int i = 0; i < need; i++)
            {
                effectUI.SetPaletteVisible(i, false);
            }

            if (lockOnDecide && drawBorderLine != null)
            {
                drawBorderLine.LockInteraction();
            }
        }
    }
}
