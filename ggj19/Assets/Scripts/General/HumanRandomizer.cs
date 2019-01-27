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

        private void Start()
        {
            RandomizeSprite(Hair, HairSprites, HairColors);
            RandomizeSprite(Head, HeadSprites, HeadColors);
            RandomizeSprite(Body, BodySprites, BodyColors);
        }

        private void RandomizeSprite(SpriteRenderer spriteRenderer, Sprite[] sprites, Color[] colors)
        {
            int spriteIndex = Random.Range(0, sprites.Length - 1);
            int colorIndex = Random.Range(0, colors.Length - 1);

            spriteRenderer.sprite = sprites[spriteIndex];
            spriteRenderer.color = colors[colorIndex];
        }
    }
}