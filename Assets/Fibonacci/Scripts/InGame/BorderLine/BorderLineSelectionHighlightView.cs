using System.Collections;
using UnityEngine;

namespace Fibonacci.InGame.BorderLine
{
    /// <summary>
    /// 「選択中ターゲット」を視覚的に分かりやすくするためのハイライト表示専用コンポーネント。
    /// - 選択ロジックは持たない（SetTarget/Clear で状態を受け取るだけ）
    /// - 2D(SpriteRenderer) と 3D(Renderer + Unlit等) の両方に対応
    /// - 3Dは MaterialPropertyBlock を使い、マテリアル複製を避ける
    /// </summary>
    public sealed class BorderLineSelectionHighlightView : MonoBehaviour
    {
        [Header("Highlight")]
        [SerializeField] private bool enableHighlight = true;
        [SerializeField, Min(0.05f)] private float blinkPeriod = 0.35f;
        [SerializeField] private Color tint = new Color(1f, 1f, 0.2f, 1f);

        private Coroutine highlightCoroutine;

        private SpriteRenderer[] spriteRenderers;
        private Color[] spriteOriginalColors;

        private Renderer[] meshRenderers;
        private int[] meshColorPropIds;
        private Color[] meshOriginalColors;

        private MaterialPropertyBlock mpb;

        private void Awake()
        {
            mpb = new MaterialPropertyBlock();
        }

        private void OnDisable()
        {
            Clear();
        }

        public void SetTarget(Transform target)
        {
            if (!enableHighlight) return;
            if (target == null) return;

            Clear();

            // 2D: SpriteRenderer があればそれを優先
            spriteRenderers = target.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
            if (spriteRenderers != null && spriteRenderers.Length > 0)
            {
                spriteOriginalColors = new Color[spriteRenderers.Length];
                for (int i = 0; i < spriteRenderers.Length; i++)
                {
                    var sr = spriteRenderers[i];
                    spriteOriginalColors[i] = sr != null ? sr.color : Color.white;
                }

                highlightCoroutine = StartCoroutine(BlinkRoutine());
                return;
            }

            // 3D: Renderer を MaterialPropertyBlock で点滅
            var allRenderers = target.GetComponentsInChildren<Renderer>(includeInactive: true);
            if (allRenderers == null || allRenderers.Length == 0) return;

            var tmpRenderers = new System.Collections.Generic.List<Renderer>(allRenderers.Length);
            var tmpPropIds = new System.Collections.Generic.List<int>(allRenderers.Length);
            var tmpOriginals = new System.Collections.Generic.List<Color>(allRenderers.Length);

            for (int i = 0; i < allRenderers.Length; i++)
            {
                var r = allRenderers[i];
                if (r == null) continue;
                if (r is SpriteRenderer) continue;

                var mat = r.sharedMaterial;
                if (mat == null) continue;

                int propId = FindColorPropertyId(mat);
                if (propId == 0) continue;

                Color original = mat.GetColor(propId);
                tmpRenderers.Add(r);
                tmpPropIds.Add(propId);
                tmpOriginals.Add(original);
            }

            if (tmpRenderers.Count == 0) return;

            meshRenderers = tmpRenderers.ToArray();
            meshColorPropIds = tmpPropIds.ToArray();
            meshOriginalColors = tmpOriginals.ToArray();

            highlightCoroutine = StartCoroutine(BlinkRoutine());
        }

        public void Clear()
        {
            if (highlightCoroutine != null)
            {
                StopCoroutine(highlightCoroutine);
                highlightCoroutine = null;
            }

            if (spriteRenderers != null && spriteOriginalColors != null)
            {
                int n = Mathf.Min(spriteRenderers.Length, spriteOriginalColors.Length);
                for (int i = 0; i < n; i++)
                {
                    var sr = spriteRenderers[i];
                    if (sr != null) sr.color = spriteOriginalColors[i];
                }
            }

            if (meshRenderers != null && meshColorPropIds != null && meshOriginalColors != null)
            {
                mpb ??= new MaterialPropertyBlock();
                int n = Mathf.Min(meshRenderers.Length, Mathf.Min(meshColorPropIds.Length, meshOriginalColors.Length));
                for (int i = 0; i < n; i++)
                {
                    var r = meshRenderers[i];
                    if (r == null) continue;
                    int propId = meshColorPropIds[i];
                    if (propId == 0) continue;

                    r.GetPropertyBlock(mpb);
                    mpb.SetColor(propId, meshOriginalColors[i]);
                    r.SetPropertyBlock(mpb);
                }
            }

            spriteRenderers = null;
            spriteOriginalColors = null;

            meshRenderers = null;
            meshColorPropIds = null;
            meshOriginalColors = null;
        }

        private IEnumerator BlinkRoutine()
        {
            float t = 0f;
            while (true)
            {
                t += Time.unscaledDeltaTime;
                float phase = Mathf.PingPong(t, blinkPeriod) / blinkPeriod; // 0..1
                float k = Mathf.SmoothStep(0f, 1f, phase);

                if (spriteRenderers != null && spriteOriginalColors != null)
                {
                    int n = Mathf.Min(spriteRenderers.Length, spriteOriginalColors.Length);
                    for (int i = 0; i < n; i++)
                    {
                        var sr = spriteRenderers[i];
                        if (sr == null) continue;

                        Color baseCol = spriteOriginalColors[i];
                        Color target = new Color(tint.r, tint.g, tint.b, baseCol.a);
                        sr.color = Color.Lerp(baseCol, target, k);
                    }
                }
                else if (meshRenderers != null && meshColorPropIds != null && meshOriginalColors != null)
                {
                    mpb ??= new MaterialPropertyBlock();
                    int n = Mathf.Min(meshRenderers.Length, Mathf.Min(meshColorPropIds.Length, meshOriginalColors.Length));
                    for (int i = 0; i < n; i++)
                    {
                        var r = meshRenderers[i];
                        if (r == null) continue;
                        int propId = meshColorPropIds[i];
                        if (propId == 0) continue;

                        Color baseCol = meshOriginalColors[i];
                        Color target = new Color(tint.r, tint.g, tint.b, baseCol.a);
                        Color c = Color.Lerp(baseCol, target, k);

                        r.GetPropertyBlock(mpb);
                        mpb.SetColor(propId, c);
                        r.SetPropertyBlock(mpb);
                    }
                }
                else
                {
                    yield break;
                }

                yield return null;
            }
        }

        private static int FindColorPropertyId(Material mat)
        {
            if (mat.HasProperty("_BaseColor")) return Shader.PropertyToID("_BaseColor");
            if (mat.HasProperty("_Color")) return Shader.PropertyToID("_Color");
            if (mat.HasProperty("_TintColor")) return Shader.PropertyToID("_TintColor");
            return 0;
        }
    }
}
