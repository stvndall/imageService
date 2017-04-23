using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace imageService
{
    public class ImageLocator : IImageLocator
    {
        private readonly IRepository<ImageDto> _repo;
        private readonly IImageSerialiser _serialiser;

        public ImageLocator(IRepository<ImageDto> repo, IImageSerialiser serialiser)
        {
            _repo = repo;
            _serialiser = serialiser;
        }

        public Task<IImage> LocateImage(Guid imageId)
        {
            return _repo.GetById(imageId)
                .ContinueWith(task => task.Result == null ? null : _serialiser.GetOriginalImage(task.Result.B64));
        }
    }

    public class LocalImage : IImage
    {
        private ImageFormat _fmt;
        public string ImageType => GetFormat(_fmt);

        private string GetFormat(ImageFormat fmt)
        {
            if (fmt.Guid == ImageFormat.Jpeg.Guid)
                return "jpg";
            return "";

        }

        public byte[]ImageBytes { get; }

        public LocalImage(byte[] image, ImageFormat imageType)
        {
            _fmt = imageType;
            ImageBytes = image;
        }
    }

    public interface IImage
    {
        string ImageType { get; }
        byte[]ImageBytes { get; }
    }

    public interface IImageLocator
    {
        Task<IImage> LocateImage(Guid imageId);
    }

    public interface IRepository<TDbType> where TDbType : IDataObject
    {
        Task<TDbType> GetById(Guid id);
        TDbType GetWhere(string whereStr, object param);
        Guid InsertNew(TDbType objectToInsert);
        void UpdateRecord(TDbType record);
        void Delete(TDbType record);
        void Delete(Guid Id);
    }

    public interface IDataObject
    {
        Guid Id { get; }
    }

    public class ImageDto : IDataObject
    {
        public ImageDto(Guid id, string b64)
        {
            Id = id;
            B64 = b64;
        }

        public Guid Id { get; }
        public string B64 { get; }
    }
}