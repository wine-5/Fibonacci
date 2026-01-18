using UnityEngine;
using System.Collections.Generic;

namespace Fibonacci.InGame.Core
{
    public enum AbilityType
    {
        None,
        GravityInvert,
    }


    /// <summary>
    /// 各エリアに割り振られたアビリティの状態を管理するピュアC#クラス。
    /// シングルトンパターンにより、どこからでもアクセス可能です。
    /// </summary>
    public class AbilityManager
    {
        private static AbilityManager _instance;
        public static AbilityManager Instance => _instance ??= new AbilityManager();

        private readonly Dictionary<int, AbilityType> _areaAbilities = new();

        private AbilityManager() { }

        /// <summary>
        /// 文字列IDからアビリティを登録します
        /// </summary>
        public void SetAreaAbility(int areaIndex, string abilityId)
        {
            // 文字列の余計な空白を消して確実に判定する
            string id = abilityId.Trim();

            if (id == "ZeroGravity")
            {
                _areaAbilities[areaIndex] = AbilityType.GravityInvert;
            }
            else
            {
                _areaAbilities[areaIndex] = AbilityType.None;
            }


            Fibonacci.Event.GameEvents.TriggerAbilitiesUpdated();
        }

        /// <summary>
        /// 指定エリアのアビリティを取得します
        /// </summary>
        public AbilityType GetAbilityAt(int areaIndex)
        {
            return _areaAbilities.TryGetValue(areaIndex, out var type) ? type : AbilityType.None;
        }

        public void Reset()
        {
            _areaAbilities.Clear();
            Debug.Log("<color=magenta>【AbilityManager】</color> リセットされました。");
        }

        private AbilityType ConvertIdToType(string id)
        {
            return id switch
            {
                "Gravity" => AbilityType.GravityInvert,
                _ => AbilityType.None
            };
        }
    }


}

