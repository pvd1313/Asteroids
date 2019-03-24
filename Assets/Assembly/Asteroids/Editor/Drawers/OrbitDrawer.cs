using UnityEditor;
using UnityEngine;
using AbruptLib.Editor;
using System;

namespace Asteroids.Gravity.Editor
{
    [CustomPropertyDrawer(typeof(Orbit))]
    public class OrbitDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUIUtility.wideMode = true;

            GUIContent[] labels =
            {
                new GUIContent("e", "Eccentricity of the orbit (scale)"),
                new GUIContent("r", "Peri-apsis of the orbit (nearest distance)"),
                new GUIContent("ω", "Argument of Peri-apsis (inclination)")
            };
            SerializedProperty eccentiricity = property.FindPropertyRelative("_eccentricity");
            SerializedProperty periapsis = property.FindPropertyRelative("_periapsis");
            SerializedProperty argument = property.FindPropertyRelative("_argument");
            double[] values = { eccentiricity.doubleValue, periapsis.doubleValue, argument.doubleValue };

            var modifiedIndex = EditorGUIUtil.MultiField(position, label, labels, values, EditorGUI.DoubleField);
            if (modifiedIndex > -1)
            {
                if (modifiedIndex == 0) eccentiricity.doubleValue = values[0];
                if (modifiedIndex == 1) periapsis.doubleValue = values[1];
                if (modifiedIndex == 2) argument.doubleValue = values[2];
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 16;
        }
    }
}