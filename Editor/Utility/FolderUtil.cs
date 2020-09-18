using System.Reflection;
using UnityEditor;

namespace BitActionSwitch.Editor.Utility
{
    public static class FolderUtil
    {
        public static string GetSaveFolderPath(string title)
        {
            var savePath = EditorUtility.OpenFolderPanel(title, GetCurrentDirectory(), "");
           return FileUtil.GetProjectRelativePath(savePath);
        }

        public static string GetCurrentDirectory()
        {
            const BindingFlags flag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
            var asm = Assembly.Load("UnityEditor.dll");
            var typeProjectBrowser = asm.GetType("UnityEditor.ProjectBrowser");
            var projectBrowserWindow = EditorWindow.GetWindow(typeProjectBrowser);
            return (string)typeProjectBrowser.GetMethod("GetActiveFolderPath", flag)?.Invoke(projectBrowserWindow, null); 
        }
    }
}