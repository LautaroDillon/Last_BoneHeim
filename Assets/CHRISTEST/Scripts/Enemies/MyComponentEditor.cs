#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// Indica que este Editor es para MyComponent
[CustomEditor(typeof(E_Shooter))]
public class MyComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Obtenemos la instancia real
        var comp = (E_Shooter)target;

        GUI.color = Color.red;
        // Cambiamos el color del campo de la zona
        comp.zoneId = EditorGUILayout.IntField("zoneId", comp.zoneId);

        // Volvemos al color normal (blanco)
        GUI.color = Color.white;

        // Dibujamos el resto de campos automáticamente
        DrawDefaultInspector();
    }
}
#endif