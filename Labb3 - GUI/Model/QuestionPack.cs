using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
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

        public QuestionPack(string name, Difficulty difficulty = Difficulty.Medium, int timeLimitInSeconds = 30, List<Question> questions = null)
        {
            Name = name;
            Difficulty = difficulty;
            TimeLimitInSeconds = timeLimitInSeconds;
            Questions = questions ?? new List<Question>();
        }
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Difficulty")]
        public Difficulty Difficulty { get; set; }

        [BsonElement("TimeLimitInSeconds")]
        public int TimeLimitInSeconds { get; set; }

        [BsonElement("Questions")]
        public List<Question> Questions { get; set; }
    }
}