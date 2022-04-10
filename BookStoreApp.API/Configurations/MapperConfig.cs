using AutoMapper;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models.Author;
using BookStoreApp.API.Models.Book;
using BookStoreApp.API.Models.User;

namespace BookStoreApp.API.Configurations
{
    public class MapperConfig:Profile
    {
        public MapperConfig()
        {
            CreateMap<Author, AuthorCreateDto>().ReverseMap();
            CreateMap<Author, AuthorReadOnlyDto>().ReverseMap();
            CreateMap<Author, AuthorUpdateDto>().ReverseMap();
            CreateMap<Author, AuthorDetailsDto>().ReverseMap();


            CreateMap< BookCreateDto, Book>().ReverseMap();
            CreateMap< BookUpdateDto, Book>().ReverseMap();

            CreateMap<Book,BookDetailsDto>()
                .ForMember(x => x.AuthorName,   d => d.MapFrom(map => map.Author!=null? $"{map.Author.FirstName} {map.Author.LastName}": ""))

                .ReverseMap();

            CreateMap<Book,BookReadOnlyDto> ()
                 .ForMember(x => x.AuthorName, d => d.MapFrom(map => map.Author != null ? $"{map.Author.FirstName} {map.Author.LastName}" : ""))
                .ReverseMap();

            CreateMap<ApiUser, UserDto>().ReverseMap();
        }
    }
}
