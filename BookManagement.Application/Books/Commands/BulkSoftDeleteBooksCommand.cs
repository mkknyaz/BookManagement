using BookManagement.Data.Interfaces;
using MediatR;

namespace BookManagement.Application.Books.Commands
{
    public record BulkSoftDeleteBooksCommand(IEnumerable<int> Ids) : IRequest<IEnumerable<int>>;

    public class BulkSoftDeleteBooksCommandHandler : IRequestHandler<BulkSoftDeleteBooksCommand, IEnumerable<int>>
    {
        private readonly IBookRepository _bookRepository;

        public BulkSoftDeleteBooksCommandHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<int>> Handle(BulkSoftDeleteBooksCommand request, CancellationToken cancellationToken)
        {
            var distinctIds = request.Ids.Distinct().ToList();

            var foundBooks = await _bookRepository.GetBooksByIdsAsync(distinctIds);
            var foundIds = foundBooks.Select(b => b.Id).ToList();
            var missingIds = distinctIds.Except(foundIds).ToList();

            if (missingIds.Any()) return missingIds;

            var result = await _bookRepository.SoftDeleteBulkAsync(distinctIds);
            if (!result) return distinctIds;

            await _bookRepository.SaveChangesAsync();
            return new List<int>();
        }
    }
}
