using System.Collections.Generic;
using Fibonacci.Event;
using UnityEngine;


/// <summary>
/// エリアに割り当てられるアビリティの種類を定義します。
/// AbilityManagerと同じ名前空間に置くことで、参照エラーを防止します。
/// </summary>
public enum AbilityType
{
    None,
    GravityInvert,
    LowGravity,
    MoveLock,
    Heavy,
    Fire,
}

namespace Fibonacci.InGame.Core
{
    /// <summary>
    /// 各エリアに割り振られたアビリティの状態を管理するクラス。
    /// プロジェクト共通の Singleton 基底クラスを継承し、エリアごとの能力設定と保持を担当します。
    /// </summary>
    public class AbilityManager : Singleton<AbilityManager>
    {
        protected override bool UseDontDestroyOnLoad => true;
        public const string ABILITY_ID_ZERO_GRAVITY = "ZeroGravity";
        public const string ABILITY_ID_GRAVITY = "Gravity";
        public const string ABILITY_ID_LOW_GRAVITY = "LowGravity";
        public const string ABILITY_ID_MOVE_LOCK = "MoveLock";
        public const string ABILITY_ID_HEAVY_SLOW = "Heavy";
        public const string ABILITY_ID_FIRE = "Fire";

        private readonly Dictionary<int, AbilityType> areaAbilities = new();

        [Header("Visual Data")]
        [SerializeField] private AbilitySpriteSO abilitySpriteData;

        private void Start()
        {
            RestoreAllGimmicks();
        }

        private void OnEnable()
        {
            GameEvents.OnRestart += RestoreAllGimmicks;
        }

        private void OnDisable()
        {
            GameEvents.OnRestart -= OnGameRestart;
        }

        private void OnGameRestart()
        {
            RestoreAllGimmicks();
        }

        public void SetGimmicksActive(int areaIndex, bool isActive)
        {
            // GimmickVisibilityController側で完結するため空実装
        }

        public void SetGimmicksHidden(int areaIndex, bool isHidden)
        {
            // GimmickVisibilityController側で完結するため空実装
        }

        private void RestoreAllGimmicks()
        {
            areaAbilities.Clear();
            GameEvents.TriggerAbilitiesUpdated();
        }

        /// <summary>
        /// アビリティに応じたスプライトを取得します。
        /// </summary>
        public Sprite GetAbilitySprite(AbilityType type)
        {
            if (abilitySpriteData == null) return null;
            return abilitySpriteData.GetSprite(type);
        }

        /// <summary>
        /// 文字列IDからアビリティを判定し、指定されたエリアに登録します。
        /// </summary>
        public void SetAreaAbility(int areaIndex, string abilityId)
        {
            string id = abilityId.Trim();
            areaAbilities[areaIndex] = ConvertIdToType(id);

            GameEvents.TriggerAbilitiesUpdated();
        }

        /// <summary>
        /// 指定されたエリアに現在割り当てられているアビリティを取得します。
        /// </summary>
        public AbilityType GetAbilityAt(int areaIndex)
        {
            return areaAbilities.TryGetValue(areaIndex, out var type) ? type : AbilityType.None;
        }

        /// <summary>
        /// 保持しているすべてのエリア能力情報をクリアします。
        /// </summary>
        public void Reset()
        {
            RestoreAllGimmicks();
        }

        /// <summary>
        /// 文字列IDを内部で使用する列挙型 AbilityType に変換します。
        /// </summary>
        private AbilityType ConvertIdToType(string id)
        {
            return id switch
            {
                ABILITY_ID_ZERO_GRAVITY => AbilityType.GravityInvert,
                ABILITY_ID_GRAVITY => AbilityType.GravityInvert,
                ABILITY_ID_MOVE_LOCK => AbilityType.MoveLock,
                ABILITY_ID_HEAVY_SLOW => AbilityType.Heavy,
                ABILITY_ID_LOW_GRAVITY => AbilityType.LowGravity,
                ABILITY_ID_FIRE => AbilityType.Fire,
                _ => AbilityType.None
            };
        }
    }
}