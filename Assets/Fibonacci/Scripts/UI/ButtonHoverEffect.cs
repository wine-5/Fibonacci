using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Fibonacci.UI
{
    /// <summary>
    /// ボタンのホバー効果を管理するスクリプト
    /// マウスカーソルを当てたときにボタンを拡大し、離したときに元のサイズに戻します
    /// </summary>
    public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("拡大設定")]
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float animationDuration = 0.2f;
        [SerializeField] private Ease easeType = Ease.OutBack;
        
        [Header("オプション効果")]
        [SerializeField] private bool enableColorChange = false;
        [SerializeField] private Color hoverColor = Color.white;
        
        private Vector3 originalScale;
        private Color originalColor;
        private UnityEngine.UI.Image buttonImage;
        private Tween scaleTween;
        private Tween colorTween;

        void Start()
        {
            // 元のスケールを保存
            originalScale = transform.localScale;
            
            // ボタンのImage コンポーネントを取得（色変更用）
            buttonImage = GetComponent<UnityEngine.UI.Image>();
            if (buttonImage != null)
            {
                originalColor = buttonImage.color;
            }
        }

        /// <summary>
        /// マウスカーソルがボタンに入った時の処理
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            // 既存のアニメーションを停止
            scaleTween?.Kill();
            colorTween?.Kill();
            
            // 拡大アニメーション
            scaleTween = transform.DOScale(originalScale * hoverScale, animationDuration)
                .SetEase(easeType);
            
            // 色変更（有効な場合）
            if (enableColorChange && buttonImage != null)
            {
                colorTween = buttonImage.DOColor(hoverColor, animationDuration);
            }
        }

        /// <summary>
        /// マウスカーソルがボタンから出た時の処理
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            // 既存のアニメーションを停止
            scaleTween?.Kill();
            colorTween?.Kill();
            
            // 元のサイズに戻すアニメーション
            scaleTween = transform.DOScale(originalScale, animationDuration)
                .SetEase(Ease.OutQuad);
            
            // 元の色に戻す（有効な場合）
            if (enableColorChange && buttonImage != null)
            {
                colorTween = buttonImage.DOColor(originalColor, animationDuration);
            }
        }

        /// <summary>
        /// オブジェクトが破棄される時にアニメーションを停止
        /// </summary>
        void OnDestroy()
        {
            scaleTween?.Kill();
            colorTween?.Kill();
        }
    }
}