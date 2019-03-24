using UnityEditor;
using UnityEngine;
using System;

namespace AbruptLib.Editor
{
    public static class EditorGUIUtil
    {
        public delegate T PropertyDrawFunction<T>(Rect position, GUIContent label, T value);

        public static bool MultiFieldDynamicLabelSize = true;
        public static int MultiFieldDisablingMask = 0;

        private static readonly int FOLDOUT_HASH = "Foldout".GetHashCode();

        public static int MultiField<T>(Rect position, GUIContent label, GUIContent[] subLabels, T[] values, PropertyDrawFunction<T> propertyDrawer)
        {
            int controlId = GUIUtility.GetControlID(FOLDOUT_HASH, FocusType.Keyboard, position);
            position = MultiFieldPrefixLabel(position, controlId, label, subLabels.Length);
            position.height = 16f;
            return MultiField(position, subLabels, values, propertyDrawer);
        }

        public static int MultiField<T>(Rect position, GUIContent[] subLabels, T[] values, PropertyDrawFunction<T> propertyDrawer)
        {
            int length = values.Length;
            float num = (position.width - (length - 1) * 2f) / length;
            Rect position1 = new Rect(position);
            position1.width = num;
            float labelWidth1 = EditorGUIUtility.labelWidth;
            int indentLevel = EditorGUI.indentLevel;
            
            EditorGUI.indentLevel = 0;
            var modifiedIndex = -1;
            for (int i = 0; i < values.Length; ++i)
            {
                EditorGUIUtility.labelWidth = MultiFieldDynamicLabelSize ? subLabels[i].text.Length * 8 + 5 : 13;
                EditorGUI.BeginChangeCheck();
                var disabled = MultiFieldDisablingMask & (int)Math.Pow(2, i);
                EditorGUI.BeginDisabledGroup(disabled != 0);
                values[i] = propertyDrawer(position1, subLabels[i], values[i]);
                EditorGUI.EndDisabledGroup();
                if (EditorGUI.EndChangeCheck()) modifiedIndex = i;
                position1.x += num + 2f;
            }
            EditorGUIUtility.labelWidth = labelWidth1;
            EditorGUI.indentLevel = indentLevel;
            MultiFieldDisablingMask = 0;
            return modifiedIndex;
        }

        private static Rect MultiFieldPrefixLabel(Rect totalPosition, int id, GUIContent label, int columns)
        {
            if (!LabelHasContent(label))
                return EditorGUI.IndentedRect(totalPosition);
            if (EditorGUIUtility.wideMode)
            {
                Rect labelPosition = new Rect(totalPosition.x + GetIndent(), totalPosition.y, EditorGUIUtility.labelWidth - GetIndent(), 16);
                Rect rect = totalPosition;
                rect.xMin += EditorGUIUtility.labelWidth;
                if (columns > 1)
                {
                    --labelPosition.width;
                    --rect.xMin;
                }
                EditorGUI.HandlePrefixLabel(totalPosition, labelPosition, label, id);
                return rect;
            }
            Rect labelPosition1 = new Rect(totalPosition.x + GetIndent(), totalPosition.y, totalPosition.width - GetIndent(), 16f);
            Rect rect1 = totalPosition;
            rect1.xMin += GetIndent() + 15;
            rect1.yMin += 16;
            EditorGUI.HandlePrefixLabel(totalPosition, labelPosition1, label, id);
            return rect1;
        }

        private static float GetIndent()
        {
            return EditorGUI.indentLevel * 15f;
        }

        private static bool LabelHasContent(GUIContent label)
        {
            if (label == null) return true;
            return label.text != string.Empty || label.image != null;
        }
    }
}