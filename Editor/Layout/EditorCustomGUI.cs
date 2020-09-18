using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BitActionSwitch.Editor.Layout
{
    public sealed class EditorCustomGUI
    {
        public static Color HeaderBackgroundColor =>
            EditorGUIUtility.isProSkin ?  new Color(0.1f, 0.1f, 0.1f, 0.2f) : new Color(1f, 1f, 1f, 0.2f);

        internal static readonly GUIStyle RemoveButtonStyle = new GUIStyle(GUI.skin.box)
        {
            fontStyle = FontStyle.Normal, 
            normal =
            {
                textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black
            }
        };
        
        private static readonly Stack<Color> ErroredStack = new Stack<Color>();

        #region ErrorCheckScope

        public static void BeginErrorCheck(Func<bool> validate)
        {
            ErroredStack.Push(GUI.backgroundColor);
            if(!validate.Invoke()) GUI.backgroundColor = Color.red;
        }

        public static void EndErrorCheck()
        {
            GUI.backgroundColor = ErroredStack.Pop();
        }

        #endregion
        
        public static void ObjectField<T>(Rect rect, string title, T content, bool showRemoveButton, bool allowSceneObject, Action<T> onValueChanged)
            where T : UnityEngine.Object
        {
            EditorGUI.BeginChangeCheck();
            T value;
            if (showRemoveButton)
            {
                var contentRect = new Rect(rect) {width = rect.width - 30};
                var removeButtonRect = new Rect(contentRect)
                {
                    x = contentRect.x + contentRect.width + 4,
                    width = 25, 
                    height = EditorGUIUtility.singleLineHeight
                };
                EditorGUILayout.BeginHorizontal();
                value = (T) EditorGUI.ObjectField(contentRect, title, content, typeof(T), allowSceneObject);
                if (GUI.Button(removeButtonRect, "×", RemoveButtonStyle)) value = default; 
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                value = (T) EditorGUI.ObjectField(rect, title, content, typeof(T), allowSceneObject);
            }
            if (!EditorGUI.EndChangeCheck()) return;
            onValueChanged.Invoke(value);
        }
        
        public static void ObjectField<T>(Rect rect, string title, T content, bool allowSceneObject, bool showRemoveButton,
            Action<T> onValueChanged, Func<bool> validate)
            where T : UnityEngine.Object
        {
            EditorGUI.BeginChangeCheck();
            T value;
            if (showRemoveButton)
            {
                var contentRect = new Rect(rect) {width = rect.width - 30};
                var removeButtonRect = new Rect(contentRect)
                {
                    x = contentRect.x + contentRect.width + 4,
                    width = 25, 
                    height = EditorGUIUtility.singleLineHeight
                };
                EditorGUILayout.BeginHorizontal();
                BeginErrorCheck(validate);
                value = (T) EditorGUI.ObjectField(contentRect, title, content, typeof(T), allowSceneObject);
                EndErrorCheck();
                if (GUI.Button(removeButtonRect, "×", RemoveButtonStyle)) value = default; 
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                BeginErrorCheck(validate);
                value = (T) EditorGUI.ObjectField(rect, title, content, typeof(T), allowSceneObject);
                EndErrorCheck();
            }
            if (!EditorGUI.EndChangeCheck()) return;
            onValueChanged.Invoke(value);
        }
        
        public static void TextField(Rect rect, string title, string text, Action<string> onValueChanged)
        {
            EditorGUI.BeginChangeCheck();
            var value = EditorGUI.TextField(rect, title, text);
            if (!EditorGUI.EndChangeCheck()) return;
            onValueChanged.Invoke(value);
        }
        
        public static void TextField(Rect rect, string title, string text, Action<string> onValueChanged, Func<bool> validate)
        {
            EditorGUI.BeginChangeCheck();
            BeginErrorCheck(validate);
            var value = EditorGUI.TextField(rect, title, text);
            EndErrorCheck();
            if (!EditorGUI.EndChangeCheck()) return;
            onValueChanged.Invoke(value);
        }
        
        public static void EnumPopup<T>(Rect rect, string title, Enum content, Action<T> onValueChanged) where T : Enum
        {
            EditorGUI.BeginChangeCheck();
            var value = (T)EditorGUI.EnumPopup(rect, title, content);
            if (!EditorGUI.EndChangeCheck()) return;
            onValueChanged.Invoke(value);
        }
        
        public static void Toggle(Rect rect, string title, bool value, Action<bool> onValueChanged)
        {
            EditorGUI.BeginChangeCheck();
            var b = EditorGUI.Toggle(rect, title, value);
            if (!EditorGUI.EndChangeCheck()) return;
            onValueChanged.Invoke(b);
        }
        
        public class ErrorCheckScope : GUI.Scope
        {
            public ErrorCheckScope(Func<bool> validate)
            {
                BeginErrorCheck(validate);
            }
            
            protected override void CloseScope()
            {
                EndErrorCheck();
            }
        }
    }
}