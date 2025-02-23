using BookManagement.Application.Models;
using BookManagement.Core.Models;
using BookManagement.Data.Interfaces;
using MediatR;

namespace BookManagement.Application.Books.Commands
{

    public record CreateBookCommand(string Title, int PublicationYear, string AuthorName) : IRequest<BookReadModel>;

    public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookReadModel>
    {
        private readonly IBookRepository _bookRepository;

        public CreateBookCommandHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<BookReadModel> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            var existingBook = await _bookRepository.GetBookByTitleAsync(request.Title);
            if (existingBook is not null) throw new System.Exception($"Book with title '{request.Title}' already exists.");

            var book = new Book
            {
                Title = request.Title,
                PublicationYear = request.PublicationYear,
                AuthorName = request.AuthorName,
                ViewsCount = 0,
                IsDeleted = false
            };

            await _bookRepository.AddBookAsync(book);
            await _bookRepository.SaveChangesAsync();

            return new BookReadModel
            {
                Id = book.Id,
                Title = book.Title,
                PublicationYear = book.PublicationYear,
                AuthorName = book.AuthorName,
                ViewsCount = book.ViewsCount,
                PopularityScore = 0
            };
        }
    }
}
