#if VRC_SDK_VRCSDK3 && UNITY_EDITOR
using BitActionSwitch.Editor.Layout;
using BitActionSwitch.Scripts;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BitActionSwitch.Editor.Views
{
    public class GameObjectRegisterUi
    {
        private readonly ReorderableList reorderableList;
        private readonly BitActionSwitchItem bitActionSwitchItem;

        private GameObject targetAvatar;

        public GameObjectRegisterUi(BitActionSwitchItem bitActionSwitchItem)
        {
            this.bitActionSwitchItem = bitActionSwitchItem;
            
            this.reorderableList = new ReorderableList(bitActionSwitchItem.gameObjects, typeof(GameObject), true, false,
                true, true);

            this.reorderableList.drawHeaderCallback += rect =>
            {
                EditorGUI.LabelField(rect, "Target Game Objects");
            };

            this.reorderableList.drawElementCallback += (rect, index, active, focused) =>
            {
                var toggleFieldRect = rect;
                toggleFieldRect.width = 16f;
                toggleFieldRect.height = EditorGUIUtility.singleLineHeight + 2;
                
                var objectFieldRect = rect;
                objectFieldRect.x += toggleFieldRect.width + 2;
                objectFieldRect.y += 1;
                objectFieldRect.width -= toggleFieldRect.width + 2;
                objectFieldRect.height = EditorGUIUtility.singleLineHeight;

                if (bitActionSwitchItem.gameObjects[index] != null)
                {
                    EditorCustomGUI.Toggle(toggleFieldRect, "", bitActionSwitchItem.gameObjects[index].activeSelf,
                        x => bitActionSwitchItem.gameObjects[index].SetActive(x));
                }

                EditorCustomGUI.ObjectField(objectFieldRect, "", bitActionSwitchItem.gameObjects[index], true, true,x =>
                {
                    bitActionSwitchItem.gameObjects[index] = x;
                }, () =>
                {
                    if (bitActionSwitchItem.gameObjects[index] == null) return false;

                    if (this.targetAvatar == null) return false;

                    return bitActionSwitchItem
                        .gameObjects[index].transform
                        .IsChildOf(this.targetAvatar.transform);
                });
            };

            this.reorderableList.onAddCallback += list => bitActionSwitchItem.gameObjects.Add(null);
        }
        
        public void OnGUI(Rect position, GameObject targetAvatar)
        {
            this.targetAvatar = targetAvatar;
            this.reorderableList.DoList(position);
            this.reorderableList.displayRemove = this.bitActionSwitchItem.gameObjects.Count > 1;
        }

        public float GetElementHeight()
        {
            return this.reorderableList.GetHeight() + 24;
        }
    }
}
#endif