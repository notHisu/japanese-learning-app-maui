using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapaneseLearningApp.Models
{
    public class CharacterProgress
    {
        public int Id { get; set; }
        public int CharacterId { get; set; }
        public int TimesStudied { get; set; }
        public int TimesCorrect { get; set; }
        public DateTime LastStudied { get; set; }
        public bool Mastered { get; set; }
        public float Difficulty { get; set; } // 0.0 to 1.0 scale
    }
}
