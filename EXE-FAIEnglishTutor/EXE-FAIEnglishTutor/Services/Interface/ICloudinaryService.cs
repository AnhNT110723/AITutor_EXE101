namespace EXE_FAIEnglishTutor.Services.Interface
{
    public interface ICloudinaryService
    {
        /// <summary>
        /// Upload audio file to Cloudinary and return the public URL.
        /// </summary>
        /// <param name="audioBytes">Audio file bytes</param>
        /// <param name="fileName">File name (without extension)</param>
        /// <returns>Public URL string</returns>
        Task<string> UploadAudioAsync(byte[] audioBytes, string fileName);
    }
}
