using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

namespace Fibonacci.Editor
{
    /// <summary>
    /// 不正なコンポーネントやMissingコンポーネントを検索するエディタツール
    /// </summary>
    public class FindMissingComponentsTool : EditorWindow
    {
        private class ObjectInfo
        {
            public string objectName;
            public string sceneName;
            public string scenePath;
            public string hierarchyPath;
        }
        
        private Vector2 scrollPosition;
        private Dictionary<GameObject, ObjectInfo> objectsWithIssues = new Dictionary<GameObject, ObjectInfo>();
        private bool scanComplete = false;

        [MenuItem("Tools/Find Missing Components")]
        public static void ShowWindow()
        {
            GetWindow<FindMissingComponentsTool>("Find Missing Components");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Missing/Invalid Components Finder", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (GUILayout.Button("Scan Current Scene", GUILayout.Height(30)))
                ScanCurrentScene();

            if (GUILayout.Button("Scan All Scenes", GUILayout.Height(30)))
                ScanAllScenes();

            if (GUILayout.Button("Scan All Prefabs", GUILayout.Height(30)))
                ScanAllPrefabs();

            EditorGUILayout.Space();

            if (scanComplete)
            {
                if (objectsWithIssues.Count == 0)
                {
                    EditorGUILayout.HelpBox("No issues found!", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox($"Found {objectsWithIssues.Count} objects with issues:", MessageType.Warning);
                    
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                    
                    foreach (var kvp in objectsWithIssues)
                    {
                        var obj = kvp.Key;
                        var info = kvp.Value;
                        if (obj == null) continue;

                        EditorGUILayout.BeginHorizontal();
                        
                        // オブジェクト名表示とクリックで選択
                        if (GUILayout.Button($"{info.objectName} ({info.sceneName})", EditorStyles.linkLabel))
                        {
                            // Prefabの場合
                            if (info.scenePath.EndsWith(".prefab"))
                            {
                                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(info.scenePath);
                                Selection.activeObject = prefab;
                                EditorGUIUtility.PingObject(prefab);
                            }
                            // シーンの場合
                            else if (!string.IsNullOrEmpty(info.scenePath))
                            {
                                EditorSceneManager.OpenScene(info.scenePath, OpenSceneMode.Single);
                                Selection.activeGameObject = obj;
                                EditorGUIUtility.PingObject(obj);
                            }
                        }
                        
                        // 階層パス表示
                        EditorGUILayout.LabelField(info.hierarchyPath, EditorStyles.miniLabel);
                        
                        EditorGUILayout.EndHorizontal();
                    }
                    
                    EditorGUILayout.EndScrollView();
                }
            }
        }

        private void ScanCurrentScene()
        {
            objectsWithIssues.Clear();
            scanComplete = false;

            var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var allObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            
            foreach (var obj in allObjects)
            {
                if (HasMissingComponents(obj))
                {
                    objectsWithIssues.Add(obj, new ObjectInfo
                    {
                        objectName = obj.name,
                        sceneName = currentScene.name,
                        scenePath = currentScene.path,
                        hierarchyPath = GetGameObjectPath(obj)
                    });
                }
            }

            scanComplete = true;
            Debug.Log($"Scan complete. Found {objectsWithIssues.Count} objects with missing components.");
        }

        private void ScanAllScenes()
        {
            objectsWithIssues.Clear();
            scanComplete = false;

            var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var scenePaths = new List<string>();

            // Build Settingsに登録されているシーンを取得
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                    scenePaths.Add(scene.path);
            }

            foreach (var scenePath in scenePaths)
            {
                if (string.IsNullOrEmpty(scenePath)) continue;
                
                Debug.Log($"[FindMissing] Opening scene: {scenePath}");
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                Debug.Log($"[FindMissing] Scene opened: {scene.name}");
                
                var allObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                
                foreach (var obj in allObjects)
                {
                    if (HasMissingComponents(obj))
                    {
                        objectsWithIssues.Add(obj, new ObjectInfo
                        {
                            objectName = obj.name,
                            sceneName = scene.name,
                            scenePath = scenePath,
                            hierarchyPath = GetGameObjectPath(obj)
                        });
                    }
                }
            }

            // 元のシーンに戻す（パスが有効な場合のみ）
            if (!string.IsNullOrEmpty(currentScene.path))
                EditorSceneManager.OpenScene(currentScene.path, OpenSceneMode.Single);

            scanComplete = true;
            Debug.Log($"All scenes scan complete. Found {objectsWithIssues.Count} objects with missing components.");
        }

        private void ScanAllPrefabs()
        {
            objectsWithIssues.Clear();
            scanComplete = false;

            // すべてのプレハブを検索
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
            
            Debug.Log($"[FindMissing] Scanning {prefabGuids.Length} prefabs...");

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (prefab == null) continue;

                // プレハブ自体とその子オブジェクトをすべてチェック
                var allObjects = new List<GameObject> { prefab };
                GetChildrenRecursive(prefab.transform, allObjects);

                foreach (var obj in allObjects)
                {
                    if (HasMissingComponents(obj))
                    {
                        objectsWithIssues.Add(obj, new ObjectInfo
                        {
                            objectName = obj.name,
                            sceneName = $"[Prefab] {path}",
                            scenePath = path,
                            hierarchyPath = GetGameObjectPath(obj)
                        });
                        
                        Debug.LogWarning($"[FindMissing] Found issue in Prefab: {path} -> {obj.name}");
                    }
                }
            }

            scanComplete = true;
            Debug.Log($"[FindMissing] Prefab scan complete. Found {objectsWithIssues.Count} objects with missing components.");
        }

        private void GetChildrenRecursive(Transform parent, List<GameObject> list)
        {
            foreach (Transform child in parent)
            {
                list.Add(child.gameObject);
                GetChildrenRecursive(child, list);
            }
        }

        private bool HasMissingComponents(GameObject obj)
        {
            var components = obj.GetComponents<Component>();
            bool hasMissing = false;
            
            // Unityの標準的なnullチェックでMissingコンポーネントを検出
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    Debug.LogWarning($"[FindMissing] Missing component at index {i} on '{GetGameObjectPath(obj)}'", obj);
                    hasMissing = true;
                }
            }
            
            return hasMissing;
        }

        private string GetGameObjectPath(GameObject obj)
        {
            string path = obj.name;
            Transform parent = obj.transform.parent;
            
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            
            return path;
        }
    }
}