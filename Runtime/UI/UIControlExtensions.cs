using System;
using UnityEngine.UI;

namespace CatAnnaDev.UI
{
    public static class UIControlExtensions
    {
        public static Button OnClick(this Button button, Action callback)
        {
            if (callback != null) button.onClick.AddListener(() => callback());
            return button;
        }

        public static Toggle OnToggled(this Toggle toggle, Action<bool> callback)
        {
            if (callback != null) toggle.onValueChanged.AddListener(value => callback(value));
            return toggle;
        }

        public static Slider OnChanged(this Slider slider, Action<float> callback)
        {
            if (callback != null) slider.onValueChanged.AddListener(value => callback(value));
            return slider;
        }

        public static Scrollbar OnChanged(this Scrollbar scrollbar, Action<float> callback)
        {
            if (callback != null) scrollbar.onValueChanged.AddListener(value => callback(value));
            return scrollbar;
        }

        public static Dropdown OnChanged(this Dropdown dropdown, Action<int> callback)
        {
            if (callback != null) dropdown.onValueChanged.AddListener(value => callback(value));
            return dropdown;
        }

        public static InputField OnChanged(this InputField input, Action<string> callback)
        {
            if (callback != null) input.onValueChanged.AddListener(value => callback(value));
            return input;
        }

        public static InputField OnSubmit(this InputField input, Action<string> callback)
        {
            if (callback != null) input.onEndEdit.AddListener(value => callback(value));
            return input;
        }

        public static ScrollRect OnScrolled(this ScrollRect scrollRect, Action<UnityEngine.Vector2> callback)
        {
            if (callback != null) scrollRect.onValueChanged.AddListener(value => callback(value));
            return scrollRect;
        }
    }
}
