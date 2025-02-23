using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class SceneAnalyzer : MonoBehaviour
{
    [MenuItem("Tools/Analyze Scene")]
    public static void AnalyzeScene()
    {
        StringBuilder sb = new StringBuilder();
        
        // Получаем все объекты на сцене
        GameObject[] allObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        
        foreach (GameObject obj in allObjects)
        {
            AnalyzeGameObject(obj, 0, sb);
        }
        
        // Сохраняем в файл
        string path = Path.Combine(Application.dataPath, "scene_analysis.txt");
        File.WriteAllText(path, sb.ToString());
        AssetDatabase.Refresh();
        
        Debug.Log($"Scene analysis saved to: {path}");
    }
    
    static void AnalyzeGameObject(GameObject obj, int depth, StringBuilder sb)
    {
        string indent = new string('-', depth * 2);
        sb.AppendLine($"{indent}{obj.name}");
        
        // Записываем все компоненты
        Component[] components = obj.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component != null)
                sb.AppendLine($"{indent}  Component: {component.GetType().Name}");
        }
        
        // Рекурсивно анализируем дочерние объекты
        foreach (Transform child in obj.transform)
        {
            AnalyzeGameObject(child.gameObject, depth + 1, sb);
        }
    }
}