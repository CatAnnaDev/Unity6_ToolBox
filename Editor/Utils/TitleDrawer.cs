using CatAnnaDev.Utils;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    [CustomPropertyDrawer(typeof(TitleAttribute))]
    internal sealed class TitleDrawer : DecoratorDrawer
    {
        private const float TopPadding = 6f;
        private const float BottomPadding = 2f;
        private const float SeparatorHeight = 1f;

        private static GUIStyle titleStyle;

        private static GUIStyle TitleStyle
        {
            get
            {
                if (titleStyle == null)
                {
                    titleStyle = new GUIStyle(EditorStyles.boldLabel)
                    {
                        fontSize = 12
                    };
                }

                return titleStyle;
            }
        }

        public override float GetHeight()
        {
            TitleAttribute title = (TitleAttribute)attribute;
            float height = TopPadding + EditorGUIUtility.singleLineHeight + BottomPadding;
            if (title.DrawSeparator)
            {
                height += SeparatorHeight + BottomPadding;
            }

            return height;
        }

        public override void OnGUI(Rect position)
        {
            TitleAttribute title = (TitleAttribute)attribute;

            Rect labelRect = new Rect(
                position.x,
                position.y + TopPadding,
                position.width,
                EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, title.Text, TitleStyle);

            if (title.DrawSeparator)
            {
                Rect separatorRect = new Rect(
                    position.x,
                    labelRect.yMax + BottomPadding,
                    position.width,
                    SeparatorHeight);
                CatAnnaDevEditorGUI.DrawSeparator(separatorRect);
            }
        }
    }
}
