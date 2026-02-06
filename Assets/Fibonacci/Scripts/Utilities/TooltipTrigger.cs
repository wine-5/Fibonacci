using UnityEngine;
using UnityEngine.EventSystems;
using Fibonacci.InGame.BorderLine.UI;
using Fibonacci.InGame.Core;

namespace Fibonacci.Utilities
{
    /// <summary>
    /// UI要素へのマウスオーバーを検知し、ツールチップの表示・非表示を切り替えるトリガー。
    /// </summary>
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TooltipManager tooltipManager; // インスペクターからアタッチ

        private BorderLineEffectDefinition definition;

        /// <summary>
        /// 表示対象となる効果の定義データを設定する。
        /// </summary>
        /// <param name="def">ボーダーライン効果の定義</param>
        public void SetDefinition(BorderLineEffectDefinition def)
        {
            definition = def;
        }

        /// <summary>
        /// マウスカーソルがUI要素に入った際に呼び出され、ツールチップを表示する。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (tooltipManager == null)
            {
                tooltipManager =FindAnyObjectByType<TooltipManager>();
            }

            if (definition == null || tooltipManager == null) return;

            tooltipManager.Show(definition.Description);
        }

        /// <summary>
        /// マウスカーソルがUI要素から外れた際に呼び出され、ツールチップを非表示にする。
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (tooltipManager == null) return;
            tooltipManager.Hide();
        }
    }
}