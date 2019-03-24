using UnityEngine;
using UnityEditor;
using AbruptLib.Editor;

namespace Asteroids.Editor
{
    [CustomPropertyDrawer(typeof(MinMax))]
    public class MinMaxDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUIUtility.wideMode = true;

            var min = property.FindPropertyRelative("Min");
            var max = property.FindPropertyRelative("Max");

            GUIContent[] labels = { new GUIContent("min", "Minimum value"), new GUIContent("max", "Maximum value") };
            double[] values = { min.doubleValue, max.doubleValue };

            if (EditorGUIUtil.MultiField(position, label, labels, values, EditorGUI.DoubleField) > -1)
            {
                min.doubleValue = values[0];
                max.doubleValue = values[1];
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 16;
        }
    }
}