using UnityEngine;
using System;
using System.Collections.Generic;

namespace Fibonacci.InGame.Core
{
    /// <summary>
    /// 各アビリティに対応するスプライトやビジュアルエフェクトの情報を保持する ScriptableObject クラス。
    /// ゲーム内でアビリティの視覚的表現を一元管理し、UI表示やエフェクト生成に利用されます。
    /// </summary>
    [CreateAssetMenu(fileName = "AbilitySpriteSO", menuName = "Fibonacci/AbilitySpriteSO")]
    public class AbilitySpriteSO : ScriptableObject
    {
        /// <summary>
        /// アビリティの種類に応じた視覚情報を定義する構造体。
        /// </summary>
        [Serializable]
        public struct AbilityVisual
        {
            public AbilityType type;
            public Sprite icon;
            public Color effectColor;
        }

        [SerializeField] private List<AbilityVisual> visuals;

        private readonly Dictionary<AbilityType, Sprite> spriteCache = new Dictionary<AbilityType, Sprite>();

        /// <summary>
        /// 指定されたアビリティタイプに対応するアイコンスプライトを取得します。
        /// </summary>
        public Sprite GetSprite(AbilityType type)
        {
            // キャッシュが空の場合のみ構築する（isInitializedフラグを廃止し、カウントで判定）
            if (spriteCache.Count == 0 && visuals.Count > 0)
            {
                RebuildCache();
            }

            return spriteCache.TryGetValue(type, out Sprite sprite) ? sprite : null;
        }

        /// <summary>
        /// リスト構造の視覚情報を Dictionary にキャッシュし、検索効率を最適化します。
        /// </summary>
        private void RebuildCache()
        {
            spriteCache.Clear();
            foreach (AbilityVisual visual in visuals)
            {
                if (!spriteCache.ContainsKey(visual.type))
                {
                    spriteCache.Add(visual.type, visual.icon);
                }
            }
        }

        /// <summary>
        /// エディタ上で値が変更された際にキャッシュをクリアし、次回の取得時に最新の情報が反映されるようにします。
        /// </summary>
        private void OnValidate()
        {
            spriteCache.Clear();
        }
    }
}