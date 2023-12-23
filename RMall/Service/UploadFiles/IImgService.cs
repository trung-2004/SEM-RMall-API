namespace RMall.Service.UploadFiles
{
    public interface IImgService
    {
        Task<string> UploadImageAsync(IFormFile img);
    }
}
