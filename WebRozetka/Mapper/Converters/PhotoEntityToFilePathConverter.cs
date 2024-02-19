using AutoMapper;
using WebRozetka.Data.Entities.Photo;

namespace WebRozetka.Mapper.Converters
{
    public class PhotoEntityToFilePathConverter : ITypeConverter<ICollection<PhotoEntity>, List<string>>
    {
        public List<string> Convert(ICollection<PhotoEntity> sourceMember, List<string> destination, ResolutionContext context)
        {
            return sourceMember?.Select(photo => photo.FilePath).ToList();
        }
    }
}
