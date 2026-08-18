using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EXE_FAIEnglishTutor.Services.Interface;
using Microsoft.Extensions.Options;

namespace EXE_FAIEnglishTutor.Services.Implementaion
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; } = null!;
        public string ApiKey { get; set; } = null!;
        public string ApiSecret { get; set; } = null!;
    }

    public class CloudinaryService : IFileUploadService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var settings = config.Value;
            var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true; // luôn dùng HTTPS
        }

        /// <summary>
        /// Upload file lên Cloudinary và trả về URL công khai.
        /// Tham số <paramref name="folder"/> là tên thư mục trên Cloudinary (ví dụ: "situations", "avatars").
        /// </summary>
        public async Task<string> UploadFileAsync(IFormFile file, string folder = "uploads")
        {
            if (file == null || file.Length == 0)
                return null;

            await using var stream = file.OpenReadStream();

            // Xác định loại resource (image / video / raw)
            var resourceType = GetResourceType(file.ContentType);

            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = $"fai-english/{folder}",
                // Tạo public_id duy nhất để tránh ghi đè
                PublicId = $"{Guid.NewGuid()}_{Path.GetFileNameWithoutExtension(file.FileName)}",
                Overwrite = false,
                UseFilename = false
            };

            UploadResult result;

            if (resourceType == ResourceType.Image)
            {
                var imageParams = new ImageUploadParams
                {
                    File = uploadParams.File,
                    Folder = uploadParams.Folder,
                    PublicId = uploadParams.PublicId,
                    Overwrite = false,
                    UseFilename = false,
                    // Tự động tối ưu chất lượng và format
                    Transformation = new Transformation().Quality("auto").FetchFormat("auto")
                };
                result = await _cloudinary.UploadAsync(imageParams);
            }
            else if (resourceType == ResourceType.Video)
            {
                var videoParams = new VideoUploadParams
                {
                    File = uploadParams.File,
                    Folder = uploadParams.Folder,
                    PublicId = uploadParams.PublicId,
                    Overwrite = false,
                    UseFilename = false
                };
                result = await _cloudinary.UploadAsync(videoParams);
            }
            else
            {
                // Raw file (PDF, audio, v.v.)
                result = await _cloudinary.UploadAsync(uploadParams);
            }

            if (result.Error != null)
                throw new Exception($"Cloudinary upload failed: {result.Error.Message}");

            // Trả về URL HTTPS của file đã upload
            return result.SecureUrl.ToString();
        }

        private static ResourceType GetResourceType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType)) return ResourceType.Raw;

            if (contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                return ResourceType.Image;

            if (contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase)
                || contentType.Contains("audio/", StringComparison.OrdinalIgnoreCase))
                return ResourceType.Video;

            return ResourceType.Raw;
        }
    }
}
