using AutoMapper;
using DocScheduler.SlotService;

namespace DocScheduler.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BookSlotRequest, TakeSlotRequest>()
                .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.Start.ToString("o")))
                .ForMember(dest => dest.End, opt => opt.MapFrom(src => src.End.ToString("o")))
                .ForMember(dest => dest.Patient, opt => opt.MapFrom(src => new Patient
                {
                    Name = src.Name,
                    SecondName = src.SecondName,
                    Email = src.Email,
                    Phone = src.Phone
                }));
        }
    }
}