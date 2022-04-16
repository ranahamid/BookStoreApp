﻿using BookStoreApp.Blazor.WebAssembly.UI.Services;
using BookStoreApp.Blazor.WebAssembly.UI.Services.Base;
using Microsoft.AspNetCore.Components;

namespace BookStoreApp.Blazor.WebAssembly.UI.Pages.Authors
{
    public partial class Create
    {
        [Inject]
        public IAuthorService authorService { get; set; }
        [Inject]
        public NavigationManager _navigationManager { get; set; }

        private AuthorCreateDto Author = new AuthorCreateDto();
        private async Task HandleCreateAuthor()
        {
            var response = await authorService.Create(author: Author);
            // if (response.Success)
            {
                BackToAuthorList();
            }
        }
        private void BackToAuthorList()
        {
            _navigationManager.NavigateTo("/authors/");
        }
    }
}
