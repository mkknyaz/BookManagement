using BookManagement.Data.Interfaces;
using MediatR;

namespace BookManagement.Application.Books.Commands
{
    public record UpdateBookCommand(int Id, string Title, int PublicationYear, string AuthorName) : IRequest<bool>;

    public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, bool>
    {
        private readonly IBookRepository _bookRepository;

        public UpdateBookCommandHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<bool> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetBookByIdAsync(request.Id);
            if (book is null) return false;

            book.Title = request.Title;
            book.PublicationYear = request.PublicationYear;
            book.AuthorName = request.AuthorName;

            await _bookRepository.UpdateBookAsync(book);
            await _bookRepository.SaveChangesAsync();

            return true;
        }
    }
}
