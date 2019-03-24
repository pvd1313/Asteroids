using UnityEditor;
using UnityEngine;
using AbruptLib;
using AbruptLib.Editor;
using System.Collections.Generic;
using System;

namespace Asteroids.Gravity.Mono.Editor
{
    [CustomEditor(typeof(OrbitTransform))]
    public class OrbitTransformEditor : UnityEditor.Editor
    {
        private readonly List<Vector3> PointsBuffer = new List<Vector3>();

        private int _transformID;
        private Tool _lastTool;
        [SerializeField]
        private Orbit _orbit;
        [SerializeField]
        private double _angle;


        // TODO: Add display of min/max circular body velocity/periods to make gamedesign simplier.
        public override void OnInspectorGUI()
        {
            var transform = target as OrbitTransform;
            _orbit = transform.Orbit;
            _angle = transform.Angle;
            var obj = new SerializedObject(this);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(obj.FindProperty("_orbit"));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed orbit elements");
                obj.ApplyModifiedProperties();
                transform.Orbit = new Orbit(_orbit.Eccentricity, _orbit.Periapsis, _orbit.Argument);
            }

            EditorGUI.BeginChangeCheck();
            // TODO: Understand why this is not working.
            //EditorGUILayout.PropertyField(obj.FindProperty("_meanAnomaly"));
            _angle = EditorGUILayout.DoubleField("Angle", _angle);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed angle");
                obj.ApplyModifiedProperties();
                transform.Angle = _angle;
            }
        }

        private void OnSceneGUI()
        {
            var orbitTransform = target as OrbitTransform;
            var transform = orbitTransform.transform;
            var orbit = orbitTransform.Orbit;
            var parent = transform.WrapParent();
            var parentPosition = parent.Position;
            var pericenter = orbitTransform.Pericenter;
            var apocenter = orbitTransform.Apocenter;
            var handlesSize = HandleUtility.GetHandleSize(parentPosition);
            var xAxis = parent.PlanarX;
            var zAxis = parent.PlanarZ;
            var pericenterDirection = (Quaternion.AngleAxis((float)((Math.PI - orbit.Argument) * MathUtil.Rad2Deg), zAxis) * -xAxis).normalized;

            // Draw visual handles
            //Handles.color = new Color(1f, 1f, 1f, .05f);
            //Handles.DrawLine(pericenter, apocenter);
            Handles.color = new Color(1f, 1f, 1f, .6f);
            var localPath = orbitTransform.LocalPath;
            parent.TransformPoints(localPath.Curve, PointsBuffer);
            HandlesUtil.DrawPolyline(PointsBuffer, localPath.Enclosed);

            // Argument handle
            Handles.color = new Color(0.0f, 0.3f, 1f);
            var pericenterHandleSize = HandleUtility.GetHandleSize(pericenter);
            var newPericenter = Handles.FreeMoveHandle(pericenter, Quaternion.LookRotation(xAxis, zAxis), pericenterHandleSize * .1f, Vector3.zero, Handles.CircleHandleCap);
            var argumentChanged = newPericenter != pericenter;
            var newVector = (newPericenter - parentPosition).normalized;
            var newArgument = VectorUtil.SignedAngle(newVector, xAxis, zAxis);

            // Eccentricity handle
            Handles.color = new Color(1f, .02f, .02f);
            var eccentricity = (float)orbit.Eccentricity;
            if (eccentricity < 0.01f) eccentricity = 0.01f;
            var handleRotation = Quaternion.LookRotation(-pericenterDirection, zAxis);
            var handlePosition = pericenter - pericenterDirection * pericenterHandleSize * .1f;
            var newEccentricity = Mathf.Abs(Handles.ScaleSlider(eccentricity, handlePosition, -pericenterDirection, handleRotation, handlesSize * .6f, .01f));
            var eccentricityChanged = newEccentricity != eccentricity;

            // Periapsis handle
            Handles.color = new Color(.04f, 1, .04f);
            var expectedPosition = pericenter + pericenterDirection * pericenterHandleSize * .1f;
            var newPosition = Handles.Slider(expectedPosition, pericenterDirection, pericenterHandleSize * .6f, Handles.ArrowHandleCap, .00f);
            var periapsisChanged = newPosition != expectedPosition;
            var newPeriapsis = Vector3.Distance(parentPosition, newPosition) - pericenterHandleSize * .1f;

            if (eccentricityChanged || argumentChanged || periapsisChanged)
            {
                var finalEccentricity = eccentricityChanged ? newEccentricity : orbit.Eccentricity;
                var finalArgument = argumentChanged ? MathUtil.RepeatAngle(newArgument) : orbit.Argument;
                var finalPeriapsis = periapsisChanged ? newPeriapsis : orbit.Periapsis;
                Undo.RecordObject(orbitTransform, "Orbit changed");
                orbitTransform.Orbit = new Orbit(finalEccentricity, finalPeriapsis, finalArgument);
            }

            Tools.current = Tool.None;
        }

        private void OnEnable()
        {
            if (target == null) return;
            var conic = target as OrbitTransform;
            var transform = conic.transform;
            _transformID = transform.GetInstanceID();
            //transform.hideFlags = HideFlags.NotEditable;
            UnityEditorInternal.ComponentUtility.MoveComponentUp(conic);
            _lastTool = Tools.current;
            Tools.current = Tool.None;
        }

        private void OnDisable()
        {
            var transform = EditorUtility.InstanceIDToObject(_transformID) as Transform;
            if (transform == null) return;
            //transform.hideFlags = HideFlags.None;
            Tools.current = _lastTool;
        }
    }
}