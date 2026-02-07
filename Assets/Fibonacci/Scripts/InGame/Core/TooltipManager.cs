using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Fibonacci.Event;

namespace Fibonacci.InGame.Core
{
    /// <summary>
    /// UI説明文（ツールチップ）の表示状態およびマウス追従座標を制御するマネージャー。
    /// </summary>
    public class TooltipManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private RectTransform tooltipRect;
        [SerializeField] private TextMeshProUGUI descriptionText;

        [Header("Input Settings")]
        [SerializeField] private InputActionReference pointActionReference;

        [Header("Settings")]
        [SerializeField] private Vector2 offset = new(15f, 15f);

        private GameObject tooltipGo;
        private InputAction pointAction;

        private void Awake()
        {
            if (tooltipRect == null) return;

            tooltipGo = tooltipRect.gameObject;

            if (pointActionReference != null)
            {
                pointAction = pointActionReference.action;
            }
        }

        private void OnEnable()
        {
            pointAction?.Enable();
            GameEvents.OnRestart += Hide;
        }

        private void OnDisable()
        {
            pointAction?.Disable();
            GameEvents.OnRestart -= Hide;
        }

        private void Start()
        {
            Hide();
        }

        private void Update()
        {
            if (tooltipGo == null || !tooltipGo.activeSelf) return;

            UpdatePosition();
        }

        /// <summary>
        /// ツールチップを指定された文字列で表示する。
        /// </summary>
        /// <param name="text">表示する説明文</param>
        public void Show(string text)
        {
            if (descriptionText == null || tooltipGo == null) return;

            descriptionText.text = text;
            tooltipGo.SetActive(true);
        }

        /// <summary>
        /// ツールチップを非表示にする。
        /// </summary>
        public void Hide()
        {
            if (tooltipGo == null) return;
            tooltipGo.SetActive(false);
        }

        private void UpdatePosition()
        {
            if (tooltipRect == null || pointAction == null) return;

            Vector2 mousePos = pointAction.ReadValue<Vector2>();
            tooltipRect.position = mousePos + offset;
        }
    }
}