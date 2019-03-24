using UnityEditor;
using UnityEngine;

namespace Asteroids.Gravity.Mono.Editor
{
    [CustomEditor(typeof(KeplerBody))]
    public class KeplerBodyEditor : UnityEditor.Editor
    {
        [SerializeField]
        private PlanarVector _position;
        [SerializeField]
        private PlanarVector _velocity;

        public override void OnInspectorGUI()
        {
            var body = target as KeplerBody;
            var gSource = body.Parent;
            if (gSource == null)
            {
                var message = string.Format("Parent must have {0}.", typeof(GravityField).Name);
                EditorGUILayout.HelpBox(message, MessageType.Error);
                return;
            }
            var bodyMotion = body.Motion;

            _velocity = body.LocalVelocity;
            _position = body.LocalPosition;
            var serialized = new SerializedObject(this);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serialized.FindProperty("_position"));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(body.GetComponent<OrbitTransform>(), "Changed body position");
                serialized.ApplyModifiedProperties();
                body.LocalPosition = _position;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serialized.FindProperty("_velocity"));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(body.GetComponent<OrbitTransform>(), "Changed body position");
                serialized.ApplyModifiedProperties();
                body.LocalVelocity = _velocity;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.DoubleField("Angle velocity", body.AngleSpeed);
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Invert", GUILayout.Height(16)))
            {
                Undo.RecordObject(target, "Inverted body velocity vector");
                body.LocalVelocity = -body.LocalVelocity;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(true);
            if (bodyMotion.Orbit.Periodic)
            {
                var period = MathUtil.TWO_PI / bodyMotion.AngleSpeed;
                EditorGUILayout.DoubleField("Period", period);
            }
            else
                EditorGUILayout.TextField("Period", "∞");
            EditorGUI.EndDisabledGroup();
        }

        private void OnSceneGUI()
        {
            var body = target as KeplerBody;
            var bodyTransform = body.transform;
            var gSource = body.Parent;
            if (gSource == null) return;

            var gSourceTransform = gSource.transform;

            var planarVelocity = body.LocalVelocity;
            var velocity = gSourceTransform.TransformDirection(VectorConverter.Convert(planarVelocity));

            //var size = HandleUtility.GetHandleSize(gReceiverTransform.position) * .5f;
            Handles.color = new Color(0, 1, 1, .5f);
            Handles.DrawLine(bodyTransform.position, bodyTransform.position + velocity);
        }
    }
}