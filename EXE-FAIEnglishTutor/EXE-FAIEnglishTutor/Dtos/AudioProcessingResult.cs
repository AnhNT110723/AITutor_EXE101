namespace EXE_FAIEnglishTutor.Dtos
{
    public class AudioProcessingResult
    {
        public string TranscribedText { get; set; }
        public double PronunciationScore { get; set; }
        public double StressScore { get; set; }
        public double SyllableScore { get; set; }
    }
}
