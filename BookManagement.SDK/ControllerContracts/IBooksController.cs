using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookManagement.SDK.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BookManagement.SDK.ControllerContracts
{
    public interface IBooksController
    {
        Task<IActionResult> GetBook(int id);
        Task<IActionResult> GetBooksTitle(int pageNumber, int pageSize);
        Task<IActionResult> CreateBook(CreateBookDTO dto);
        Task<IActionResult> CreateBooksBulk(IEnumerable<CreateBookDTO> dtos);
        Task<IActionResult> UpdateBook(int id, UpdateBookDTO dto);
        Task<IActionResult> SoftDeleteBook(int id);
        Task<IActionResult> SoftDeleteBooksBulk(IEnumerable<int> ids);
    }
}
