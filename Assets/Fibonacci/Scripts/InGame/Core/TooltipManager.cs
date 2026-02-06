using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

namespace Fibonacci.InGame.Core
{
    /// <summary>
    /// UI説明文（ツールチップ）の表示状態およびマウス追従座標を制御するマネージャー。
    /// </summary>
    public class TooltipManager : Singleton<TooltipManager>
    {
        /// <summary>
        /// 各シーンのCanvasに配置されたUIオブジェクトを参照するため、シーン間での保持は行わない。
        /// </summary>
        protected override bool UseDontDestroyOnLoad => true;

        [Header("UI References")]
        [SerializeField] private RectTransform tooltipRect;
        [SerializeField] private TextMeshProUGUI descriptionText;

        [Header("Settings")]
        [SerializeField] private Vector2 offset = new(15f, 15f);

        private GameObject tooltipGo;

        /// <summary>
        /// インスタンス生成時の初期化処理。
        /// 親のAwakeでインスタンス登録を行い、自身でコンポーネントのキャッシュを行う。
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (tooltipRect == null) return;

            tooltipGo = tooltipRect.gameObject;
        }

        /// <summary>
        /// 開始時の初期化処理。
        /// </summary>
        private void Start()
        {
            Hide();
        }

        /// <summary>
        /// ツールチップが表示中である場合のみ、座標の更新処理を行う。
        /// </summary>
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

        /// <summary>
        /// マウスの現在位置を読み取り、オフセットを加算して表示位置を更新する。
        /// </summary>
        private void UpdatePosition()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            tooltipRect.position = mousePos + offset;
        }
    }
}