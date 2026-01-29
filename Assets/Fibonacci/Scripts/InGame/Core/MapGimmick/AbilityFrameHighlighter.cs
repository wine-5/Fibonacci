using UnityEngine;
using Fibonacci.InGame.BorderLine;
using Fibonacci.Event;

namespace Fibonacci.InGame.Core.MapGimmick
{
    /// <summary>
    /// 各エリアのアビリティ状態に応じて、境界線のフレームカラーを動的に変更するクラス。
    /// アビリティの更新イベントを購読し、視覚的なフィードバックを制御します。
    /// </summary>
    public class AbilityFrameHighlighter : MonoBehaviour
    {
        [SerializeField] private DrawBorderLine drawBorderLine;

        [Header("Ability Colors")]
        [SerializeField] private Color fireColor = Color.red;
        [SerializeField] private Color moveLockColor = Color.blue;
        [SerializeField] private Color defaultColor = Color.gray;

        private AbilityType lastType0 = (AbilityType)(-1);
        private AbilityType lastType1 = (AbilityType)(-1);

        private void OnEnable()
        {
            GameEvents.OnAbilitiesUpdated += RefreshHighlighter;
        }

        private void OnDisable()
        {
            GameEvents.OnAbilitiesUpdated -= RefreshHighlighter;
        }

        private void Start()
        {
            RefreshHighlighter();
        }

        /// <summary>
        /// 現在のアビリティ状態を確認し、変更がある場合のみフレームの配色を更新します。
        /// </summary>
        public void RefreshHighlighter()
        {
            if (AbilityManager.Instance == null) return;

            AbilityType type0 = AbilityManager.Instance.GetAbilityAt(0);
            AbilityType type1 = AbilityManager.Instance.GetAbilityAt(1);

            if (type0 == lastType0 && type1 == lastType1) return;

            lastType0 = type0;
            lastType1 = type1;

            Color color0 = GetColorForType(type0);
            Color color1 = GetColorForType(type1);

            drawBorderLine.UpdateFrameColors(color0, color1);
        }

        /// <summary>
        /// アビリティの種類に対応する色を返します。
        /// </summary>
        private Color GetColorForType(AbilityType type)
        {
            return type switch
            {
                AbilityType.Fire => fireColor,
                AbilityType.MoveLock => moveLockColor,
                _ => defaultColor
            };
        }
    }
}