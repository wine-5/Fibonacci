using System.Collections.Generic;
using Fibonacci.Event;
using UnityEngine;
using Fibonacci.InGame.Core.AreaGimmick;

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

        private readonly Dictionary<int, AbilityType> areaAbilities = new();

        [Header("Gimmick Settings")]
        [SerializeField] private string targetTag = "SpecialGimmick";
        private readonly List<GameObject> gimmickObjects = new();

        [Header("Visual Data")]
        [SerializeField] private AbilitySpriteSO abilitySpriteData;

        private void Start()
        {
            RefreshGimmickCache();
        }

        public void RefreshGimmickCache()
        {
            gimmickObjects.Clear();
            GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
            gimmickObjects.AddRange(targets);
        }

        public void SetGimmicksActive(int areaIndex, bool isActive)
        {
            foreach (var parentObj in gimmickObjects)
            {

                var identifier = parentObj.GetComponent<AreaGimmickIdentifier>();

                if (identifier != null && identifier.areaIndex != areaIndex)
                {
                    if (isActive) SetChildrenActive(parentObj, false);
                    continue;
                }

                SetChildrenActive(parentObj, isActive);
            }
        }

        public void AllGimmicksOff()
        {
            foreach (var parentObj in gimmickObjects)
            {
                if (parentObj == null) continue;

                foreach (Transform child in parentObj.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        private void SetChildrenActive(GameObject parent, bool active)
        {
            foreach (Transform child in parent.transform)
            {
                child.gameObject.SetActive(active);
            }
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
            areaAbilities.Clear();
            AllGimmicksOff();
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
                _ => AbilityType.None
            };
        }
    }
}