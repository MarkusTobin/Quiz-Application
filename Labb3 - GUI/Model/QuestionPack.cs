using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3___GUI.Model
{
    enum Difficulty { Easy, Medium, Hard };
    internal class QuestionPack
    {
        public const int EasyIndex = 0;
        public const int MediumIndex = 1;
        public const int HardIndex = 2;

        // This property is what you will bind to the ComboBox
    /*    public int DifficultyIndex
        {
            get => (int)Difficulty;  // Get the index from the enum
            set => Difficulty = (Difficulty)value;  // Set the enum based on the index
        }*/

        public QuestionPack() { }
        public QuestionPack(string name, Difficulty difficulty = Difficulty.Medium, int timeLimitInSeconds = 30)
        {
            Name = name;
            Difficulty = difficulty;
            TimeLimitInSeconds = timeLimitInSeconds;
            Questions = new List<Question>();
        }


        public string Name { get; set; }
        public Difficulty Difficulty { get; set; }
        public int TimeLimitInSeconds { get; set; }
        public List<Question> Questions { get; set; }
    }
}