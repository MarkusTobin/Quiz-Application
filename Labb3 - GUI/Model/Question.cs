using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3___GUI.Model
{
    internal class Question
    {
        public ObservableCollection<Question> Questions { get; } = new ObservableCollection<Question>();
        public Question(string query, string correctAnswer, string incorrectAnswer1, string incorrectAnswer2, string incorrectAnswer3)
        {
            Query = query;
            CorrectAnswer = correctAnswer;
            IncorrectAnswer1 = incorrectAnswer1;
            IncorrectAnswer2 = incorrectAnswer2;
            IncorrectAnswer3 = incorrectAnswer3;
        }

        public string Query { get; set; }
        public string CorrectAnswer { get; set; }
        public string IncorrectAnswer1 { get; set; }
        public string IncorrectAnswer2 { get; set; }
        public string IncorrectAnswer3 { get; set; }
    }
}
