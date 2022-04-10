﻿using BookStoreApp.Blazor.WebAssembly.UI.Services.Base;
namespace BookStoreApp.Blazor.WebAssembly.UI.Services
{
    public interface IBookService
    {
        Task<Response<List<BookReadOnlyDto>>> Get();
        Task<Response<BookDetailsDto>> Get(int id);
        Task<Response<BookUpdateDto>> GetForUpdate(int id);
        Task<Response<int>> Create(BookCreateDto Book);
          
        Task<Response<int>> Edit(int id, BookUpdateDto Book); 
        Task<Response<int>> Delete(int id);
    }
}
