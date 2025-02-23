using BookManagement.Data.Interfaces;
using MediatR;

namespace BookManagement.Application.Books.Commands
{
    public record SoftDeleteBookCommand(int Id) : IRequest<bool>;

    public class SoftDeleteBookCommandHandler : IRequestHandler<SoftDeleteBookCommand, bool>
    {
        private readonly IBookRepository _bookRepository;

        public SoftDeleteBookCommandHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<bool> Handle(SoftDeleteBookCommand request, CancellationToken cancellationToken)
        {
            var result = await _bookRepository.SoftDeleteAsync(request.Id);
            if (!result) return false;

            await _bookRepository.SaveChangesAsync();
            return true;
        }
    }
}
