using UnityEngine;
using UnityEditor;
using UnityEngine.UI; // Добавляем для работы с UI элементами

public class SceneSetup : EditorWindow
{
    [MenuItem("Tools/Scene Setup")]
    public static void ShowWindow()
    {
        GetWindow<SceneSetup>("Scene Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("Scene Setup Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Setup Character Data"))
        {
            CreateCharacterData();
        }

        if (GUILayout.Button("Setup Game Objects"))
        {
            SetupGameObjects();
        }
    }

    private void CreateCharacterData()
    {
        // Создаем папку для данных если её нет
        if (!System.IO.Directory.Exists("Assets/Data"))
        {
            AssetDatabase.CreateFolder("Assets", "Data");
        }

        // Создаем CharacterData для игрока
        CharacterData playerData = ScriptableObject.CreateInstance<CharacterData>();
        AssetDatabase.CreateAsset(playerData, "Assets/Data/PlayerCharacterData.asset");
        
        // Создаем CharacterData для врага
        CharacterData enemyData = ScriptableObject.CreateInstance<CharacterData>();
        AssetDatabase.CreateAsset(enemyData, "Assets/Data/EnemyCharacterData.asset");
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Character Data files created in Assets/Data folder");
    }

    private void SetupGameObjects()
    {
        GameObject gameManager = GameObject.Find("GameManager1");
        if (gameManager != null)
        {
            GameManager manager = gameManager.GetComponent<GameManager>();
            if (manager != null)
            {
                // Находим UI элементы
                Text scoreText = GameObject.Find("ScoreText")?.GetComponent<Text>();
                Text timerText = GameObject.Find("TimerText")?.GetComponent<Text>();
                
                // Присваиваем их GameManager
                SerializedObject serializedManager = new SerializedObject(manager);
                SerializedProperty scoreTextProperty = serializedManager.FindProperty("scoreText");
                SerializedProperty timerTextProperty = serializedManager.FindProperty("timerText");
                
                if (scoreText != null) 
                {
                    scoreTextProperty.objectReferenceValue = scoreText;
                    Debug.Log("ScoreText assigned");
                }
                else
                {
                    Debug.LogWarning("ScoreText not found in scene");
                }

                if (timerText != null) 
                {
                    timerTextProperty.objectReferenceValue = timerText;
                    Debug.Log("TimerText assigned");
                }
                else
                {
                    Debug.LogWarning("TimerText not found in scene");
                }
                
                serializedManager.ApplyModifiedProperties();
                Debug.Log("GameManager setup completed");
            }
            else
            {
                Debug.LogError("GameManager component not found on GameManager1");
            }
        }
        else
        {
            Debug.LogError("GameManager1 not found in scene");
        }
    }
}