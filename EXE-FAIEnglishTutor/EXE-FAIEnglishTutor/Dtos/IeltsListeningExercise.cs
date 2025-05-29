using Microsoft.AspNetCore.Mvc;

namespace EXE_FAIEnglishTutor.Dtos
{
    public class IeltsListeningExercise
    {
        public string Script { get; set; }
        public List<IeltsQuestion> Questions { get; set; }
    }
}
