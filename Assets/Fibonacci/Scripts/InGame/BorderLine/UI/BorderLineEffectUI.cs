using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace Fibonacci.InGame.BorderLine.UI
{
    /// <summary>
    /// 分割後の各領域の重心位置に「空の四角枠」を表示するだけの表示クラス。
    /// まずは枠表示だけ実装し、効果選択UIは次のステップで追加する。
    /// </summary>
    public sealed class BorderLineEffectUI : MonoBehaviour
    {
        [Header("Source")]
        [SerializeField] private DrawBorderLine drawBorderLine;

        [Header("World Canvas (Frame)")]
        [SerializeField] private Canvas worldCanvas;
        [SerializeField] private Transform framesParent;
        [SerializeField] private Sprite frameSprite;
        [SerializeField] private Vector2 frameSize = new(120f, 120f);
        [SerializeField] private Color frameColor = Color.white;
        [SerializeField] private Image.Type frameImageType = Image.Type.Sliced;

        [Header("Effect Catalog")]
        [SerializeField] private BorderLineEffectCatalog effectCatalog;

        [Header("Palette Layout")]
        [SerializeField] private float paletteYOffset = 80f;
        [SerializeField] private Vector2 paletteIconSize = new(56f, 56f);
        [SerializeField] private float paletteSpacing = 8f;

        [Header("Debug")]
        [SerializeField] private bool showEffectIdText = true;
        [SerializeField] private Vector2 effectIdTextOffset = new(0f, -80f);
        [SerializeField] private int effectIdFontSize = 24;
        [SerializeField] private Color effectIdTextColor = Color.white;
        [SerializeField] private Font effectIdFont;

        public event Action<int, int, BorderLineEffectDefinition> EffectClicked;

        private readonly List<FrameView> frames = new();
        public int ActiveFrameCount { get; private set; }

        private sealed class FrameView
        {
            public GameObject root;
            public Image selectedIcon;
            public Text effectIdText;
            public GameObject paletteRoot;
            public string selectedEffectId;
            public int regionId;
        }

        private void Awake()
        {
            if (drawBorderLine == null)
            {
                drawBorderLine = FindFirstObjectByType<DrawBorderLine>();
            }

            // デバッグの数字(1/2)は使わない想定なので抑制
            if (drawBorderLine != null)
            {
                drawBorderLine.SetSuppressRegionMarkers(true);
            }

            if (worldCanvas == null)
            {
                worldCanvas = GetComponentInChildren<Canvas>(includeInactive: true);
            }
            if (framesParent == null)
            {
                framesParent = worldCanvas != null ? worldCanvas.transform : transform;
            }
        }

        private void OnDisable()
        {
            HideAll();
        }

        private void Update()
        {
            if (drawBorderLine == null)
            {
                HideAll();
                return;
            }

            if (!drawBorderLine.TryGetCurrentPartition(out var partition) || partition.Regions == null)
            {
                HideAll();
                return;
            }

            EnsureFrameCount(partition.Regions.Count);
            ActiveFrameCount = partition.Regions.Count;

            for (int i = 0; i < partition.Regions.Count; i++)
            {
                var region = partition.Regions[i];
                var view = frames[i];
                if (view?.root == null) continue;
                view.root.SetActive(true);

                Vector2 c = region.Centroid;
                view.root.transform.position = new Vector3(c.x, c.y, drawBorderLine.WorldZ);
                view.regionId = region.Id;
            }
        }

        private void EnsureFrameCount(int count)
        {
            while (frames.Count < count)
            {
                frames.Add(CreateFrameView(frames.Count + 1));
            }

            for (int i = 0; i < frames.Count; i++)
            {
                if (frames[i]?.root != null) frames[i].root.SetActive(i < count);
            }
        }

        private FrameView CreateFrameView(int index)
        {
            var go = new GameObject($"RegionFrame_{index}");
            go.transform.SetParent(framesParent != null ? framesParent : transform, worldPositionStays: true);

            var rt = go.AddComponent<RectTransform>();
            rt.sizeDelta = frameSize;

            var img = go.AddComponent<Image>();
            img.sprite = frameSprite;
            img.color = frameColor;
            img.type = frameImageType;
            img.raycastTarget = false;

            // 選択済みアイコン（枠の中）
            var selectedGo = new GameObject("SelectedIcon");
            selectedGo.transform.SetParent(go.transform, worldPositionStays: false);
            var selectedRt = selectedGo.AddComponent<RectTransform>();
            selectedRt.anchorMin = new Vector2(0.5f, 0.5f);
            selectedRt.anchorMax = new Vector2(0.5f, 0.5f);
            selectedRt.anchoredPosition = Vector2.zero;
            selectedRt.sizeDelta = paletteIconSize;

            var selectedImg = selectedGo.AddComponent<Image>();
            selectedImg.sprite = null;
            selectedImg.enabled = false;
            selectedImg.raycastTarget = false;

            Text effectText = null;
            if (showEffectIdText)
            {
                var textGo = new GameObject("EffectIdText");
                textGo.transform.SetParent(go.transform, worldPositionStays: false);
                var textRt = textGo.AddComponent<RectTransform>();
                textRt.anchorMin = new Vector2(0.5f, 0.5f);
                textRt.anchorMax = new Vector2(0.5f, 0.5f);
                textRt.anchoredPosition = effectIdTextOffset;
                textRt.sizeDelta = new Vector2(frameSize.x * 2f, 40f);

                effectText = textGo.AddComponent<Text>();
                effectText.text = string.Empty;
                effectText.font = GetEffectIdFontOrNull();
                effectText.fontSize = effectIdFontSize;
                effectText.color = effectIdTextColor;
                effectText.alignment = TextAnchor.MiddleCenter;
                effectText.raycastTarget = false;

                if (effectText.font == null)
                {
                    // フォントが取れない環境ではテキスト表示を諦める（例外回避）
                    effectText.enabled = false;
                }
            }

            // パレット（枠の上）
            var paletteGo = new GameObject("PaletteRoot");
            paletteGo.transform.SetParent(go.transform, worldPositionStays: false);
            var paletteRt = paletteGo.AddComponent<RectTransform>();
            paletteRt.anchorMin = new Vector2(0.5f, 0.5f);
            paletteRt.anchorMax = new Vector2(0.5f, 0.5f);
            paletteRt.pivot = new Vector2(0.5f, 0f);
            paletteRt.anchoredPosition = new Vector2(0f, frameSize.y * 0.5f + paletteYOffset);

            var h = paletteGo.AddComponent<HorizontalLayoutGroup>();
            h.childAlignment = TextAnchor.MiddleCenter;
            h.childControlHeight = false;
            h.childControlWidth = false;
            h.childForceExpandHeight = false;
            h.childForceExpandWidth = false;
            h.spacing = paletteSpacing;
            h.padding = new RectOffset(0, 0, 0, 0);

            var fitter = paletteGo.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            RebuildPalette(index - 1, paletteRt, selectedImg, paletteGo);

            return new FrameView
            {
                root = go,
                selectedIcon = selectedImg,
                effectIdText = effectText,
                paletteRoot = paletteGo,
                selectedEffectId = null,
                regionId = index,
            };

        }

        private void RebuildPalette(int frameIndex, RectTransform paletteRt, Image selectedImg, GameObject paletteRoot)
        {
            if (paletteRt == null || paletteRoot == null) return;

            // 既存ボタン削除
            for (int i = paletteRt.childCount - 1; i >= 0; i--)
            {
                Destroy(paletteRt.GetChild(i).gameObject);
            }

            if (effectCatalog == null || effectCatalog.Effects == null) return;

            for (int i = 0; i < effectCatalog.Effects.Count; i++)
            {
                var def = effectCatalog.Effects[i];
                if (def == null || def.Icon == null || string.IsNullOrEmpty(def.Id)) continue;

                var btnGo = new GameObject($"Effect_{def.Id}");
                btnGo.transform.SetParent(paletteRt, worldPositionStays: false);

                var btnRt = btnGo.AddComponent<RectTransform>();
                btnRt.sizeDelta = paletteIconSize;

                var btnImg = btnGo.AddComponent<Image>();
                btnImg.sprite = def.Icon;
                btnImg.type = Image.Type.Simple;
                btnImg.color = Color.white;

                var btn = btnGo.AddComponent<Button>();
                int capturedFrameIndex = frameIndex;
                string capturedId = def.Id;
                Sprite capturedIcon = def.Icon;
                btn.onClick.AddListener(() => OnClickEffect(capturedFrameIndex, capturedId, capturedIcon));
            }
        }

        private void OnClickEffect(int frameIndex, string effectId, Sprite icon)
        {
            if (frameIndex < 0 || frameIndex >= frames.Count) return;
            var view = frames[frameIndex];
            if (view == null) return;

            // 表示側は状態を決めない。選択側(BorderLineSelectedEffect)へ通知する。
            var def = FindDefinition(effectId);
            EffectClicked?.Invoke(frameIndex, view.regionId, def);
        }

        private BorderLineEffectDefinition FindDefinition(string effectId)
        {
            if (effectCatalog == null || effectCatalog.Effects == null) return null;
            for (int i = 0; i < effectCatalog.Effects.Count; i++)
            {
                var def = effectCatalog.Effects[i];
                if (def == null) continue;
                if (def.Id == effectId) return def;
            }
            // 何らかの理由で見つからない場合もnull許容
            return null;
        }

        public void ApplySelection(int frameIndex, string effectId, Sprite icon)
        {
            if (frameIndex < 0 || frameIndex >= frames.Count) return;
            var view = frames[frameIndex];
            if (view == null) return;

            view.selectedEffectId = effectId;
            if (view.selectedIcon != null)
            {
                view.selectedIcon.sprite = icon;
                view.selectedIcon.enabled = icon != null;
            }
            if (view.effectIdText != null)
            {
                view.effectIdText.text = effectId;
            }
        }

        public void SetPaletteVisible(int frameIndex, bool visible)
        {
            if (frameIndex < 0 || frameIndex >= frames.Count) return;
            var view = frames[frameIndex];
            if (view?.paletteRoot == null) return;
            view.paletteRoot.SetActive(visible);
        }

        public void ResetSelectionsAndShowPalettes()
        {
            for (int i = 0; i < frames.Count; i++)
            {
                var v = frames[i];
                if (v == null) continue;
                v.selectedEffectId = null;
                if (v.selectedIcon != null)
                {
                    v.selectedIcon.sprite = null;
                    v.selectedIcon.enabled = false;
                }
                if (v.effectIdText != null)
                {
                    v.effectIdText.text = string.Empty;
                }
                if (v.paletteRoot != null)
                {
                    v.paletteRoot.SetActive(true);
                }
            }
        }

        private Font GetEffectIdFontOrNull()
        {
            if (effectIdFont != null) return effectIdFont;
            try
            {
                // Unityの一部環境ではArial.ttfが無効。LegacyRuntime.ttfが推奨。
                return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
            catch
            {
                return null;
            }
        }

        private void HideAll()
        {
            ActiveFrameCount = 0;
            for (int i = 0; i < frames.Count; i++)
            {
                if (frames[i]?.root != null) frames[i].root.SetActive(false);
            }
        }
    }
}
