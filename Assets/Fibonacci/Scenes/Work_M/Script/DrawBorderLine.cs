using UnityEngine;
using UnityEngine.InputSystem;

namespace BorderLine
{
    /// <summary>
    /// 入力から2つのターゲット(球)を選択し、
    /// 線の描画と領域分割の処理を各責務クラスへ委譲するコーディネータ。
    /// （描画: BorderLineSegmentRenderer / 領域計算: BorderLineRegionSplitter / デバッグ表示: BorderLineRegionDebugView）
    /// </summary>
    public class DrawBorderLine : MonoBehaviour
    {
        [Header("Input / Camera")]
        [SerializeField] private Camera targetCamera;
        [SerializeField] private float worldZ = 0f;

        [Header("Targets")]
        [SerializeField] private string targetTag = "LineTarget";

        [Header("Line")]
        [SerializeField] private bool extendLineToBounds = true;

        [SerializeField] private LineRenderer lineRenderer;
        private Transform firstSelectedBall;
        private BorderLineSegmentRenderer lineDrawer;
        private BorderLineRegionDebugView regionDebugView;

        [Header("Region Debug")]
        [SerializeField] private bool showRegionMarkers = true;
        [SerializeField] private bool drawRegionOutlines = true;
        [SerializeField] private float debugDrawDuration = 2f;
        [SerializeField] private float markerTextSize = 0.25f;

        void Start()
        {
            if (lineRenderer == null)
            {
                lineRenderer = GetComponent<LineRenderer>();
            }

            lineDrawer = new BorderLineSegmentRenderer(lineRenderer);
            lineDrawer.Hide();

            regionDebugView = new BorderLineRegionDebugView(transform, worldZ, markerTextSize, debugDrawDuration);
        }

        private Camera GetCameraOrMain()
        {
            return targetCamera != null ? targetCamera : Camera.main;
        }

        private Vector2 GetMouseWorldPosition2D(Camera cam)
        {
            // ScreenToWorldPoint の z は「カメラからの距離」なので、2Dの盤面(z=worldZ)までの距離を入れる
            Vector2 screenPos = Mouse.current.position.ReadValue();
            float zDistance = Mathf.Abs(cam.transform.position.z - worldZ);
            Vector3 world = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, zDistance));
            return new Vector2(world.x, world.y);
        }

        void Update()
        {
            // 左クリックされたら判定を行う
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                CheckSelection();
            }
        }

        void CheckSelection()
        {
            // マウス位置をワールド座標に変換
            var cam = GetCameraOrMain();
            if (cam == null)
            {
                Debug.LogWarning("[DrawBorderLine] Camera が見つかりません。MainCameraタグか targetCamera を設定してください。", this);
                return;
            }

            Vector2 mousePos = GetMouseWorldPosition2D(cam);

            // クリックした座標を必ず表示（ヒットしなくても出る）
            var screenPos = Mouse.current.position.ReadValue();
            Debug.Log($"[DrawBorderLine] Click cam={cam.name} ortho={cam.orthographic} camZ={cam.transform.position.z} worldZ={worldZ} screen={screenPos} world={mousePos}", this);

            // Sceneビューで見える十字マーカー（Gameビューには出ません。GizmosをONにしてください）
            const float markSize = 0.25f;
            Debug.DrawLine(mousePos + Vector2.left * markSize, mousePos + Vector2.right * markSize, Color.yellow, 1f);
            Debug.DrawLine(mousePos + Vector2.up * markSize, mousePos + Vector2.down * markSize, Color.yellow, 1f);

            // クリック地点に重なっているCollider2Dを取得（2D向け）
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePos);

            if (hitCollider == null)
            {
                Debug.Log("[DrawBorderLine] hitCollider: null（その座標にCollider2Dが無い）", this);
                return;
            }

            if (!hitCollider.CompareTag(targetTag))
            {
                Debug.Log($"[DrawBorderLine] hitCollider tag mismatch: {hitCollider.tag}（{targetTag} ではない）", this);
                return;
            }

            Transform clickedObject = hitCollider.transform;

            if (firstSelectedBall == null)
            {
                // 1つ目の玉を選択
                firstSelectedBall = clickedObject;
                Debug.Log($"1つ目の玉を選択: {firstSelectedBall.name}", this);
            }
            else
            {
                // 2つ目の玉を選択（自分自身以外）
                if (clickedObject != firstSelectedBall)
                {
                    Debug.Log($"2つ目の玉を選択: {clickedObject.name}", this);

                    // 領域分割（矩形外枠ベース）
                    if (!BorderLineRegionSplitter.TryGetPlayAreaRectFromTargets(targetTag, out var rect))
                    {
                        Debug.LogWarning("[DrawBorderLine] プレイ領域矩形を作れません。targetTagの対象が存在するか確認してください。", this);
                        firstSelectedBall = null;
                        return;
                    }

                    if (!BorderLineRegionSplitter.TrySplitRectByLine(rect, firstSelectedBall.position, clickedObject.position, out var split))
                    {
                        Debug.LogWarning("[DrawBorderLine] 分割線が矩形外枠と交差しません（ほぼ接線/同一直線等の可能性）。", this);
                        firstSelectedBall = null;
                        return;
                    }

                    // 見た目の線（領域と一致させるなら外枠まで延長）
                    if (extendLineToBounds)
                    {
                        lineDrawer.Draw(new Vector3(split.Intersection0.x, split.Intersection0.y, 0f), new Vector3(split.Intersection1.x, split.Intersection1.y, 0f));
                    }
                    else
                    {
                        lineDrawer.Draw(firstSelectedBall.position, clickedObject.position);
                    }

                    // デバッグ表示（目印/アウトライン）
                    if (showRegionMarkers)
                    {
                        regionDebugView.EnsureMarkers();
                        regionDebugView.SetMarkerPositions(split.Centroid1, split.Centroid2);
                    }
                    if (drawRegionOutlines)
                    {
                        regionDebugView.DrawPolygons(split);
                    }

                    // 選択状態をリセット（必要に応じて変更してください）
                    firstSelectedBall = null;
                }
            }
        }
    }
}
