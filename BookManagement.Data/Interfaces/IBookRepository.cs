using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookManagement.Core.Models;

namespace BookManagement.Data.Interfaces
{
    public interface IBookRepository
    {
        Task<Book?> GetBookByIdAsync(int id);
        Task<Book?> GetBookByTitleAsync(string title);
        Task<IEnumerable<Book>> GetBooksByIdsAsync(IEnumerable<int> ids);

        Task<IEnumerable<Book>> GetBooksByTitleAsync(IEnumerable<string> titles);
        Task<IEnumerable<string>> GetBooksTitleAsync(int PageNumber, int PageSize);

        Task AddBookAsync(Book book);
        Task AddBooksBulkAsync(IEnumerable<Book> books);

        Task<bool> SoftDeleteAsync(int id);
        Task<bool> SoftDeleteBulkAsync(IEnumerable<int> ids);

        Task UpdateBookAsync(Book book);
        Task SaveChangesAsync();
    }
}
