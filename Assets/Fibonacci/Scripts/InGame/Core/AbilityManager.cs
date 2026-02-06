using System.Collections.Generic;
using Fibonacci.Event;
using UnityEngine;

/// <summary>
/// エリアに割り当てられるアビリティの種類を定義します。
/// </summary>
public enum AbilityType
{
    None,
    GravityInvert,
    LowGravity,
    MoveLock,
    Heavy,
    Fire,
    PowerUp,
    Jump,
}

namespace Fibonacci.InGame.Core
{
    /// <summary>
    /// 各エリアに割り振られたアビリティの状態を管理するシングルトンクラス。
    /// ステージ内のアビリティ配置情報を保持し、更新を通知します。
    /// </summary>
    public class AbilityManager : Singleton<AbilityManager>
    {
        // シーンを跨いで保持するが、シーンロード時に適切に初期化する必要がある
        protected override bool UseDontDestroyOnLoad => true;

        private const string ID_GRAVITY = "Gravity";
        private const string ID_LOW_GRAVITY = "LowGravity";
        private const string ID_MOVE_LOCK = "MoveLock";
        private const string ID_HEAVY = "Heavy";
        private const string ID_FIRE = "Fire";
        private const string ID_POWER_UP = "PowerUp";
        private const string ID_JUMP = "Jump";

        private readonly Dictionary<int, AbilityType> areaAbilities = new();

        [Header("Visual Data")]
        [SerializeField] private AbilitySpriteSO abilitySpriteData;

        private void OnEnable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            GameEvents.OnRestart += ClearAllAbilities;
        }

        private void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
            GameEvents.OnRestart -= ClearAllAbilities;
        }

        /// <summary>
        /// 引数の Scene を完全修飾名にして、名前空間の衝突を回避します
        /// </summary>
        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            ClearAllAbilities();
        }

        public Sprite GetAbilitySprite(AbilityType type)
        {
            return abilitySpriteData != null ? abilitySpriteData.GetSprite(type) : null;
        }

        public void SetAreaAbility(int areaIndex, string abilityId)
        {
            if (TooltipManager.HasInstance)
            {
                TooltipManager.Instance.Hide();
            }
            string id = abilityId.Trim();
            areaAbilities[areaIndex] = ConvertIdToType(id);
            GameEvents.TriggerAbilitiesUpdated();
        }

        public AbilityType GetAbilityAt(int areaIndex)
        {
            return areaAbilities.TryGetValue(areaIndex, out AbilityType type) ? type : AbilityType.None;
        }

        /// <summary>
        /// 外部（PlayerController等）から明示的にリセットを行うための公開メソッド。
        /// </summary>
        public void ResetAbilities()
        {
            ClearAllAbilities();
        }

        /// <summary>
        /// 全てのアビリティ情報を削除し、システム全体に通知します。
        /// </summary>
        private void ClearAllAbilities()
        {
            areaAbilities.Clear();
            GameEvents.TriggerAbilitiesUpdated();
        }

        private AbilityType ConvertIdToType(string id)
        {
            return id switch
            {
                ID_GRAVITY => AbilityType.GravityInvert,
                "ZeroGravity" => AbilityType.GravityInvert, 
                ID_MOVE_LOCK => AbilityType.MoveLock,
                ID_HEAVY => AbilityType.Heavy,
                ID_LOW_GRAVITY => AbilityType.LowGravity,
                ID_FIRE => AbilityType.Fire,
                ID_POWER_UP => AbilityType.PowerUp,
                ID_JUMP => AbilityType.Jump,
                _ => AbilityType.None
            };
        }
    }
}