using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Reflection;

public class SceneAnalyzer : MonoBehaviour
{
    [MenuItem("Tools/Analyze Scene")]
    public static void AnalyzeScene()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== SCENE ANALYSIS ===");
        sb.AppendLine($"Scene name: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        sb.AppendLine("====================\n");
        
        GameObject[] allObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        
        foreach (GameObject obj in allObjects)
        {
            AnalyzeGameObject(obj, 0, sb);
        }
        
        string path = Path.Combine(Application.dataPath, "scene_analysis.txt");
        File.WriteAllText(path, sb.ToString());
        AssetDatabase.Refresh();
        
        Debug.Log($"Scene analysis saved to: {path}");
    }
    
    static void AnalyzeGameObject(GameObject obj, int depth, StringBuilder sb)
    {
        string indent = new string('-', depth * 2);
        
        // Basic GameObject info
        sb.AppendLine($"{indent}GameObject: {obj.name}");
        sb.AppendLine($"{indent}Tag: {obj.tag}");
        sb.AppendLine($"{indent}Layer: {LayerMask.LayerToName(obj.layer)}");
        sb.AppendLine($"{indent}Active: {obj.activeSelf}");
        
        // Transform info
        Transform transform = obj.transform;
        sb.AppendLine($"{indent}Transform:");
        sb.AppendLine($"{indent}  Position: {transform.position}");
        sb.AppendLine($"{indent}  Rotation: {transform.rotation.eulerAngles}");
        sb.AppendLine($"{indent}  Scale: {transform.localScale}");
        
        // All other components
        Component[] components = obj.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component == null || component is Transform) continue;
            
            sb.AppendLine($"{indent}Component: {component.GetType().Name}");
            
            // Get public properties
            PropertyInfo[] properties = component.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                try
                {
                    if (property.CanRead && property.GetIndexParameters().Length == 0)
                    {
                        object value = property.GetValue(component);
                        if (value != null)
                        {
                            sb.AppendLine($"{indent}  {property.Name}: {value}");
                        }
                    }
                }
                catch { }
            }
            
            // Get public fields
            FieldInfo[] fields = component.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                try
                {
                    object value = field.GetValue(component);
                    if (value != null)
                    {
                        sb.AppendLine($"{indent}  {field.Name}: {value}");
                    }
                }
                catch { }
            }
            
            // Special handling for specific components
            if (component is Renderer renderer)
            {
                sb.AppendLine($"{indent}  Materials:");
                foreach (Material mat in renderer.sharedMaterials)
                {
                    if (mat != null)
                        sb.AppendLine($"{indent}    - {mat.name}");
                }
            }
            else if (component is Collider collider)
            {
                sb.AppendLine($"{indent}  IsTrigger: {collider.isTrigger}");
                sb.AppendLine($"{indent}  Enabled: {collider.enabled}");
            }
            else if (component is Rigidbody rb)
            {
                sb.AppendLine($"{indent}  Mass: {rb.mass}");
                sb.AppendLine($"{indent}  UseGravity: {rb.useGravity}");
                sb.AppendLine($"{indent}  IsKinematic: {rb.isKinematic}");
            }
        }
        
        // Scripts
        MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script == null) continue;
            
            sb.AppendLine($"{indent}Script: {script.GetType().Name}");
            SerializedObject serializedScript = new SerializedObject(script);
            SerializedProperty property = serializedScript.GetIterator();
            
            while (property.Next(true))
            {
                if (property.propertyPath != "m_Script")
                {
                    sb.AppendLine($"{indent}  {property.propertyPath}: {GetSerializedPropertyValue(property)}");
                }
            }
        }
        
        sb.AppendLine(); // Empty line between objects
        
        // Analyze children
        foreach (Transform child in obj.transform)
        {
            AnalyzeGameObject(child.gameObject, depth + 1, sb);
        }
    }
    
    static string GetSerializedPropertyValue(SerializedProperty property)
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
            case SerializedPropertyType.Vector4:
                return property.vector4Value.ToString();
            case SerializedPropertyType.Quaternion:
                return property.quaternionValue.ToString();
            case SerializedPropertyType.Color:
                return property.colorValue.ToString();
            case SerializedPropertyType.ObjectReference:
                return property.objectReferenceValue ? property.objectReferenceValue.name : "None";
            default:
                return "Unable to read value";
        }
    }
}