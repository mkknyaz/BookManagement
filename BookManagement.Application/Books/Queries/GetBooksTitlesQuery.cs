using BookManagement.Data.Interfaces;
using MediatR;

namespace BookManagement.Application.Books.Queries
{
    public record GetBooksTitlesQuery(int PageNumber, int PageSize) : IRequest<IEnumerable<string>>;

    public class GetBooksTitlesQueryHandler : IRequestHandler<GetBooksTitlesQuery, IEnumerable<string>>
    {
        private readonly IBookRepository _bookRepository;

        public GetBooksTitlesQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<string>> Handle(GetBooksTitlesQuery request, CancellationToken cancellationToken)
        {
            return await _bookRepository.GetBooksTitleAsync(request.PageNumber, request.PageSize);
        }
    }
}
