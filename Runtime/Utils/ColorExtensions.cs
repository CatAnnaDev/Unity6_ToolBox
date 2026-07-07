using UnityEngine;

namespace CatAnnaDev.Utils
{
    public static class ColorExtensions
    {
        public static Color WithAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        public static Color WithRed(this Color color, float r)
        {
            color.r = r;
            return color;
        }

        public static Color WithGreen(this Color color, float g)
        {
            color.g = g;
            return color;
        }

        public static Color WithBlue(this Color color, float b)
        {
            color.b = b;
            return color;
        }

        public static string ToHex(this Color color, bool includeAlpha = false)
        {
            Color32 c = color;
            if (includeAlpha)
            {
                return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", c.r, c.g, c.b, c.a);
            }
            return string.Format("#{0:X2}{1:X2}{2:X2}", c.r, c.g, c.b);
        }

        public static Color FromHex(string hex)
        {
            if (string.IsNullOrEmpty(hex))
            {
                return Color.white;
            }
            if (ColorUtility.TryParseHtmlString(hex[0] == '#' ? hex : string.Concat("#", hex), out Color result))
            {
                return result;
            }
            return Color.magenta;
        }

        public static Color Lighten(this Color color, float amount)
        {
            return Color.Lerp(color, Color.white, Mathf.Clamp01(amount));
        }

        public static Color Darken(this Color color, float amount)
        {
            return Color.Lerp(color, Color.black, Mathf.Clamp01(amount));
        }

        public static Color Invert(this Color color)
        {
            return new Color(1f - color.r, 1f - color.g, 1f - color.b, color.a);
        }

        public static float Luminance(this Color color)
        {
            return 0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b;
        }

        public static Color Grayscale(this Color color)
        {
            float l = color.Luminance();
            return new Color(l, l, l, color.a);
        }

        public static Color WithSaturation(this Color color, float saturation)
        {
            Color.RGBToHSV(color, out float h, out _, out float v);
            Color result = Color.HSVToRGB(h, Mathf.Clamp01(saturation), v);
            result.a = color.a;
            return result;
        }

        public static Color WithValue(this Color color, float value)
        {
            Color.RGBToHSV(color, out float h, out float s, out _);
            Color result = Color.HSVToRGB(h, s, Mathf.Clamp01(value));
            result.a = color.a;
            return result;
        }
    }
}
