using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Services.Implementaion.AI;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO; // Explicitly include System.IO
using System.Text.Json;
using System.Threading.Tasks;
namespace EXE_FAIEnglishTutor.Controllers
{
    [Route("api/audio")]
    [ApiController]
    public class AudioController : ControllerBase
    {
        private readonly SpeechService _speechService;

        public AudioController(SpeechService speechService)
        {
            _speechService = speechService;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessAudio([FromForm] IFormFile audio, [FromForm] string standardSentence)
        {
            if (audio == null || string.IsNullOrEmpty(standardSentence))
                return BadRequest("Audio file or standard sentence missing.");

            var tempPath = Path.Combine(Path.GetTempPath(), audio.FileName);
            try
            {
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    await audio.CopyToAsync(stream);
                }

                // Transcribe audio using OpenAI Whisper API
                var transcribedText = await _speechService.TranscribeAudioAsync(tempPath, "en"); // Change to "vi" for Vietnamese

                // Compare with standard sentence
                var pronunciationScore = _speechService.CalculateLevenshteinScore(transcribedText.ToLower(), standardSentence.ToLower());
                var sequenceScore = _speechService.CalculateSequenceMatchScore(transcribedText.ToLower(), standardSentence.ToLower());

                // Syllable scoring
                var standardSyllables = _speechService.CountSyllables(standardSentence);
                var transcribedSyllables = _speechService.CountSyllables(transcribedText);
                var syllableScore = 100 * (1 - Math.Abs(standardSyllables - transcribedSyllables) / (double)Math.Max(standardSyllables, 1));

                // Stress score (placeholder: based on sequence score)
                var stressScore = sequenceScore * 0.9;

                var result = new AudioProcessingResult
                {
                    TranscribedText = transcribedText,
                    PronunciationScore = Math.Round(pronunciationScore, 2),
                    StressScore = Math.Round(stressScore, 2),
                    SyllableScore = Math.Round(syllableScore, 2)
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing audio: {ex.Message}");
            }
            finally
            {
                if (System.IO.File.Exists(tempPath)) // Use fully qualified System.IO.File
                {
                    try
                    {
                        System.IO.File.Delete(tempPath);
                    }
                    catch (IOException)
                    {
                        // Log the error if deletion fails (optional)
                    }
                }
            }
        }

        [HttpPost("generate-english")]
        public async Task<IActionResult> GenerateEnglishSpeech([FromForm] string text)
        {
            if (string.IsNullOrEmpty(text))
                return BadRequest("Text input is missing.");

            try
            {
                // Tạo tên file tạm với GUID duy nhất
                var tempFileName = $"{Guid.NewGuid()}.mp3";
                var tempPath = Path.Combine(Path.GetTempPath(), tempFileName);

                // Gọi dịch vụ để tạo âm thanh
                await _speechService.TextToSpeechAsync(text, tempPath, "en");

                // Đọc file âm thanh bằng FileStream
                byte[] audioBytes;
                using (var fileStream = new FileStream(tempPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    audioBytes = new byte[fileStream.Length];
                    await fileStream.ReadAsync(audioBytes, 0, (int)fileStream.Length);
                }

                // Xóa file tạm
                try
                {
                    if (System.IO.File.Exists(tempPath))
                    {
                        System.IO.File.Delete(tempPath);
                    }
                }
                catch (IOException ex)
                {
                    // Log lỗi chi tiết để debug
                    Console.WriteLine($"Lỗi khi xóa file tạm: {ex.Message}");
                    return StatusCode(500, $"Lỗi khi xóa file tạm: {ex.Message}");
                }

                // Trả về file âm thanh
                return File(audioBytes, "audio/mpeg", tempFileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating speech: {ex.Message}");
            }
        }
        [HttpPost("compare-audio")]
        public async Task<IActionResult> CompareAudio([FromForm] IFormFile userAudio, [FromForm] IFormFile referenceAudio)
        {
            if (userAudio == null || referenceAudio == null)
                return BadRequest("Both audio files are required.");

            var userAudioPath = Path.Combine(Path.GetTempPath(), "user_" + userAudio.FileName);
            var refAudioPath = Path.Combine(Path.GetTempPath(), "ref_" + referenceAudio.FileName);

            try
            {
                // Save both audio files
                using (var stream = new FileStream(userAudioPath, FileMode.Create))
                {
                    await userAudio.CopyToAsync(stream);
                }
                using (var stream = new FileStream(refAudioPath, FileMode.Create))
                {
                    await referenceAudio.CopyToAsync(stream);
                }

                // Transcribe both using Whisper API
                var userText = await _speechService.TranscribeAudioAsync(userAudioPath, "en");
                var refText = await _speechService.TranscribeAudioAsync(refAudioPath, "en");

                // Calculate text-based scores
                var pronunciationScore = _speechService.CalculateLevenshteinScore(userText.ToLower(), refText.ToLower());
                var sequenceScore = _speechService.CalculateSequenceMatchScore(userText.ToLower(), refText.ToLower());

                // Syllable comparison
                var userSyllables = _speechService.CountSyllables(userText);
                var refSyllables = _speechService.CountSyllables(refText);
                var syllableScore = 100 * (1 - Math.Abs(userSyllables - refSyllables) / (double)Math.Max(refSyllables, 1));

                // Stress score placeholder
                var stressScore = sequenceScore * 0.9;

                var result = new AudioProcessingResult
                {
                    TranscribedText = userText,
                    ReferenceText = refText,
                    PronunciationScore = Math.Round(pronunciationScore, 2),
                    StressScore = Math.Round(stressScore, 2),
                    SyllableScore = Math.Round(syllableScore, 2)
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing audio: {ex.Message}");
            }
            finally
            {
                if (System.IO.File.Exists(userAudioPath)) System.IO.File.Delete(userAudioPath);
                if (System.IO.File.Exists(refAudioPath)) System.IO.File.Delete(refAudioPath);
            }
        }


        [HttpPost("generate-ielts-listening")]
public async Task<IActionResult> GenerateIeltsListening([FromBody] string topic)
{
    try
    {
        // Generate IELTS listening exercise based on topic
        var listeningExercise = await _speechService.GenerateIeltsListeningExerciseAsync(topic);

        // Return JSON data
        return Ok(listeningExercise);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "Error generating IELTS listening exercise", error = ex.Message });
    }
}
        public class AudioProcessingResult
        {
            public string TranscribedText { get; set; }
            public string ReferenceText { get; set; }
            public double PronunciationScore { get; set; } // 0–100%
            public double StressScore { get; set; }
            public double SyllableScore { get; set; }
        }

        [HttpPost("generate-random-words")]
        public async Task<IActionResult> GenerateRandomWords([FromForm] string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
                return BadRequest("Topic is missing.");

            try
            {
                var wordResults = await _speechService.GetRandomWordsAsync(topic.ToLower());
                if (wordResults == null || !wordResults.Any())
                    return BadRequest("No words found for the specified topic.");

                return Ok(wordResults.Select(w => new
                {
                    word = w.Word,
                    meaning = w.Meaning,
                    phonetic = w.Phonetic,
                    vietnameseMeaning = w.VietnameseMeaning
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Error generating random words.",
                    detail = ex.Message
                });
            }
        }
        [HttpPost("generate-random-word")]
        public async Task<IActionResult> GenerateRandomWord([FromForm] string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
                return BadRequest("Topic is missing.");

            try
            {
                var wordResults = await _speechService.GetRandomWordAsync(topic.ToLower());
                if (wordResults == null)
                    return BadRequest("No words found for the specified topic.");

                return Ok(wordResults);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Error generating random words.",
                    detail = ex.Message
                });
            }
        }

    }
}
