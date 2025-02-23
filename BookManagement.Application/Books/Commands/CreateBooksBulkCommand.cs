using BookManagement.Core.Models;
using BookManagement.Data.Interfaces;
using MediatR;

namespace BookManagement.Application.Books.Commands
{
    namespace BookManagement.Application.Books.Commands
    {
        public record CreateBooksBulkCommand(IEnumerable<(string Title, int PublicationYear, string AuthorName)> Books) : IRequest<IEnumerable<int>>;

        public class CreateBooksBulkCommandHandler : IRequestHandler<CreateBooksBulkCommand, IEnumerable<int>>
        {
            private readonly IBookRepository _bookRepository;

            public CreateBooksBulkCommandHandler(IBookRepository bookRepository)
            {
                _bookRepository = bookRepository;
            }

            public async Task<IEnumerable<int>> Handle(CreateBooksBulkCommand request, CancellationToken cancellationToken)
            {
                var bookData = request.Books.ToList();
                var titles = bookData.Select(x => x.Title).Distinct().ToList();

                var existingBooks = await _bookRepository.GetBooksByTitleAsync(titles);
                if (existingBooks.Any())
                {
                    var existingTitles = string.Join(", ", existingBooks.Select(x => x.Title).Distinct());
                    throw new System.Exception($"Books with titles '{existingTitles}' already exist.");
                }

                var books = bookData.Select(x => new Book
                {
                    Title = x.Title,
                    PublicationYear = x.PublicationYear,
                    AuthorName = x.AuthorName,
                    ViewsCount = 0,
                    IsDeleted = false
                }).ToList();

                await _bookRepository.AddBooksBulkAsync(books);
                await _bookRepository.SaveChangesAsync();

                return books.Select(x => x.Id);
            }
        }
    }
}
