using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

namespace Practice.Effect
{
    /// <summary>
    /// マウストレイルエフェクトを制御するクラス
    /// マウスの移動に追従してMouseTrailEffectエフェクトを表示し、カメラの切り替えに応じて表示/非表示を制御
    /// </summary>
    public class MouseTrail : MonoBehaviour
    {
        private float speed;
        [SerializeField] private VisualEffect MouseTrailEffect;
        private Vector3 mousePos, effectPos;
        [SerializeField] private float mouseDepth = 10.0f;
    
        [Header("カメラ設定")]
        [SerializeField] private Camera targetCamera;
        private Vector3 previousMousePos;


        /// <summary>
        /// カメラの状態監視、マウス追従、エフェクト制御を行う
        /// </summary>
        void Update()
        {
            if (targetCamera == null || Mouse.current == null)
            {
                return;
            }

            Vector2 mousePos2D = Mouse.current.position.ReadValue();
            mousePos = new Vector3(mousePos2D.x, mousePos2D.y, 0f);
            effectPos = targetCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, mouseDepth));

            CalculateMouseSpeed();

            MouseTrailEffect.SetFloat("Speed", speed);
            previousMousePos = effectPos;
            transform.position = effectPos;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                SendClickEvent();
            }
        }

        /// <summary>
        /// マウスの移動速度を計算
        /// 前フレームとの位置差分から速度を算出
        /// </summary>
        void CalculateMouseSpeed()
        {
            speed = Vector3.Distance(effectPos, previousMousePos) / Time.deltaTime;
        }

        /// <summary>
        /// マウスクリック時のMouseTrailEffectイベントを送信
        /// MouseTrailEffectグラフの"Click"イベントをトリガー
        /// </summary>
        void SendClickEvent()
        {
            MouseTrailEffect.SendEvent("Click");
        }
    }
}
