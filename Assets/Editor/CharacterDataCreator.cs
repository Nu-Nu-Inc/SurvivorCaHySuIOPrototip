using UnityEngine;
using UnityEditor;

public class CharacterDataCreator : EditorWindow
{
    [MenuItem("Assets/Create/Game Data/Character Data")]
    public static void CreateCharacterData()
    {
        // Создаем экземпляр CharacterData
        CharacterData asset = ScriptableObject.CreateInstance<CharacterData>();

        // Получаем путь к выбранной папке
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path))
        {
            path = "Assets";
        }
        else if (!System.IO.Directory.Exists(path))
        {
            path = System.IO.Path.GetDirectoryName(path);
        }

        // Создаем файл
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New Character Data.asset");
        AssetDatabase.CreateAsset(asset, assetPathAndName);

        // Сохраняем изменения
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Выделяем созданный файл
        Selection.activeObject = asset;
    }
}