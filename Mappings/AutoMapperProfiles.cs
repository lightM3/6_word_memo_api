using AutoMapper;
using WordMemoryApi.DTOs;
using WordMemoryApi.Entities;

namespace WordMemoryApi.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Word, WordDto>().ReverseMap();
        }
    }
}
