using UnityEditor;
using UnityEngine;
using AbruptLib.Editor;

namespace Asteroids.Gravity.Mono.Editor
{
    [CustomEditor(typeof(GravityField))]
    public class GravitySourceEditor : UnityEditor.Editor
    {
        private static bool EditMassMode;

        [SerializeField]
        private MinMax _gravity;

        public override void OnInspectorGUI()
        {
            var gSource = target as GravityField;
            _gravity = gSource.Gravity;
            var serializedEditor = new SerializedObject(this);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();

            EditorGUIUtil.MultiFieldDisablingMask = EditMassMode ? 0 : 3;
            var content = new GUIContent("Field", "Properties of gravity field body.");
            GUIContent[] labels =
            {
                new GUIContent("m", "Mass"),
                new GUIContent("r", "Surface radius"),
                new GUIContent("R", "Influence radius"),
            };
            double[] values = { gSource.Mass, gSource.SurfaceRadius, gSource.InfluenceRadius };
            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(18));
            EditorGUIUtility.wideMode = true;
            EditorGUIUtil.MultiField(rect, content, labels, values, EditorGUI.DoubleField);

            EditorGUI.BeginDisabledGroup(EditMassMode);
            content = new GUIContent("Gravity", "Gravity force at closest and farthest distances.");
            EditorGUILayout.PropertyField(serializedEditor.FindProperty("_gravity"), content);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

            var buttonContent = new GUIContent(EditMassMode ? "m" : "G", "Toggle edit mode.");
            if (GUILayout.Button(buttonContent, GUILayout.Width(32), GUILayout.Height(33))) EditMassMode = !EditMassMode;

            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed gravity source properties");
                serializedEditor.ApplyModifiedProperties();
                if (EditMassMode)
                    gSource.Modify(values[0], values[1], values[2]);
                else
                    gSource.Modify(values[2], _gravity);
            }
        }

        private void OnSceneGUI()
        {
            var gSource = target as GravityField;
            var transform = gSource.transform;
            var gravity = gSource.Gravity;
            var zAxis = transform.TransformDirection(VectorConverter.planarZ).normalized;
            var xAxis = transform.TransformDirection(VectorConverter.planarX).normalized;
            var rotation = Quaternion.LookRotation(xAxis, zAxis);
            Handles.color = new Color(1f, 1f, 1f, .6f);

            var influenceRadius = (float)gSource.InfluenceRadius;
            EditorGUI.BeginChangeCheck();
            var newInfluenceRadius = HandlesUtil.DiscRadiusHandle(rotation, transform.position, influenceRadius);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(gSource, "Changed gravity source influence radius");
                if (EditMassMode)
                    gSource.Modify(gSource.Mass, gSource.SurfaceRadius, newInfluenceRadius);
                else
                    gSource.Modify(newInfluenceRadius, gravity);
            }

            var surfaceRadius = (float)gSource.SurfaceRadius;
            if (EditMassMode)
            {
                EditorGUI.BeginChangeCheck();
                var newSurfaceRadius = HandlesUtil.DiscRadiusHandle(rotation, transform.position, surfaceRadius);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(gSource, "Changed gravity source surface radius");
                    gSource.Modify(gSource.Mass, newSurfaceRadius, gSource.InfluenceRadius);
                }
            }
            else
                Handles.DrawWireDisc(transform.position, zAxis, surfaceRadius);
        }
    }
}