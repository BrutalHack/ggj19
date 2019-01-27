using JetBrains.Annotations;
using UnityEngine;

namespace BrutalHack.ggj19.General
{
    public class HumanRandomizer : MonoBehaviour
    {
        public Sprite[] HairSprites;
        public Sprite[] HeadSprites;
        public Sprite[] BodySprites;

        public Color[] HairColors;
        public Color[] HeadColors;
        public Color[] BodyColors;

        public SpriteRenderer Hair;
        public SpriteRenderer Head;
        public SpriteRenderer Body;

        private bool isColored;

        private void Start()
        {
            RandomizeSprite(Hair, HairSprites);
            RandomizeSprite(Head, HeadSprites);
            RandomizeSprite(Body, BodySprites);
        }

        [UsedImplicitly]
        public void RandomizeColors()
        {
            if (isColored)
            {
                return;
            }

            RandomizeColor(Hair, HairColors);
            RandomizeColor(Head, HeadColors);
            RandomizeColor(Body, BodyColors);
        }

        private void RandomizeSprite(SpriteRenderer spriteRenderer, Sprite[] sprites)
        {
            int spriteIndex = Random.Range(0, sprites.Length - 1);
            spriteRenderer.sprite = sprites[spriteIndex];
        }

        private void RandomizeColor(SpriteRenderer spriteRenderer, Color[] colors)
        {
            int colorIndex = Random.Range(0, colors.Length - 1);
            spriteRenderer.color = colors[colorIndex];
        }
    }
}