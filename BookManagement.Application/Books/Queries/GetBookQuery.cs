using BookManagement.Core.Sevices;
using BookManagement.Data.Interfaces;
using BookManagement.SDK.DTOs;
using MediatR;

namespace BookManagement.Application.Books.Queries
{
    public record GetBookQuery(int Id) : IRequest<BookDetailsDTO>;

    public class GetBookQueryHandler : IRequestHandler<GetBookQuery, BookDetailsDTO>
    {
        private readonly IBookRepository _bookRepository;

        public GetBookQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<BookDetailsDTO> Handle(GetBookQuery request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetBookByIdAsync(request.Id);
            if (book is null) throw new System.Exception($"Book with id '{request.Id}' was not found.");

            book.ViewsCount++;
            await _bookRepository.UpdateBookAsync(book);
            await _bookRepository.SaveChangesAsync();

            var popularityCalculator = new PopularityScore();
            var score = book.GetPopularityScore(popularityCalculator);

            return new BookDetailsDTO
            {
                Id = book.Id,
                Title = book.Title,
                PublicationYear = book.PublicationYear,
                AuthorName = book.AuthorName,
                ViewsCount = book.ViewsCount,
                PopularityScore = score
            };
        }

    }
}
