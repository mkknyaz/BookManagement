using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookManagement.Core.Models;
using BookManagement.Data.EntityFramework;
using BookManagement.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Data.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly BookDbContext _dbContext;

        public BookRepository(BookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddBookAsync(Book book)
        {
            await _dbContext.Books.AddAsync(book);
        }

        public async Task AddBooksBulkAsync(IEnumerable<Book> books)
        {
            await _dbContext.Books.AddRangeAsync(books);
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _dbContext.Books.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<Book?> GetBookByTitleAsync(string title)
        {
            return await _dbContext.Books.FirstOrDefaultAsync(x => x.Title == title && !x.IsDeleted);
        }

        public async Task<IEnumerable<Book>> GetBooksByTitleAsync(IEnumerable<string> titles)
        {
            return await _dbContext.Books.Where(x => titles.Contains(x.Title) && !x.IsDeleted).ToListAsync();
        }

        public async Task<IEnumerable<string>> GetBooksTitleAsync(int PageNumber, int PageSize)
        {
            return await _dbContext.Books
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.ViewsCount)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(x => x.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByIdsAsync(IEnumerable<int> ids)
        {
            return await _dbContext.Books.Where(x => ids.Contains(x.Id) && !x.IsDeleted).ToListAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var book = await _dbContext.Books.FindAsync(id);
            if (book == null) return false;

            book.IsDeleted = true;
            _dbContext.Books.Update(book);
            return true;
        }

        public async Task<bool> SoftDeleteBulkAsync(IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                var book = await _dbContext.Books.FindAsync(id);
                if (book is null || book.IsDeleted)
                {
                    return false;
                }

                book.IsDeleted = true;
                _dbContext.Books.Update(book);
            }
            return true;
        }

        public async Task UpdateBookAsync(Book book)
        {
            _dbContext.Books.Update(book);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
