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
        }

        [SerializeField] private List<AbilityVisual> visuals;

        private readonly Dictionary<AbilityType, Sprite> spriteCache = new Dictionary<AbilityType, Sprite>();
        private bool isInitialized = false;

        /// <summary>
        /// リスト構造の視覚情報を Dictionary にキャッシュし、検索効率を最適化します。
        /// </summary>
        private void InitializeCache()
        {
            if (isInitialized) return;

            spriteCache.Clear();
            foreach (AbilityVisual visual in visuals)
            {
                if (!spriteCache.ContainsKey(visual.type))
                {
                    spriteCache.Add(visual.type, visual.icon);
                }
            }
            isInitialized = true;
        }

        /// <summary>
        /// 指定されたアビリティタイプに対応するアイコンスプライトを取得します。
        /// </summary>
        public Sprite GetSprite(AbilityType type)
        {
            InitializeCache();

            return spriteCache.TryGetValue(type, out Sprite sprite) ? sprite : null;
        }
    }
}