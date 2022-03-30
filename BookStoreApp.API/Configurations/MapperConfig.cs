using AutoMapper;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models.Author;
using BookStoreApp.API.Models.Book;

namespace BookStoreApp.API.Configurations
{
    public class MapperConfig:Profile
    {
        public MapperConfig()
        {
            CreateMap<Author, AuthorCreateDto>().ReverseMap();
            CreateMap<Author, AuthorReadOnlyDto>().ReverseMap();
            CreateMap<Author, AuthorUpdateDto>().ReverseMap();

            CreateMap<Book, BookCreateDto>().ReverseMap();
            CreateMap<Book, BookDetailsDto>().ReverseMap();
            CreateMap<Book, BookReadOnlyDto>().ReverseMap();
            CreateMap<Book, BookUpdateDto>().ReverseMap();

        }
    }
}
