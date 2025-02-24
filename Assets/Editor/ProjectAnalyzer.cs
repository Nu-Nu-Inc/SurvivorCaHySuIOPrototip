using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;

public class ProjectAnalyzer : EditorWindow
{
    private Vector2 scrollPosition;
    private StringBuilder report = new StringBuilder();
    private bool showPrefabs = true;
    private bool showScriptableObjects = true;
    private bool showSceneObjects = true;

    [MenuItem("Tools/Project Analyzer")]
    public static void ShowWindow()
    {
        GetWindow<ProjectAnalyzer>("Project Analyzer");
    }

    void OnGUI()
    {
        GUILayout.Label("Project Structure Analyzer", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        showPrefabs = EditorGUILayout.ToggleLeft("Show Prefabs", showPrefabs, GUILayout.Width(100));
        showScriptableObjects = EditorGUILayout.ToggleLeft("Show ScriptableObjects", showScriptableObjects, GUILayout.Width(150));
        showSceneObjects = EditorGUILayout.ToggleLeft("Show Scene Objects", showSceneObjects, GUILayout.Width(150));
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Analyze Project"))
        {
            AnalyzeProject();
        }

        if (GUILayout.Button("Fix Project Structure"))
        {
            FixProjectStructure();
        }

        EditorGUILayout.Space();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        EditorGUILayout.TextArea(report.ToString());
        EditorGUILayout.EndScrollView();
    }

    void AnalyzeProject()
    {
        report.Clear();
        report.AppendLine("=== Project Analysis Report ===\n");

        if (showPrefabs)
        {
            AnalyzePrefabs();
        }

        if (showScriptableObjects)
        {
            AnalyzeScriptableObjects();
        }

        if (showSceneObjects)
        {
            AnalyzeSceneObjects();
        }
    }

    void AnalyzePrefabs()
    {
        report.AppendLine("=== PREFABS ===");
        
        // Поиск всех префабов с компонентом Character
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null)
            {
                Character character = prefab.GetComponent<Character>();
                if (character != null)
                {
                    report.AppendLine($"\nFound Character Prefab: {prefab.name}");
                    report.AppendLine($"Path: {path}");
                    AnalyzeGameObject(prefab, 1);

                    CharacterData data = character.CharacterData;
                    report.AppendLine($"  Character Data: {(data != null ? data.name : "NOT SET")}");
                }
            }
        }
    }

    void AnalyzeScriptableObjects()
    {
        report.AppendLine("\n=== SCRIPTABLE OBJECTS ===");

        string[] guids = AssetDatabase.FindAssets("t:CharacterData");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            CharacterData data = AssetDatabase.LoadAssetAtPath<CharacterData>(path);
            if (data != null)
            {
                report.AppendLine($"\nCharacterData: {data.name}");
                report.AppendLine($"  Path: {path}");
                report.AppendLine($"  Speed: {data.Speed}");
                report.AppendLine($"  Score Cost: {data.ScoreCost}");
                report.AppendLine($"  Time Between Attacks: {data.TimeBetweenAttacks}");
                report.AppendLine($"  Character Controller Prefab: {(data.CharacterController != null ? data.CharacterController.name : "NOT SET")}");
                report.AppendLine($"  Character Prefab: {(data.CharacterTransform != null ? data.CharacterTransform.name : "NOT SET")}");
            }
        }
    }

    void AnalyzeSceneObjects()
    {
        report.AppendLine("\n=== SCENE OBJECTS ===");

        GameObject gameManager = GameObject.Find("GameManager1");
        if (gameManager != null)
        {
            report.AppendLine("\nGameManager Analysis:");
            AnalyzeGameObject(gameManager, 1);

            GameManager manager = gameManager.GetComponent<GameManager>();
            if (manager != null)
            {
                // Проверяем ссылки в GameManager
                report.AppendLine("  GameManager References:");
                report.AppendLine($"    CharacterFactory: {(manager.CharacterFactory != null ? "Set" : "NOT SET")}");
            }
        }

        CharacterFactory[] factories = GameObject.FindObjectsOfType<CharacterFactory>();
        foreach (CharacterFactory factory in factories)
        {
            report.AppendLine("\nCharacterFactory Analysis:");
            AnalyzeGameObject(factory.gameObject, 1);
            report.AppendLine($"  Active Characters Count: {(factory.ActiveCharacters != null ? factory.ActiveCharacters.Count : 0)}");
        }
    }

    void AnalyzeGameObject(GameObject obj, int indent)
    {
        string indentation = new string(' ', indent * 2);
        
        report.AppendLine($"{indentation}GameObject: {obj.name}");
        
        Component[] components = obj.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component != null)
            {
                report.AppendLine($"{indentation}  Component: {component.GetType().Name}");
            }
        }

        foreach (Transform child in obj.transform)
        {
            AnalyzeGameObject(child.gameObject, indent + 1);
        }
    }

    void FixProjectStructure()
    {
        // Создаем необходимые папки
        CreateDirectoryIfNotExists("Assets/Prefabs");
        CreateDirectoryIfNotExists("Assets/Data");

        // Перемещаем префабы в правильную папку
        MoveCharacterPrefabs();

        // Исправляем ScriptableObjects
        FixScriptableObjects();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    void CreateDirectoryIfNotExists(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parentFolder = System.IO.Path.GetDirectoryName(path).Replace('\\', '/');
            string newFolderName = System.IO.Path.GetFileName(path);
            AssetDatabase.CreateFolder(parentFolder, newFolderName);
            Debug.Log($"Created directory: {path}");
        }
    }

    void MoveCharacterPrefabs()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null && prefab.GetComponent<Character>() != null)
            {
                string fileName = System.IO.Path.GetFileName(path);
                string newPath = $"Assets/Prefabs/{fileName}";
                
                if (path != newPath)
                {
                    AssetDatabase.MoveAsset(path, newPath);
                    Debug.Log($"Moved {fileName} to Prefabs folder");
                }
            }
        }
    }

    void FixScriptableObjects()
    {
        string[] guids = AssetDatabase.FindAssets("t:CharacterData");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            CharacterData data = AssetDatabase.LoadAssetAtPath<CharacterData>(path);
            
            if (data != null)
            {
                SerializedObject serializedData = new SerializedObject(data);
                bool modified = false;

                // Проверяем и исправляем путь к файлу
                if (!path.StartsWith("Assets/Data/"))
                {
                    string fileName = System.IO.Path.GetFileName(path);
                    string newPath = $"Assets/Data/{fileName}";
                    AssetDatabase.MoveAsset(path, newPath);
                    modified = true;
                }

                if (modified)
                {
                    serializedData.ApplyModifiedProperties();
                    EditorUtility.SetDirty(data);
                }
            }
        }
    }

    [MenuItem("Tools/Find Missing References")]
    static void FindMissingReferences()
    {
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            Component[] components = go.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (component == null)
                {
                    Debug.LogError($"Missing component on GameObject: {go.name}");
                    continue;
                }

                SerializedObject serializedComponent = new SerializedObject(component);
                SerializedProperty property = serializedComponent.GetIterator();

                while (property.Next(true))
                {
                    if (property.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if (property.objectReferenceValue == null && property.objectReferenceInstanceIDValue != 0)
                        {
                            Debug.LogError($"Missing reference in {go.name} component {component.GetType().Name} for property {property.name}");
                        }
                    }
                }
            }
        }
    }
}