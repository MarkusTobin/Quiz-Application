using System.Text.Json.Serialization;

namespace Labb3___GUI.Model
{
    enum Difficulty { Easy, Medium, Hard };
    internal class QuestionPack
    {
        public const int EasyIndex = 0;
        public const int MediumIndex = 1;
        public const int HardIndex = 2;

        public QuestionPack() { }

        [JsonConstructor]
        public QuestionPack(string name, Difficulty difficulty = Difficulty.Medium, int timeLimitInSeconds = 30, List<Question> questions = null)
        {
            Name = name;
            Difficulty = difficulty;
            TimeLimitInSeconds = timeLimitInSeconds;
            Questions = questions ?? new List<Question>();
        }


        public string Name { get; set; }
        public Difficulty Difficulty { get; set; }
        public int TimeLimitInSeconds { get; set; }
        public List<Question> Questions { get; set; }
    }
}