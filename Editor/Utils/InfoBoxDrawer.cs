using CatAnnaDev.Utils;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    [CustomPropertyDrawer(typeof(InfoBoxAttribute))]
    internal sealed class InfoBoxDrawer : DecoratorDrawer
    {
        private const float BottomPadding = 2f;

        public override float GetHeight()
        {
            InfoBoxAttribute infoBox = (InfoBoxAttribute)attribute;
            float width = EditorGUIUtility.currentViewWidth - 40f;
            return CatAnnaDevEditorGUI.GetHelpBoxHeight(infoBox.Text, width) + BottomPadding;
        }

        public override void OnGUI(Rect position)
        {
            InfoBoxAttribute infoBox = (InfoBoxAttribute)attribute;
            Rect boxRect = new Rect(
                position.x,
                position.y,
                position.width,
                position.height - BottomPadding);
            CatAnnaDevEditorGUI.DrawHelpBox(boxRect, infoBox.Text, ToMessageType(infoBox.Type));
        }

        private static MessageType ToMessageType(InfoBoxType type)
        {
            switch (type)
            {
                case InfoBoxType.Info:
                    return MessageType.Info;
                case InfoBoxType.Warning:
                    return MessageType.Warning;
                case InfoBoxType.Error:
                    return MessageType.Error;
                default:
                    return MessageType.None;
            }
        }
    }
}
