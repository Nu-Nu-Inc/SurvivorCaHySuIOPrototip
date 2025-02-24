using UnityEngine;
using UnityEditor;
using System.IO;

public class CharacterDataEditor : EditorWindow
{
    private const string PLAYER_DATA_PATH = "Assets/Data/PlayerCharacterData.asset";
    private const string ENEMY_DATA_PATH = "Assets/Data/EnemyCharacterData.asset";

    [MenuItem("Tools/Character Data Creator")]
    public static void ShowWindow()
    {
        GetWindow<CharacterDataEditor>("Character Data Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("Character Data Creator", EditorStyles.boldLabel);

        if (GUILayout.Button("Create Player Data"))
        {
            CreatePlayerData();
        }

        if (GUILayout.Button("Create Enemy Data"))
        {
            CreateEnemyData();
        }

        if (GUILayout.Button("Create Both"))
        {
            CreatePlayerData();
            CreateEnemyData();
        }
    }

    private void CreatePlayerData()
    {
        // Создаем папку Data если её нет
        if (!Directory.Exists("Assets/Data"))
        {
            AssetDatabase.CreateFolder("Assets", "Data");
        }

        // Создаем ScriptableObject
        CharacterData playerData = ScriptableObject.CreateInstance<CharacterData>();
        
        // Настраиваем значения по умолчанию для игрока
        SerializedObject serializedObj = new SerializedObject(playerData);
        
        SerializedProperty scoreCostProp = serializedObj.FindProperty("scoreCost");
        SerializedProperty speedProp = serializedObj.FindProperty("speed");
        SerializedProperty timeBetweenAttacksProp = serializedObj.FindProperty("timeBetweenAttacks");
        
        scoreCostProp.intValue = 0;  // Игрок не дает очков
        speedProp.floatValue = 5f;   // Скорость игрока
        timeBetweenAttacksProp.floatValue = 0.5f;  // Время между атаками
        
        serializedObj.ApplyModifiedProperties();

        // Сохраняем asset
        AssetDatabase.CreateAsset(playerData, PLAYER_DATA_PATH);
        AssetDatabase.SaveAssets();
        Debug.Log("Created PlayerCharacterData at " + PLAYER_DATA_PATH);
    }

    private void CreateEnemyData()
    {
        // Создаем папку Data если её нет
        if (!Directory.Exists("Assets/Data"))
        {
            AssetDatabase.CreateFolder("Assets", "Data");
        }

        // Создаем ScriptableObject
        CharacterData enemyData = ScriptableObject.CreateInstance<CharacterData>();
        
        // Настраиваем значения по умолчанию для врага
        SerializedObject serializedObj = new SerializedObject(enemyData);
        
        SerializedProperty scoreCostProp = serializedObj.FindProperty("scoreCost");
        SerializedProperty speedProp = serializedObj.FindProperty("speed");
        SerializedProperty timeBetweenAttacksProp = serializedObj.FindProperty("timeBetweenAttacks");
        
        scoreCostProp.intValue = 10;  // Очки за убийство врага
        speedProp.floatValue = 3f;    // Скорость врага
        timeBetweenAttacksProp.floatValue = 1f;  // Время между атаками
        
        serializedObj.ApplyModifiedProperties();

        // Сохраняем asset
        AssetDatabase.CreateAsset(enemyData, ENEMY_DATA_PATH);
        AssetDatabase.SaveAssets();
        Debug.Log("Created EnemyCharacterData at " + ENEMY_DATA_PATH);
    }
}