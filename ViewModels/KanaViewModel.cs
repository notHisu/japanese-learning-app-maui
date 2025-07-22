// Language: C#
using System.Collections.ObjectModel;
using JapaneseLearningApp.Models;

namespace JapaneseLearningApp.ViewModels;

public class KanaViewModel
{
    public ObservableCollection<KanaItem> HiraganaList { get; set; }
    public ObservableCollection<KanaItem> KatakanaList { get; set; }

    public KanaViewModel()
    {
        HiraganaList = new ObservableCollection<KanaItem>
        {
            new KanaItem { Character = "あ", Romaji = "a" },
            new KanaItem { Character = "い", Romaji = "i" },
            new KanaItem { Character = "う", Romaji = "u" }
        };

        KatakanaList = new ObservableCollection<KanaItem>
        {
            new KanaItem { Character = "ア", Romaji = "a" },
            new KanaItem { Character = "イ", Romaji = "i" },
            new KanaItem { Character = "ウ", Romaji = "u" }
        };
    }
}
