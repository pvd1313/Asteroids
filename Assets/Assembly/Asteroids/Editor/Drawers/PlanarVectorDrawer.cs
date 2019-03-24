using UnityEditor;
using UnityEngine;
using AbruptLib.Editor;

namespace Asteroids.Editor
{
    [CustomPropertyDrawer(typeof(PlanarVector))]
    public class PlanarVectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUIUtility.wideMode = true;
            SerializedProperty x = property.FindPropertyRelative("X");
            SerializedProperty y = property.FindPropertyRelative("Y");

            GUIContent[] labels =
            {
                new GUIContent("x"),
                new GUIContent("y"),
            };
            double[] values = { x.doubleValue, y.doubleValue };

            var modifiedIndex = EditorGUIUtil.MultiField(position, label, labels, values, EditorGUI.DoubleField);
            if (modifiedIndex > -1)
            {
                if (modifiedIndex == 0) x.doubleValue = values[0];
                if (modifiedIndex == 1) y.doubleValue = values[1];
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 16;
        }
    }
}