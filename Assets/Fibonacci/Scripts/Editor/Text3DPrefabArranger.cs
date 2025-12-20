using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Fibonacci.Editor
{
    /// <summary>
    /// 3D文字Prefabを画面中央に配置するエディタツール
    /// </summary>
    public class Text3DPrefabArranger : EditorWindow
    {
        // 既存のヒエラルキーオブジェクトを配置するモード
        private GameObject existingTextObject;
        private float spacing = 1.0f;
        private bool centerVertically = true;
        
        // 回転設定
        private bool applyRotation = true;
        private Vector3 rotationAngles = new Vector3(90f, 180f, 0f);

        [MenuItem("Tools/3D Text Prefab Arranger")]
        public static void ShowWindow()
        {
            GetWindow<Text3DPrefabArranger>("3D Text Arranger");
        }

        private void OnGUI()
        {
            GUILayout.Label("3D文字配置ツール", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            DrawExistingObjectMode();
        }

        /// <summary>
        /// 既存オブジェクト配置モードのGUI
        /// </summary>
        private void DrawExistingObjectMode()
        {
            EditorGUILayout.HelpBox(
                "ヒエラルキー上にある親オブジェクト（例: TitleText）を指定してください。\n" +
                "子オブジェクトが自動的に画面中央に配置されます。",
                MessageType.Info
            );

            EditorGUILayout.Space();

            // 既存オブジェクト指定
            existingTextObject = (GameObject)EditorGUILayout.ObjectField(
                "親オブジェクト",
                existingTextObject,
                typeof(GameObject),
                true
            );

            // オブジェクト情報表示
            if (existingTextObject != null)
            {
                int childCount = existingTextObject.transform.childCount;
                EditorGUILayout.HelpBox($"子オブジェクト数: {childCount}", MessageType.Info);

                // 子オブジェクトのリスト表示
                if (childCount > 0)
                {
                    EditorGUILayout.LabelField("子オブジェクト一覧:", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;

                    for (int i = 0; i < Mathf.Min(childCount, 10); i++)
                    {
                        Transform child = existingTextObject.transform.GetChild(i);
                        EditorGUILayout.LabelField($"{i + 1}. {child.name}");
                    }

                    if (childCount > 10)
                    {
                        EditorGUILayout.LabelField($"... 他 {childCount - 10} 個");
                    }

                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.Space();

            // 配置設定
            spacing = EditorGUILayout.FloatField("文字間隔", spacing);
            centerVertically = EditorGUILayout.Toggle("垂直方向も中央配置", centerVertically);

            EditorGUILayout.Space();

            // 回転設定
            EditorGUILayout.LabelField("回転設定", EditorStyles.boldLabel);
            applyRotation = EditorGUILayout.Toggle("回転を適用", applyRotation);

            if (applyRotation)
            {
                EditorGUI.indentLevel++;
                rotationAngles = EditorGUILayout.Vector3Field("回転角度 (XYZ)", rotationAngles);
                EditorGUILayout.HelpBox(
                    $"現在の設定: X={rotationAngles.x}°, Y={rotationAngles.y}°, Z={rotationAngles.z}°",
                    MessageType.Info
                );
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // 配置ボタン
            GUI.enabled = existingTextObject != null && existingTextObject.transform.childCount > 0;

            if (GUILayout.Button("中央配置を実行", GUILayout.Height(40)))
            {
                ArrangeExistingObject();
            }

            GUI.enabled = true;
        }

        /// <summary>
        /// 既存のヒエラルキーオブジェクトを中央配置する
        /// </summary>
        private void ArrangeExistingObject()
        {
            if (existingTextObject == null)
            {
                EditorUtility.DisplayDialog("エラー", "親オブジェクトを設定してください", "OK");
                return;
            }

            Transform parentTrans = existingTextObject.transform;
            int childCount = parentTrans.childCount;

            if (childCount == 0)
            {
                EditorUtility.DisplayDialog("エラー", "子オブジェクトが存在しません", "OK");
                return;
            }

            // Undo登録
            Undo.RecordObject(parentTrans, "Arrange Existing Text");
            for (int i = 0; i < childCount; i++)
            {
                Undo.RecordObject(parentTrans.GetChild(i), "Arrange Existing Text");
            }

            // 子オブジェクトをリストに取得
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < childCount; i++)
            {
                children.Add(parentTrans.GetChild(i));
            }

            // 全体の幅を計算（中央配置用）
            float totalWidth = (childCount - 1) * spacing;
            float startX = -totalWidth / 2f;

            // 各子オブジェクトの位置を更新
            for (int i = 0; i < children.Count; i++)
            {
                Transform child = children[i];

                // 現在の位置を取得
                Vector3 currentPos = child.localPosition;

                // 新しい位置を計算
                Vector3 newPos = new Vector3(
                    startX + (i * spacing),
                    centerVertically ? 0 : currentPos.y,
                    currentPos.z
                );

                // ローカル位置を設定
                child.localPosition = newPos;

                // 回転を適用
                if (applyRotation)
                {
                    child.localRotation = Quaternion.Euler(rotationAngles);
                }

                Debug.Log($"配置: {child.name} -> X: {newPos.x}, Rotation: {(applyRotation ? rotationAngles.ToString() : "なし")}");
            }

            // 親オブジェクトの位置をリセット（必要に応じて）
            if (centerVertically)
            {
                Vector3 parentPos = parentTrans.position;
                parentPos.y = 0;
                parentTrans.position = parentPos;
            }

            // シーンビューにフォーカス
            Selection.activeGameObject = existingTextObject;
            SceneView.FrameLastActiveSceneView();

            // Unicode名をデコードして表示
            string decodedText = DecodeUnicodeNames(children);
            Debug.Log($"3D文字配置完了: {childCount}文字を中央配置しました（{decodedText}）");

            EditorUtility.DisplayDialog(
                "配置完了",
                $"{childCount}個のオブジェクトを中央配置しました\n\n文字: {decodedText}",
                "OK"
            );
        }

        /// <summary>
        /// Unicode形式の名前をデコードして文字列を取得
        /// </summary>
        private string DecodeUnicodeNames(List<Transform> children)
        {
            string result = "";

            foreach (Transform child in children)
            {
                string name = child.name;

                // "U"で始まる16進数形式の名前をデコード
                if (name.StartsWith("U") && name.Length >= 5)
                {
                    try
                    {
                        string hexCode = name.Substring(1, 4);
                        int unicodeValue = System.Convert.ToInt32(hexCode, 16);
                        char character = (char)unicodeValue;
                        result += character;
                    }
                    catch
                    {
                        result += "?";
                    }
                }
                else
                {
                    result += name.Length > 0 ? name[0].ToString() : "?";
                }
            }

            return result;
        }
    }
}