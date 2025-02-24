using UnityEngine;
using UnityEditor;

public class CharacterSetupHelper : EditorWindow
{
    private Object playerPrefab;
    private Object enemyPrefab;
    private CharacterData playerData;
    private CharacterData enemyData;

    [MenuItem("Tools/Character Setup Helper")]
    public static void ShowWindow()
    {
        GetWindow<CharacterSetupHelper>("Character Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("Character Setup Helper", EditorStyles.boldLabel);

        playerPrefab = EditorGUILayout.ObjectField("Player Prefab", playerPrefab, typeof(GameObject), false);
        enemyPrefab = EditorGUILayout.ObjectField("Enemy Prefab", enemyPrefab, typeof(GameObject), false);

        GUILayout.Space(10);

        if (GUILayout.Button("Create Character Data"))
        {
            CreateCharacterData();
        }

        GUILayout.Space(10);

        playerData = EditorGUILayout.ObjectField("Player Data", playerData, typeof(CharacterData), false) as CharacterData;
        enemyData = EditorGUILayout.ObjectField("Enemy Data", enemyData, typeof(CharacterData), false) as CharacterData;

        if (GUILayout.Button("Setup Prefabs"))
        {
            SetupPrefabs();
        }
    }

    private void CreateCharacterData()
    {
        // Создаем папку Data если её нет
        if (!AssetDatabase.IsValidFolder("Assets/Data"))
        {
            AssetDatabase.CreateFolder("Assets", "Data");
        }

        // Создаем данные игрока
        if (playerData == null)
        {
            playerData = ScriptableObject.CreateInstance<CharacterData>();
            AssetDatabase.CreateAsset(playerData, "Assets/Data/PlayerCharacterData.asset");
            
            SerializedObject serializedPlayerData = new SerializedObject(playerData);
            serializedPlayerData.FindProperty("scoreCost").intValue = 0;
            serializedPlayerData.FindProperty("speed").floatValue = 5f;
            serializedPlayerData.FindProperty("timeBetweenAttacks").floatValue = 0.5f;
            if (playerPrefab != null)
            {
                GameObject player = playerPrefab as GameObject;
                serializedPlayerData.FindProperty("characterControllerPrefab").objectReferenceValue = player.GetComponent<CharacterController>();
                serializedPlayerData.FindProperty("characterPrefab").objectReferenceValue = player;
            }
            serializedPlayerData.ApplyModifiedProperties();
        }

        // Создаем данные врага
        if (enemyData == null)
        {
            enemyData = ScriptableObject.CreateInstance<CharacterData>();
            AssetDatabase.CreateAsset(enemyData, "Assets/Data/EnemyCharacterData.asset");
            
            SerializedObject serializedEnemyData = new SerializedObject(enemyData);
            serializedEnemyData.FindProperty("scoreCost").intValue = 10;
            serializedEnemyData.FindProperty("speed").floatValue = 3f;
            serializedEnemyData.FindProperty("timeBetweenAttacks").floatValue = 1f;
            if (enemyPrefab != null)
            {
                GameObject enemy = enemyPrefab as GameObject;
                serializedEnemyData.FindProperty("characterControllerPrefab").objectReferenceValue = enemy.GetComponent<CharacterController>();
                serializedEnemyData.FindProperty("characterPrefab").objectReferenceValue = enemy;
            }
            serializedEnemyData.ApplyModifiedProperties();
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void SetupPrefabs()
    {
        if (playerPrefab != null)
        {
            GameObject player = playerPrefab as GameObject;
            Character playerCharacter = player.GetComponent<Character>();
            if (playerCharacter != null)
            {
                SerializedObject serializedPlayer = new SerializedObject(playerCharacter);
                serializedPlayer.FindProperty("characterDataAsset").objectReferenceValue = playerData;
                serializedPlayer.ApplyModifiedProperties();
                
                // Удаляем компонент CharacterData если он есть
                CharacterData oldData = player.GetComponent<CharacterData>();
                if (oldData != null)
                {
                    DestroyImmediate(oldData, true);
                }
            }
        }

        if (enemyPrefab != null)
        {
            GameObject enemy = enemyPrefab as GameObject;
            Character enemyCharacter = enemy.GetComponent<Character>();
            if (enemyCharacter != null)
            {
                SerializedObject serializedEnemy = new SerializedObject(enemyCharacter);
                serializedEnemy.FindProperty("characterDataAsset").objectReferenceValue = enemyData;
                serializedEnemy.ApplyModifiedProperties();
                
                // Удаляем компонент CharacterData если он есть
                CharacterData oldData = enemy.GetComponent<CharacterData>();
                if (oldData != null)
                {
                    DestroyImmediate(oldData, true);
                }
            }
        }

        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(playerPrefab);
        EditorUtility.SetDirty(enemyPrefab);
    }
}