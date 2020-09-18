#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace BitActionSwitch.Editor.Readme
{
	[CustomEditor(typeof(Readme))]
	public class ReadmeEditor : UnityEditor.Editor
	{

		private const float SPACE = 16f;

		[MenuItem("Tutorial/Show Tutorial Instructions")]
		private static Readme SelectReadme() 
		{
			var ids = AssetDatabase.FindAssets("Readme t:Readme");
			if (ids.Length == 1)
			{
				var readmeObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(ids[0]));
			
				Selection.objects = new[]{readmeObject};
			
				return (Readme)readmeObject;
			}

			Debug.Log("Couldn't find a readme");
			return null;
		}
	
		protected override void OnHeaderGUI()
		{
			var readme = (Readme)this.target;
			this.Init();
		
			var iconWidth = Mathf.Min(EditorGUIUtility.currentViewWidth/3f - 20f, 128f);
		
			GUILayout.BeginHorizontal("In BigTitle");
			{
				GUILayout.Label(readme.icon, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
				GUILayout.Label(readme.title, this.TitleStyle);
			}
			GUILayout.EndHorizontal();
		}
	
		public override void OnInspectorGUI()
		{
			var readme = (Readme)this.target;
			this.Init();
		
			foreach (var section in readme.sections)
			{
				if (!string.IsNullOrEmpty(section.heading))
				{
					GUILayout.Label(section.heading, this.HeadingStyle);
				}
				if (!string.IsNullOrEmpty(section.text))
				{
					foreach (var s in section.text.Split(new[] {"<\\br>"}, StringSplitOptions.None))
					{
						GUILayout.Label(s, this.BodyStyle);
					}
				}
				if (!string.IsNullOrEmpty(section.linkText))
				{
					if (this.LinkLabel(new GUIContent(section.linkText)))
					{
						Application.OpenURL(section.url);
					}
				}
				GUILayout.Space(SPACE);
			}
		}


		private bool initialized;

		private GUIStyle LinkStyle => this.m_LinkStyle;
		[SerializeField] GUIStyle m_LinkStyle;

		private GUIStyle TitleStyle => this.m_TitleStyle;
		[SerializeField] GUIStyle m_TitleStyle;

		private GUIStyle HeadingStyle => this.m_HeadingStyle;
		[SerializeField] GUIStyle m_HeadingStyle;

		private GUIStyle BodyStyle => this.m_BodyStyle;
		[SerializeField] GUIStyle m_BodyStyle;

		private void Init()
		{
			if (this.initialized)
				return;
			this.m_BodyStyle = new GUIStyle(EditorStyles.label) {wordWrap = true, fontSize = 14};

			this.m_TitleStyle = new GUIStyle(this.m_BodyStyle) {fontSize = 26};

			this.m_HeadingStyle = new GUIStyle(this.m_BodyStyle) {fontSize = 18};

			this.m_LinkStyle = new GUIStyle(this.m_BodyStyle)
			{
				wordWrap = false,
				normal = {textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f)},
				stretchWidth = false
			};
			this.initialized = true;
		}

		private bool LinkLabel (GUIContent label, params GUILayoutOption[] options)
		{
			var position = GUILayoutUtility.GetRect(label, this.LinkStyle, options);

			Handles.BeginGUI ();
			Handles.color = this.LinkStyle.normal.textColor;
			Handles.DrawLine (new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
			Handles.color = Color.white;
			Handles.EndGUI ();

			EditorGUIUtility.AddCursorRect (position, MouseCursor.Link);

			return GUI.Button (position, label, this.LinkStyle);
		}
	}
}
#endif
