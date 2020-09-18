using System;
using System.Collections.Generic;
using BitActionSwitch.Editor.Utility;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BitActionSwitch.Editor.Layout
{
    public sealed class EditorCustomGUILayout
    {
        private static readonly Stack<bool> PropertyFoldGroupStack = new Stack<bool>();
        private static readonly Stack<FoldToggleParameter> PropertyToggleFoldGroupStack = new Stack<FoldToggleParameter>();
        private static readonly Stack<ObjectFieldToggleParameter> ObjectFieldFoldGroupStack = new Stack<ObjectFieldToggleParameter>();

        #region PropertyFoldGroup

        public static void BeginPropertyFoldGroup(string title, bool fold)
        {
            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            var labelRect = backgroundRect;
            labelRect.xMin += 32f;
            labelRect.xMax -= 20f;

            var foldoutRect = backgroundRect;
            foldoutRect.y += 1f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            // Background
            EditorGUI.DrawRect(backgroundRect, Styles.HeaderBackgroundColor);

            // Title
            EditorGUI.LabelField(labelRect, new GUIContent(title), EditorStyles.boldLabel);

            // foldout
            fold = GUI.Toggle(foldoutRect, fold, GUIContent.none, EditorStyles.foldout);

            // Handle events
            var e = Event.current;

            if (e.type == EventType.MouseDown)
            {
                if (labelRect.Contains(e.mousePosition))
                {
                    if (e.button == 0)
                    {
                        fold = !fold;
                    }

                    e.Use();
                }
            }
            
            PropertyFoldGroupStack.Push(fold);   
        }
        
        public static bool EndPropertyFoldGroup()
        {
            GUILayout.Space(1);
            return PropertyFoldGroupStack.Pop();   
        }
        

        #endregion

        #region PropertyToggleFoldGroup

        public static void BeginPropertyToggleFoldGroup(string title, FoldToggleParameter foldToggleParameter)
        {
            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            var labelRect = backgroundRect;
            labelRect.xMin += 32f;
            labelRect.xMax -= 20f;

            var foldoutRect = backgroundRect;
            foldoutRect.y += 1f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            var toggleRect = backgroundRect;
            toggleRect.x += 16f;
            toggleRect.y += 2f;
            toggleRect.width = 13f;
            toggleRect.height = 13f;

            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            // Background
            EditorGUI.DrawRect(backgroundRect, Styles.HeaderBackgroundColor);

            // Title
            using (new EditorGUI.DisabledScope(!foldToggleParameter.IsChecked))
            {
                EditorGUI.LabelField(labelRect, new GUIContent(title), EditorStyles.boldLabel);
            }

            // foldout
            foldToggleParameter.Fold = GUI.Toggle(foldoutRect, foldToggleParameter.Fold, GUIContent.none, EditorStyles.foldout);

            // Active checkbox
            foldToggleParameter.IsChecked = GUI.Toggle(toggleRect, foldToggleParameter.IsChecked, GUIContent.none, Styles.SmallTickBox);

            // Handle events
            var e = Event.current;

            if (e.type == EventType.MouseDown)
            {
                if (labelRect.Contains(e.mousePosition))
                {
                    if (e.button == 0)
                    {
                        foldToggleParameter.Fold = !foldToggleParameter.Fold;
                    }

                    e.Use();
                }
            }
            
            EditorGUI.BeginDisabledGroup(!foldToggleParameter.IsChecked);
            
            PropertyToggleFoldGroupStack.Push(foldToggleParameter);
        }


        public static FoldToggleParameter EndPropertyToggleFoldGroup()
        {
            GUILayout.Space(1);
            EditorGUI.EndDisabledGroup();
            return PropertyToggleFoldGroupStack.Pop();
        }

        #endregion

        #region ObjectFieldFoldGroup

        public static void BeginObjectFieldFoldGroup<T>(string title, ObjectFieldToggleParameter parameter, bool allowSceneObject, bool showRemoveButton, Action<T> onValueChanged, Func<bool> validate) where T : Object
        {
            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            var labelRect = backgroundRect;
            labelRect.xMin += 32f;
            labelRect.xMax -= 20f;

            var foldoutRect = backgroundRect;
            foldoutRect.y += 1f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            // Background
            EditorGUI.DrawRect(backgroundRect, Styles.HeaderBackgroundColor);

            var origFontStyle = EditorStyles.label.fontStyle;
            EditorStyles.label.fontStyle = FontStyle.Bold;
            EditorCustomGUI.ObjectField(labelRect, title, (T)parameter.Content, allowSceneObject, showRemoveButton, onValueChanged, validate);
            EditorStyles.label.fontStyle = origFontStyle;
            
            // foldout
            parameter.Fold = GUI.Toggle(foldoutRect, parameter.Fold, GUIContent.none, EditorStyles.foldout);

            // Handle events
            var e = Event.current;

            if (e.type == EventType.MouseDown)
            {
                if (labelRect.Contains(e.mousePosition))
                {
                    if (e.button == 0)
                    {
                        parameter.Fold = !parameter.Fold;
                    }

                    e.Use();
                }
            }

            EditorGUILayout.BeginVertical();
            ObjectFieldFoldGroupStack.Push(parameter);
        }

        public static ObjectFieldToggleParameter EndObjectFieldFoldGroup()
        {
            EditorGUILayout.EndVertical();
            GUILayout.Space(1);
            return ObjectFieldFoldGroupStack.Pop();
        }

        #endregion

        #region ObjectField

        public static void ObjectField<T>(string title, T content, bool allowSceneObject, bool showRemoveButton, Action<T> onValueChanged, params GUILayoutOption[] options)
            where T : Object
        {
            EditorGUI.BeginChangeCheck();
            T value;
            if (showRemoveButton)
            {
                EditorGUILayout.BeginHorizontal();
                value = (T) EditorGUILayout.ObjectField(title, content, typeof(T), allowSceneObject, options);
                if (GUILayout.Button("×", EditorCustomGUI.RemoveButtonStyle, GUILayout.Width(25), GUILayout.Height(EditorGUIUtility.singleLineHeight))) value = default; 
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                value = (T) EditorGUILayout.ObjectField(title, content, typeof(T), allowSceneObject, options);
            }
            
            if (!EditorGUI.EndChangeCheck()) return;
            onValueChanged.Invoke(value);
        }

        public static void ObjectField<T>(string title, T content, bool allowSceneObject, bool showRemoveButton, Action<T> onValueChanged, Func<bool> validate, params GUILayoutOption[] options)
            where T : Object
        {
            EditorGUI.BeginChangeCheck();
            EditorCustomGUI.BeginErrorCheck(validate);
            T value;
            if (showRemoveButton)
            {
                EditorGUILayout.BeginHorizontal();
                value = (T) EditorGUILayout.ObjectField(title, content, typeof(T), allowSceneObject, options);
                EditorCustomGUI.EndErrorCheck();
                if (GUILayout.Button("×", EditorCustomGUI.RemoveButtonStyle,GUILayout.Width(25), GUILayout.Height(EditorGUIUtility.singleLineHeight))) value = default; 
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                value = (T) EditorGUILayout.ObjectField(title, content, typeof(T), allowSceneObject, options);
                EditorCustomGUI.EndErrorCheck();
            }
            if (!EditorGUI.EndChangeCheck()) return;
            onValueChanged.Invoke(value);
        }

        #endregion

        #region TextField
        
        public static void TextField(string title, string text, Action<string> onValueChanged)
        {
            EditorGUI.BeginChangeCheck();
            var value = EditorGUILayout.TextField(title, text);
            if (!EditorGUI.EndChangeCheck()) return;
            onValueChanged.Invoke(value);
        }

        public static void TextField(string title, string text, Action<string> onValueChanged, Func<bool> validate)
        {
            EditorGUI.BeginChangeCheck();
            EditorCustomGUI.BeginErrorCheck(validate);
            var value = EditorGUILayout.TextField(title, text);
            EditorCustomGUI.EndErrorCheck();
            if (!EditorGUI.EndChangeCheck()) return;
            onValueChanged.Invoke(value);
        }

        #endregion

        #region FolderPathField

        public static void FolderPathField(string label, string text, string title, Action<string> onValueChanged)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                var value = EditorGUILayout.TextField(label, text);
                if (EditorGUI.EndChangeCheck()) onValueChanged.Invoke(value);

                if (GUILayout.Button("...", GUILayout.Width(30)))
                {
                    onValueChanged(FolderUtil.GetSaveFolderPath(title));
                }
            }
        }
        
        public static void FolderPathField(string label, string text, string title, Action<string> onValueChanged, Func<bool> validate)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                EditorCustomGUI.BeginErrorCheck(validate);
                var value = EditorGUILayout.TextField(label, text);
                EditorCustomGUI.EndErrorCheck();
                if (EditorGUI.EndChangeCheck()) onValueChanged.Invoke(value);

                if (GUILayout.Button("...", EditorStyles.miniButton, GUILayout.Width(30)))
                {
                    onValueChanged(FolderUtil.GetSaveFolderPath(title));
                }
            }
        }

        #endregion

        public static void EnumPopup<T>(string title, Enum content, Action<T> onValueChanged, params GUILayoutOption[] options) where T : Enum
        {
            EditorGUI.BeginChangeCheck();
            var value = (T)EditorGUILayout.EnumPopup(title, content, options);
            if (!EditorGUI.EndChangeCheck()) return;
            onValueChanged.Invoke(value);
        }


        public class FoldToggleParameter
        {
            public FoldToggleParameter(bool fold, bool isChecked)
            {
                this.Fold = fold;
                this.IsChecked = isChecked;
            }

            public bool Fold { get; set; }
            
            public bool IsChecked { get; set; }
        }
        
        public class ObjectFieldToggleParameter
        {
            public ObjectFieldToggleParameter(Object content, bool fold)
            {
                this.Fold = fold;
                this.Content = content;
            }

            public bool Fold { get; set; }
            
            public Object Content { get; set; }
        }
        
        public class PropertyFoldGroupScope : GUI.Scope
        {
            private bool changeFold;
            private bool fold;
            
            public PropertyFoldGroupScope(string title, bool fold)
            {
                BeginPropertyFoldGroup(title, fold);
            }

            public bool Fold
            {
                get
                {
                    if (this.changeFold) return this.fold;
                    
                    this.changeFold = true;
                    this.fold = EndPropertyFoldGroup();
                    return this.fold;
                }
            }

            protected override void CloseScope()
            {
                if (this.changeFold) return;
                EndPropertyFoldGroup();
            }
        }
        
        public class PropertyToggleFoldGroupScope : GUI.Scope
        {
            private bool changeFold;
            private bool fold;
            private bool isChecked;
            
            public PropertyToggleFoldGroupScope(string title, FoldToggleParameter foldToggleParameter)
            {
                BeginPropertyToggleFoldGroup(title, foldToggleParameter);
            }

            public PropertyToggleFoldGroupScope(string title, bool fold, bool isActive)
            {
                BeginPropertyToggleFoldGroup(title, new FoldToggleParameter(fold, isActive));
            }

            public bool Fold
            {
                get
                {
                    if (this.changeFold) return this.fold;
                    
                    this.changeFold = true;
                    var param = EndPropertyToggleFoldGroup();
                    this.fold = param.Fold;
                    this.isChecked = param.IsChecked;
                    return this.fold;
                }
            }

            public bool IsChecked => this.isChecked;

            protected override void CloseScope()
            {
                if (this.changeFold) return;
                EndPropertyToggleFoldGroup();
            }
        }

        public class ObjectFieldFoldGroupScope<T> : GUI.Scope where T : Object
        {
            private bool changeFold;
            private bool fold;
            private T content;
            
            public ObjectFieldFoldGroupScope(string title, T content, bool fold, bool allowSceneObject, bool showRemoveButton, Action<T> onValueChanged, Func<bool> validate)
            {
                BeginObjectFieldFoldGroup(title, new ObjectFieldToggleParameter(content, fold), allowSceneObject, showRemoveButton, onValueChanged, validate);
            }
            
            public ObjectFieldFoldGroupScope(string title, ObjectFieldToggleParameter parameter, bool allowSceneObject, bool showRemoveButton, Action<T> onValueChanged, Func<bool> validate)
            {
                BeginObjectFieldFoldGroup(title, parameter, allowSceneObject, showRemoveButton, onValueChanged, validate);
            }
            
            public bool Fold
            {
                get
                {
                    if (this.changeFold) return this.fold;
                    
                    this.changeFold = true;
                    var param = EndObjectFieldFoldGroup();
                    this.fold = param.Fold;
                    this.content = (T)param.Content;
                    return this.fold;
                }
            }

            public T Content => this.content;
            
            protected override void CloseScope()
            {
                if (this.changeFold) return;
                EndObjectFieldFoldGroup();
            }
        }

        private static class Styles
        {
            /// <summary>
            /// オプションアイコンの黒い版
            /// </summary>
            private static readonly Texture2D PaneOptionsIconDark;

            /// <summary>
            /// オプションアイコンの白い版
            /// </summary>
            private static readonly Texture2D PaneOptionsIconLight;

            /// <summary>
            /// 黒ヘッダー
            /// </summary>
            private static readonly Color HeaderBackgroundDarkColor;

            /// <summary>
            /// 白ヘッダー
            /// </summary>
            private static readonly Color HeaderBackgroundLightColor;
            
            static Styles()
            {
                SmallTickBox = new GUIStyle("ShurikenToggle");

                PaneOptionsIconDark = (Texture2D) EditorGUIUtility.Load("Builtin Skins/DarkSkin/Images/pane options.png");
                PaneOptionsIconLight = (Texture2D) EditorGUIUtility.Load("Builtin Skins/LightSkin/Images/pane options.png");

                HeaderBackgroundDarkColor = new Color(0.1f, 0.1f, 0.1f, 0.2f);
                HeaderBackgroundLightColor = new Color(1f, 1f, 1f, 0.2f);
            }

            /// <summary>
            /// チェックボックスのスタイル。
            /// </summary>
            public static GUIStyle SmallTickBox { get; }

            public static Color HeaderBackgroundColor =>
                EditorGUIUtility.isProSkin ? HeaderBackgroundDarkColor : HeaderBackgroundLightColor;
        }
    }
}