using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Labb3___GUI.Model
{
    internal class Question
    {

        public Question()
        {
            Query = "Default Question Text";
            CorrectAnswer = "Correct Answer";
            IncorrectAnswer1 = "Incorrect Answer 1";
            IncorrectAnswer2 = "Incorrect Answer 2";
            IncorrectAnswer3 = "Incorrect Answer 3";
        }

        public Question(string query, string correctAnswer, string incorrectAnswer1, string incorrectAnswer2, string incorrectAnswer3)
        {
            Query = query;
            CorrectAnswer = correctAnswer;
            IncorrectAnswer1 = incorrectAnswer1;
            IncorrectAnswer2 = incorrectAnswer2;
            IncorrectAnswer3 = incorrectAnswer3;
        }

     //  [BsonId] // Mark as identifier for MongoDB if required
     //   public ObjectId Id { get; set; }

       [BsonElement("Query")]
        public string Query { get; set; }

         [BsonElement("CorrectAnswer")]
        public string CorrectAnswer { get; set; }

          [BsonElement("IncorrectAnswer1")]
        public string IncorrectAnswer1 { get; set; }

           [BsonElement("IncorrectAnswer2")]
        public string IncorrectAnswer2 { get; set; }

          [BsonElement("IncorrectAnswer3")]
        public string IncorrectAnswer3 { get; set; }

        public List<string> GetAnswerOptions()
        {
            var options = new List<string>
        {
            CorrectAnswer,
            IncorrectAnswer1,
            IncorrectAnswer2,
            IncorrectAnswer3
        };

            return options.OrderBy(x => Guid.NewGuid()).ToList(); // Randomize order
        }
        
        public ObservableCollection<string> AnswerOptions { get; }  //ta bort alla AnswerOptions?
        public ObservableCollection<Question> Questions { get; } = new ObservableCollection<Question>();

    }
}
