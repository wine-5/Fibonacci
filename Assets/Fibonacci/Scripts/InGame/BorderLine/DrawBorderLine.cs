using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;

namespace Fibonacci.InGame.BorderLine
{
    public class DrawBorderLine : MonoBehaviour
    {
        [Header("Input System (Actions)")]
        [SerializeField] private InputActionReference clickAction;
        [SerializeField] private InputActionReference pointerPositionAction;

        [Header("Input / Camera")]
        [SerializeField] private Camera targetCamera;
        [SerializeField] private float worldZ = 0f;

        [Header("Targets")]
        [SerializeField] private string targetTag = "LineTarget";

        [Header("Line")]
        [SerializeField] private bool extendLineToBounds = true;
        [SerializeField] private LineRenderer lineRenderer;

        // --- [ADD] Color Map 設定 ---
        [Header("Color Map")]
        [SerializeField, Label("色を表示する板")] private SpriteRenderer displayRenderer;
        [SerializeField, Label("解像度")] private int textureResolution = 1024;
        private BorderLineColorMap colorMap; 
        // ----------------------------

        private Transform firstSelectedBall;
        private BorderLineSegmentRenderer lineDrawer;
        private BorderLineRegionDebugView regionDebugView;

        [Header("Selection Highlight")]
        [SerializeField, Label("1つ目選択のハイライト表示")] private BorderLineSelectionHighlightView selectionHighlightView;

        [Header("Region Debug")]
        [SerializeField, Label("領域の数字デバッグ")] private bool showRegionMarkers = true;
        [SerializeField, Label("領域の囲いデバッグ")] private bool drawRegionOutlines = true;
        [SerializeField] private float debugDrawDuration = 2f;
        [SerializeField] private float markerTextSize = 0.25f;

        private void Awake()
        {
            if (lineRenderer == null)
            {
                lineRenderer = GetComponent<LineRenderer>();
            }

            lineDrawer = new BorderLineSegmentRenderer(lineRenderer);
            lineDrawer.Hide();

            regionDebugView = new BorderLineRegionDebugView(transform, worldZ, markerTextSize, debugDrawDuration);

            // --- [ADD] 初期化 ---
            colorMap = new BorderLineColorMap(displayRenderer, worldZ, textureResolution);

            if (selectionHighlightView == null)
            {
                selectionHighlightView = GetComponent<BorderLineSelectionHighlightView>();
            }
        }

        

        private void OnEnable()
        {
            if (clickAction != null && clickAction.action != null)
            {
                clickAction.action.performed += OnClickPerformed;
                clickAction.action.Enable();
            }

            if (pointerPositionAction != null && pointerPositionAction.action != null)
            {
                pointerPositionAction.action.Enable();
            }
        }

        private void OnDisable()
        {
            selectionHighlightView?.Clear();

            if (clickAction != null && clickAction.action != null)
            {
                clickAction.action.performed -= OnClickPerformed;
                clickAction.action.Disable();
            }

            if (pointerPositionAction != null && pointerPositionAction.action != null)
            {
                pointerPositionAction.action.Disable();
            }
        }

        private Camera GetCameraOrMain()
        {
            return targetCamera != null ? targetCamera : Camera.main;
        }

        private bool TryGetPointerScreenPosition(out Vector2 screenPos)
        {
            Vector2 pointerCurrentPos = default;
            bool hasPointerCurrent = Pointer.current != null;
            if (hasPointerCurrent)
            {
                pointerCurrentPos = Pointer.current.position.ReadValue();
            }

            bool hasAction = pointerPositionAction != null && pointerPositionAction.action != null;
            if (hasAction)
            {
                Vector2 actionPos = pointerPositionAction.action.ReadValue<Vector2>();

                if (hasPointerCurrent && actionPos == Vector2.zero && pointerCurrentPos != Vector2.zero)
                {
                    screenPos = pointerCurrentPos;
                    return true;
                }

                screenPos = actionPos;
                return true;
            }

            if (hasPointerCurrent)
            {
                screenPos = pointerCurrentPos;
                return true;
            }

            screenPos = default;
            return false;
        }

        private void OnClickPerformed(InputAction.CallbackContext _)
        {
            if (lineDrawer == null)
            {
                Awake();
            }
            if (!TryGetPointerScreenPosition(out var screenPos)) return;
            CheckSelection(screenPos);
        }

        private Vector2 ScreenToWorld2D(Camera cam, Vector2 screenPos)
        {
            float zDistance = Mathf.Abs(cam.transform.position.z - worldZ);
            Vector3 world = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, zDistance));
            return new Vector2(world.x, world.y);
        }

        void CheckSelection(Vector2 screenPos)
        {
            var cam = GetCameraOrMain();
            if (cam == null) return;

            Vector2 mousePos = ScreenToWorld2D(cam, screenPos);
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePos);

            if (hitCollider == null) return;
            if (!hitCollider.CompareTag(targetTag)) return;

            Transform clickedObject = hitCollider.transform;

            if (firstSelectedBall == null)
            {
                firstSelectedBall = clickedObject;
                selectionHighlightView?.SetTarget(firstSelectedBall);
            }
            else
            {
                if (clickedObject != firstSelectedBall)
                {
                    if (!BorderLineRegionSplitter.TryGetPlayAreaRectFromTargets(targetTag, out var rect))
                    {
                        selectionHighlightView?.Clear();
                        firstSelectedBall = null;
                        return;
                    }

                    if (!BorderLineRegionSplitter.TrySplitRectByLine(rect, firstSelectedBall.position, clickedObject.position, out var split))
                    {
                        selectionHighlightView?.Clear();
                        firstSelectedBall = null;
                        return;
                    }

                    lineDrawer.DrawSplitOrSegment(
                        firstSelectedBall.position,
                        clickedObject.position,
                        split.Intersection0,
                        split.Intersection1,
                        extendLineToBounds,
                        z: worldZ);

                    regionDebugView.Render(split, showRegionMarkers, drawRegionOutlines);

                    // --- [ADD] 色塗りの呼び出し ---
                    colorMap.UpdateVisual(split);

                    selectionHighlightView?.Clear();
                    firstSelectedBall = null;
                }
                
            }
        }
    }
}