using UnityEngine;
using UnityEditor;

public class CharacterDataFixer : EditorWindow
{
    private CharacterData playerData;
    private CharacterData enemyData;
    private GameObject playerPrefab;
    private GameObject enemyPrefab;
    private CharacterFactory characterFactory;
    private Vector2 scrollPos;

    [MenuItem("Tools/Character Data Fixer")]
    public static void ShowWindow()
    {
        GetWindow<CharacterDataFixer>("Character Data Fixer");
    }

    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.Label("Character Data Setup", EditorStyles.boldLabel);

        // Поля для ScriptableObjects
        playerData = EditorGUILayout.ObjectField("Player Character Data", playerData, typeof(CharacterData), false) as CharacterData;
        enemyData = EditorGUILayout.ObjectField("Enemy Character Data", enemyData, typeof(CharacterData), false) as CharacterData;

        // Поля для префабов
        playerPrefab = EditorGUILayout.ObjectField("Player Prefab", playerPrefab, typeof(GameObject), false) as GameObject;
        enemyPrefab = EditorGUILayout.ObjectField("Enemy Prefab", enemyPrefab, typeof(GameObject), false) as GameObject;

        // Поле для CharacterFactory
        characterFactory = EditorGUILayout.ObjectField("Character Factory", characterFactory, typeof(CharacterFactory), true) as CharacterFactory;

        GUILayout.Space(10);

        // Кнопка Find All
        if (GUILayout.Button("Find All References"))
        {
            FindAllReferences();
        }

        // Кнопка Fix All
        if (GUILayout.Button("Fix All References"))
        {
            FixAllReferences();
        }

        // Отображение текущего состояния
        GUILayout.Space(10);
        GUILayout.Label("Current State:", EditorStyles.boldLabel);
        
        if (playerData != null)
        {
            EditorGUILayout.LabelField("Player Data References:");
            EditorGUI.indentLevel++;
            ShowCharacterDataInfo(playerData);
            EditorGUI.indentLevel--;
        }

        if (enemyData != null)
        {
            EditorGUILayout.LabelField("Enemy Data References:");
            EditorGUI.indentLevel++;
            ShowCharacterDataInfo(enemyData);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndScrollView();
    }

    private void ShowCharacterDataInfo(CharacterData data)
    {
        var so = new SerializedObject(data);
        SerializedProperty controllerProp = so.FindProperty("characterControllerPrefab");
        SerializedProperty prefabProp = so.FindProperty("characterPrefab");

        EditorGUILayout.LabelField("Controller: " + (controllerProp.objectReferenceValue != null ? "Set" : "Not Set"));
        EditorGUILayout.LabelField("Prefab: " + (prefabProp.objectReferenceValue != null ? "Set" : "Not Set"));
    }

    private void FindAllReferences()
    {
        // Поиск CharacterData assets
        string[] guids = AssetDatabase.FindAssets("t:CharacterData");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            CharacterData data = AssetDatabase.LoadAssetAtPath<CharacterData>(path);
            if (data != null)
            {
                if (path.Contains("Player"))
                    playerData = data;
                else if (path.Contains("Enemy"))
                    enemyData = data;
            }
        }

        // Поиск префабов
        guids = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                Character character = prefab.GetComponent<Character>();
                if (character != null)
                {
                    if (character.CharacterType == CharacterType.Player)
                        playerPrefab = prefab;
                    else if (character.CharacterType == CharacterType.DefaultEnemy)
                        enemyPrefab = prefab;
                }
            }
        }

        // Поиск CharacterFactory в сцене
        characterFactory = FindObjectOfType<CharacterFactory>();
    }

    private void FixAllReferences()
    {
        if (playerData != null && playerPrefab != null)
        {
            SetupCharacterData(playerData, playerPrefab);
        }

        if (enemyData != null && enemyPrefab != null)
        {
            SetupCharacterData(enemyData, enemyPrefab);
        }

        if (characterFactory != null)
        {
            SerializedObject serializedFactory = new SerializedObject(characterFactory);
            var playerPrefabProp = serializedFactory.FindProperty("playerCharacterPrefab");
            var enemyPrefabProp = serializedFactory.FindProperty("enemyCharacterPrefab");

            if (playerPrefab != null)
                playerPrefabProp.objectReferenceValue = playerPrefab.GetComponent<Character>();
            if (enemyPrefab != null)
                enemyPrefabProp.objectReferenceValue = enemyPrefab.GetComponent<Character>();

            serializedFactory.ApplyModifiedProperties();
        }

        // Применяем изменения и обновляем ассеты
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void SetupCharacterData(CharacterData data, GameObject prefab)
    {
        SerializedObject serializedData = new SerializedObject(data);
        
        var controllerProp = serializedData.FindProperty("characterControllerPrefab");
        var prefabProp = serializedData.FindProperty("characterPrefab");

        var controller = prefab.GetComponent<CharacterController>();
        if (controller != null)
            controllerProp.objectReferenceValue = controller;

        prefabProp.objectReferenceValue = prefab;

        serializedData.ApplyModifiedProperties();
        EditorUtility.SetDirty(data);
    }
}