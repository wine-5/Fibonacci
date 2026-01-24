using UnityEngine;
using System;
using System.Collections.Generic;

namespace Fibonacci.InGame.Core
{
    /// <summary>
    /// 各アビリティに対応するスプライトやビジュアルエフェクトの情報を保持するScriptableObjectクラス。
    /// ゲーム内でアビリティの視覚的表現を一元管理し、UI表示やエフェクト生成に利用されます。
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

        public List<AbilityVisual> visuals;

        public Sprite GetSprite(AbilityType type)
        {
            var item = visuals.Find(v => v.type == type);
            return item.icon;
        }
    }
}
