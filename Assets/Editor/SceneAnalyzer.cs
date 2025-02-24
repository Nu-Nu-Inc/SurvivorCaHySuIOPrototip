using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class SceneAnalyzer : EditorWindow
{
    [MenuItem("Tools/Analyze Scene")]
    public static void AnalyzeScene()
    {
        StringBuilder sb = new StringBuilder();
        
        // Получаем все объекты на сцене
        GameObject[] allObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        
        sb.AppendLine("=== SCENE ANALYSIS ===");
        sb.AppendLine($"Scene name: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        sb.AppendLine("====================\n");

        foreach (GameObject obj in allObjects)
        {
            AnalyzeGameObject(obj, 0, sb);
        }
        
        // Сохраняем в файл
        string path = Path.Combine(Application.dataPath, "scene_analysis.txt");
        File.WriteAllText(path, sb.ToString());
        AssetDatabase.Refresh();
        
        Debug.Log($"Scene analysis saved to: {path}");

        // Открываем файл для просмотра
        EditorUtility.OpenWithDefaultApp(path);
    }
    
    static void AnalyzeGameObject(GameObject obj, int depth, StringBuilder sb)
    {
        string indent = new string('-', depth * 2);
        
        // Информация об объекте
        sb.AppendLine($"{indent}GameObject: {obj.name}");
        sb.AppendLine($"{indent}Tag: {obj.tag}");
        sb.AppendLine($"{indent}Layer: {LayerMask.LayerToName(obj.layer)}");
        sb.AppendLine($"{indent}Active: {obj.activeSelf}");
        
        // Transform
        Transform transform = obj.transform;
        sb.AppendLine($"{indent}Transform:");
        sb.AppendLine($"{indent}  Position: {transform.position}");
        sb.AppendLine($"{indent}  Rotation: {transform.rotation.eulerAngles}");
        sb.AppendLine($"{indent}  Scale: {transform.localScale}");
        
        // Компоненты
        Component[] components = obj.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component == null || component is Transform) continue;
            
            sb.AppendLine($"{indent}Component: {component.GetType().Name}");

            // Специальная обработка для некоторых компонентов
            if (component is Renderer renderer)
            {
                sb.AppendLine($"{indent}  Materials:");
                foreach (Material mat in renderer.sharedMaterials)
                {
                    if (mat != null)
                        sb.AppendLine($"{indent}    - {mat.name}");
                }
            }
            else if (component is MonoBehaviour script)
            {
                SerializedObject serializedScript = new SerializedObject(script);
                SerializedProperty property = serializedScript.GetIterator();
                
                while (property.Next(true))
                {
                    if (property.propertyType != SerializedPropertyType.Generic && !property.propertyPath.Contains("m_"))
                    {
                        sb.AppendLine($"{indent}  {property.propertyPath}: {GetPropertyValue(property)}");
                    }
                }
            }
        }
        
        sb.AppendLine();
        
        // Рекурсивно анализируем дочерние объекты
        foreach (Transform child in obj.transform)
        {
            AnalyzeGameObject(child.gameObject, depth + 1, sb);
        }
    }

    static string GetPropertyValue(SerializedProperty property)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.Integer:
                return property.intValue.ToString();
            case SerializedPropertyType.Boolean:
                return property.boolValue.ToString();
            case SerializedPropertyType.Float:
                return property.floatValue.ToString();
            case SerializedPropertyType.String:
                return property.stringValue;
            case SerializedPropertyType.Vector2:
                return property.vector2Value.ToString();
            case SerializedPropertyType.Vector3:
                return property.vector3Value.ToString();
            case SerializedPropertyType.Color:
                return property.colorValue.ToString();
            case SerializedPropertyType.ObjectReference:
                return property.objectReferenceValue ? property.objectReferenceValue.name : "None";
            default:
                return "Unable to read value";
        }
    }
}