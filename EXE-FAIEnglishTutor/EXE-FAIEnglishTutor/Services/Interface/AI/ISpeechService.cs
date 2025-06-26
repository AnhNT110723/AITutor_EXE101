using Microsoft.AspNetCore.Cors.Infrastructure;
using System.Text.Json.Serialization;

namespace EXE_FAIEnglishTutor.Services.Interface.AI
{
    public interface ISpeechService
    {
        Task<string> TranscribeAudioAsync(string audioPath, string language = "en");
        double CalculateLevenshteinScore(string source, string target);
        double CalculateSequenceMatchScore(string source, string target);
        int CountSyllables(string text);
        Task TextToSpeechAsync(string text, string outputPath, string language);
        Task<List<WordResult>> GetRandomWordsAsync(string topic);
        Task<string> GeneratePostcardAsync(string prompt);
    }
    public class WordResult
    {
        public string Word { get; set; }
        public string Meaning { get; set; }
        public string Phonetic { get; set; }
        [JsonPropertyName("vietnameseMeaning")]
        public string VietnameseMeaning { get; set; }
    }

}
