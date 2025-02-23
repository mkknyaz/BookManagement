using BookManagement.Application.Models;
using BookManagement.Core.Sevices;
using BookManagement.Data.Interfaces;
using MediatR;

namespace BookManagement.Application.Books.Queries
{
    public record GetBookQuery(int Id) : IRequest<BookReadModel>;

    public class GetBookQueryHandler : IRequestHandler<GetBookQuery, BookReadModel>
    {
        private readonly IBookRepository _bookRepository;

        public GetBookQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<BookReadModel> Handle(GetBookQuery request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetBookByIdAsync(request.Id);
            if (book is null) throw new System.Exception($"Book with id '{request.Id}' was not found.");

            book.ViewsCount++;
            await _bookRepository.UpdateBookAsync(book);
            await _bookRepository.SaveChangesAsync();

            var popularityCalculator = new PopularityScore();
            var score = book.GetPopularityScore(popularityCalculator);

            return new BookReadModel
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
