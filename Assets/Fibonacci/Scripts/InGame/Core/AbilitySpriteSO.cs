using UnityEngine;
using System;
using System.Collections.Generic;

namespace Fibonacci.InGame.Core
{
    /// <summary>
    /// 各アビリティに対応するスプライトやビジュアルエフェクトの情報を保持する ScriptableObject クラス。
    /// </summary>
    [CreateAssetMenu(fileName = "AbilitySpriteSO", menuName = "Fibonacci/AbilitySpriteSO")]
    public class AbilitySpriteSO : ScriptableObject
    {
        [Serializable]
        public struct AbilityVisual
        {
            public AbilityType type;
            public Sprite icon;
            public Color effectColor;
        }

        [SerializeField] private List<AbilityVisual> visuals;

        private readonly Dictionary<AbilityType, Sprite> spriteCache = new();

        /// <summary>
        /// オブジェクトがロードされた際にキャッシュを構築します。
        /// ScriptableObjectのライフサイクルにおいて、Awakeよりも安定して呼ばれる初期化タイミングです。
        /// </summary>
        private void OnEnable()
        {
            RebuildCache();
        }

        /// <summary>
        /// 指定されたアビリティタイプに対応するアイコンスプライトを取得します。
        /// </summary>
        public Sprite GetSprite(AbilityType type)
        {
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
        /// エディタ上で値が変更された際にキャッシュを更新し、最新の情報を反映させます。
        /// </summary>
        private void OnValidate()
        {
            // エディタ実行中でない場合でも、インスペクターの変更を即座に反映
            RebuildCache();
        }
    }
}