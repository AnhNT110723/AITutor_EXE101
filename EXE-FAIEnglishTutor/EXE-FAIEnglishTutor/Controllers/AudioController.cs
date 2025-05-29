using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Services.Implementaion.AI;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO; // Explicitly include System.IO
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
                // Generate a unique temporary file path for the audio
                var tempFileName = $"{Guid.NewGuid()}.mp3";
                var tempPath = Path.Combine(Path.GetTempPath(), tempFileName);

                // Call the speech service to generate audio for English
                await _speechService.TextToSpeechAsync(text, tempPath, "en");

                // Read the generated audio file
                var audioBytes = await System.IO.File.ReadAllBytesAsync(tempPath);

                // Clean up the temporary file
                try
                {
                    if (System.IO.File.Exists(tempPath))
                    {
                        System.IO.File.Delete(tempPath);
                    }
                }
                catch (IOException)
                {
                    // Log the error if deletion fails (optional)
                }

                // Return the audio file as a response
                return File(audioBytes, "audio/mpeg", tempFileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating speech: {ex.Message}");
            }
        }
        [HttpPost("generate-random-word")]
        public async Task<IActionResult> GenerateRandomWord([FromForm] string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
                return BadRequest("Topic is missing.");

            try
            {
                var wordResult = await _speechService.GetRandomWordAsync(topic.ToLower());
                if (wordResult == null)
                    return BadRequest("No words found for the specified topic.");

                return Ok(new
                {
                    word = wordResult.Word,
                    meaning = wordResult.Meaning,
                    phonetic = wordResult.Phonetic
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Error generating random word.",
                    detail = ex.Message
                });
            }
        }


    }
}
