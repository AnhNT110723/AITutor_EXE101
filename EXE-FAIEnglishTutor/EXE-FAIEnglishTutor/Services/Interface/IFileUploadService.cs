namespace EXE_FAIEnglishTutor.Services.Interface
{
    public interface IFileUploadService
    {
        Task<string> UploadFileAsync(IFormFile file, string folder = "uploads");
    }
}
