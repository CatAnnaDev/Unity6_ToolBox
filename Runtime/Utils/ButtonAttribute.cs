using System;

namespace CatAnnaDev.Utils
{
    public enum ButtonActivity
    {
        Always,
        EditorOnly,
        PlayModeOnly
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class ButtonAttribute : Attribute
    {
        public readonly string Label;
        public readonly ButtonActivity Activity;

        public ButtonAttribute()
        {
            Label = null;
            Activity = ButtonActivity.Always;
        }

        public ButtonAttribute(string label)
        {
            Label = label;
            Activity = ButtonActivity.Always;
        }

        public ButtonAttribute(string label, ButtonActivity activity)
        {
            Label = label;
            Activity = activity;
        }
    }
}
