using JapaneseLearningApp.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapaneseLearningApp.Models
{
    public class HiraganaCharacter
    {
        public int Id { get; set; }
        public string Character { get; set; } = string.Empty;
        public CharacterGroup Group { get; set; }
        public string Row { get; set; } = string.Empty;
        public string StrokeOrder { get; set; } = string.Empty;
        public string AudioFile { get; set; } = string.Empty;
    }
}
