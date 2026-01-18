using System.Collections.Generic;

namespace Fibonacci.InGame.Core
{
    /// <summary>
    /// ゲーム内の各エリアに割り当てられる特殊能力（アビリティ）の種類を定義します。
    /// </summary>
    public enum AbilityType
    {
        None,
        GravityInvert,
    }

    /// <summary>
    /// 各エリアに割り振られたアビリティの状態を一括管理するピュアC#クラス。
    /// シングルトンパターンにより保持され、エリアごとの能力設定、取得、およびリセット機能を提供します。
    /// </summary>
    public class AbilityManager
    {
        private static AbilityManager _instance;
        public static AbilityManager Instance => _instance ??= new AbilityManager();

        private readonly Dictionary<int, AbilityType> _areaAbilities = new();

        private AbilityManager() { }

        /// <summary>
        /// 文字列形式の識別子を受け取り、対応するアビリティを特定のエリアに登録します。
        /// 登録完了後、能力更新イベントを発火させます。
        /// </summary>
        public void SetAreaAbility(int areaIndex, string abilityId)
        {
            string id = abilityId.Trim();
            _areaAbilities[areaIndex] = ConvertIdToType(id);

            Fibonacci.Event.GameEvents.TriggerAbilitiesUpdated();
        }

        /// <summary>
        /// 指定されたエリアインデックスに現在割り当てられているアビリティを取得します。
        /// エリアが未登録の場合は AbilityType.None を返します。
        /// </summary>
        public AbilityType GetAbilityAt(int areaIndex)
        {
            return _areaAbilities.TryGetValue(areaIndex, out var type) ? type : AbilityType.None;
        }

        /// <summary>
        /// 全てのエリアに割り当てられた能力データを消去し、初期状態に戻します。
        /// </summary>
        public void Reset()
        {
            _areaAbilities.Clear();
        }

        /// <summary>
        /// 文字列IDを内部で使用する AbilityType 型に変換します。
        /// IDと列挙型の紐付けロジックをここに集約しています。
        /// </summary>
        private AbilityType ConvertIdToType(string id)
        {
            return id switch
            {
                "ZeroGravity" => AbilityType.GravityInvert,
                "Gravity" => AbilityType.GravityInvert,
                _ => AbilityType.None
            };
        }
    }
}