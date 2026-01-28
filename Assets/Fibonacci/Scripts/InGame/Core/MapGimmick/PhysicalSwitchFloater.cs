using UnityEngine;
using System.Collections.Generic;

namespace Fibonacci.InGame.Core.MapGimmick
{
    /// <summary>
    /// 物理的なスイッチ判定を行い、対象のオブジェクトを浮遊・沈降させるギミック。
    /// 指定されたレイヤーのオブジェクトが乗っている間、目標座標へスムーズに移動させます。
    /// </summary>
    public class PhysicalSwitchFloater : MonoBehaviour
    {
        [Header("浮かせたい対象")]
        [SerializeField] private Transform targetObject;

        [Header("浮遊設定")]
        [SerializeField] private Vector3 floatOffset = new(0, 2f, 0); 
        [SerializeField] private float smoothSpeed = 5f;

        [Header("反応するレイヤー")]
        [SerializeField] private LayerMask detectionLayer = ~0; 

        [Header("スイッチの設定")]
        [SerializeField] private bool debugMode = false;

        private Vector3 startPosition;
        private Vector3 goalPosition;
        
        private readonly HashSet<Collider2D> onSwitchObjects = new();

        /// <summary>
        /// 初期座標を保持し、オフセットを加えた目標座標を事前に計算します。
        /// </summary>
        private void Start()
        {
            startPosition = targetObject.position;
            goalPosition = startPosition + floatOffset;
        }

        /// <summary>
        /// スイッチの押下状態に応じて、対象オブジェクトを目標座標へ補間移動させます。
        /// </summary>
        private void Update()
        {
            bool isPressed = onSwitchObjects.Count > 0;
            Vector3 targetPos = isPressed ? goalPosition : startPosition;
            
            targetObject.position = Vector3.Lerp(targetObject.position, targetPos, Time.deltaTime * smoothSpeed);
        }

        /// <summary>
        /// 特定レイヤーのオブジェクトが進入した際、セットに登録してスイッチを起動します。
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & detectionLayer) == 0) return;

            onSwitchObjects.Add(other);

        }

        /// <summary>
        /// オブジェクトが離脱した際、セットから除外してスイッチ状態を更新します。
        /// </summary>
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!onSwitchObjects.Remove(other)) return;
        }
    }
}