using UnityEngine;
using CatAnnaDev.Utils;

namespace CatAnnaDev.Samples
{
    public sealed class AttributesShowcase : MonoBehaviour
    {
        [Title("Identity")]
        [InfoBox("Select this component in the Inspector to see the custom drawers. Nothing runs at Play time; this is a tour of the property attributes.")]
        [SerializeField]
        private string displayName = "Hero";

        [ReadOnly]
        [SerializeField]
        private string generatedId = "npc_0001";

        [Required("Assign a spawn point before shipping")]
        [SerializeField]
        private Transform spawnPoint;

        [Title("Stats")]
        [ProgressBar(100f, "Health")]
        [SerializeField]
        private float health = 72f;

        [ProgressBar("maxMana")]
        [SerializeField]
        private float mana = 30f;

        [SerializeField]
        private float maxMana = 50f;

        [MinMaxSlider(0f, 100f)]
        [SerializeField]
        private Vector2 damageRange = new Vector2(15f, 40f);

        [Title("Conditional")]
        [SerializeField]
        private bool usesShield = true;

        [ShowIf(nameof(usesShield))]
        [SerializeField]
        private float shieldStrength = 25f;

        [ShowIf(nameof(usesShield), true)]
        [InfoBox("This warning only appears when Uses Shield is OFF.", InfoBoxType.Warning)]
        [SerializeField]
        private string noShieldReason = "Glass cannon build";

        private GUIStyle boxStyle;
        private bool styleReady;

        [Button("Randomize Stats")]
        private void RandomizeStats()
        {
            health = RandomUtils.RandomColor().Luminance() * 100f;
            mana = Random.Range(0f, maxMana);
            damageRange = new Vector2(Random.Range(0f, 50f), Random.Range(50f, 100f));
            Debug.Log("AttributesShowcase: randomized stats. health=" + health.ToString("F1"));
        }

        [Button("Full Heal", ButtonActivity.PlayModeOnly)]
        private void FullHeal()
        {
            health = 100f;
            mana = maxMana;
            Debug.Log("AttributesShowcase: full heal.");
        }

        private void OnGUI()
        {
            EnsureStyle();

            GUILayout.BeginArea(new Rect(10f, 10f, 520f, 220f), boxStyle);
            GUILayout.Label("Attributes Showcase");
            GUILayout.Label(
                "This component demonstrates the pack's inspector attributes, not runtime\n" +
                "code. Select the GameObject holding it and look at the Inspector to see:\n" +
                "  [Title]        section headers\n" +
                "  [InfoBox]      help / warning boxes\n" +
                "  [ReadOnly]     a locked, non-editable field\n" +
                "  [Required]     a highlighted missing reference (Spawn Point)\n" +
                "  [ProgressBar]  Health and Mana bars\n" +
                "  [MinMaxSlider] a two-handle Damage Range slider\n" +
                "  [ShowIf]       Shield fields toggled by the Uses Shield checkbox\n" +
                "  [Button]       Randomize Stats / Full Heal method buttons");
            GUILayout.EndArea();
        }

        private void EnsureStyle()
        {
            if (styleReady)
            {
                return;
            }
            boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.padding = new RectOffset(12, 12, 12, 12);
            boxStyle.alignment = TextAnchor.UpperLeft;
            styleReady = true;
        }
    }
}
