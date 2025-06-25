using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EXE_FAIEnglishTutor.Services.Interface;

namespace EXE_FAIEnglishTutor.Services.Implementaion
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];
            _cloudinary = new Cloudinary(new Account(cloudName, apiKey, apiSecret));
        }

        public async Task<string> UploadAudioAsync(byte[] audioBytes, string fileName)
        {
            using (var stream = new MemoryStream(audioBytes))
            {
                var uploadParams = new RawUploadParams()
                {
                    File = new FileDescription($"{fileName}.mp3", stream),
                    PublicId = fileName,
                    Overwrite = true
                    // Không cần gán ResourceType nữa!
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl?.ToString();
            }
        }
    }
}
