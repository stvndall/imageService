using System;
using System.Threading.Tasks;
using imageService;
using Moq;
using Xunit;

namespace tests.imageService
{
    public class ImageLocatorTests
    {
        private Mock<IRepository<ImageDto>> _repo;
        private IImageLocator _imageLocator;
        private Mock<IImageSerialiser> _serialiser;


        public ImageLocatorTests()
        {
            _repo = new Mock<IRepository<ImageDto>>();
            _serialiser=new Mock<IImageSerialiser>();
            _imageLocator = new ImageLocator(_repo.Object, _serialiser.Object);
        }
        [Fact]
        public async Task RequestImage_NotFound_ShouldReturn_null()
        {
            var notFound = Guid.NewGuid();
            _repo.Setup(r => r.GetById(notFound)).Returns(Task.FromResult((ImageDto)null));

            var image = await _imageLocator.LocateImage(notFound);

            Assert.Null(image);
        }

        [Fact]
        public async Task RequestImage_NotFound_ShouldReturn_OriginalImage()
        {
            var foundId = Guid.NewGuid();
            var b64 = "base64";
            ImageDto dto = new ImageDto(foundId, b64);
            Mock<IImage> returnedImage = new Mock<IImage>();
            _repo.Setup(r => r.GetById(foundId)).Returns(Task.FromResult(dto));
            _serialiser.Setup(x => x.GetOriginalImage(b64)).Returns(returnedImage.Object);
            var image = await _imageLocator.LocateImage(foundId);

            Assert.NotNull(image);
            Assert.Same(returnedImage.Object, image);
        }
    }
}
