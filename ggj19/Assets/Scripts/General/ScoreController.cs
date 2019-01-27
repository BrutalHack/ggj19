namespace BrutalHack.ggj19.General
{
    public class ScoreController
    {
        private static ScoreController instance;

        public int Score { get; set; }

        public static ScoreController Instance => instance ?? (instance = new ScoreController());
    }
}