using BookManagement.API.DTOs;
using BookManagement.Core.Models;
using BookManagement.Core.Sevices;
using BookManagement.Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBook(int id)
        {
            if (id <= 0) return BadRequest("Invalid book ID.");

            var book = await _bookRepository.GetBookByIdAsync(id);
            if (book is null) return NotFound();

            book.ViewsCount++;
            await _bookRepository.UpdateBookAsync(book);
            await _bookRepository.SaveChangesAsync();

            var popularityCalculator = new PopularityScore();
            var score = book.GetPopularityScore(popularityCalculator);

            var bookDto = new BookDetailsDTO
            {
                Id = book.Id,
                Title = book.Title,
                PublicationYear = book.PublicationYear,
                AuthorName = book.AuthorName,
                ViewsCount = book.ViewsCount,
                PopularityScore = score
            };
            return Ok(bookDto);

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetBooksTitle([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            if(pageNumber <= 0 || pageSize <= 0) return BadRequest("Page number and page size must be greater than zero.");
            var books = await _bookRepository.GetBooksTitleAsync(pageNumber, pageSize);

            return Ok(books);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookDTO bookDTO)
        {
            if (bookDTO == null) return BadRequest("Book data is required.");
            if (string.IsNullOrWhiteSpace(bookDTO.Title)) return BadRequest("Title cannot be empty.");
            if (string.IsNullOrWhiteSpace(bookDTO.AuthorName)) return BadRequest("Author name cannot be empty.");
            if (bookDTO.PublicationYear > DateTime.Now.Year) return BadRequest("Publication year cannot be in the future.");
            
            if (await _bookRepository.GetBookByTitleAsync(bookDTO.Title) is not null) return BadRequest($"Book with '{bookDTO.Title}' Title already exist."); 

            var book = new Book
            {
                Title = bookDTO.Title,
                PublicationYear = bookDTO.PublicationYear,
                AuthorName = bookDTO.AuthorName
            };

            await _bookRepository.AddBookAsync(book);
            await _bookRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        [HttpPost("bulk")]
        [Authorize]
        public async Task<IActionResult> CreateBooksBulk([FromBody] IEnumerable<CreateBookDTO> createBookDTOs)
        {
            if (createBookDTOs is null || !createBookDTOs.Any()) return BadRequest("No books provided.");

            var booksTitle = createBookDTOs.Select(x => x.Title).ToList();

            var existingBooks = await _bookRepository.GetBooksByTitleAsync(booksTitle);

            var alreadyCreatedBooks = existingBooks.Select(x => x.Title).Distinct().ToList();
            if (alreadyCreatedBooks.Any())
            {
                var existingTitles = string.Join(", ", alreadyCreatedBooks);
                return BadRequest($"Books with '{existingTitles}' Titles already exist.");
            }

            var books = createBookDTOs.Select(dto => new Book
            {
                Title = dto.Title,
                PublicationYear = dto.PublicationYear,
                AuthorName = dto.AuthorName,
                ViewsCount = 0,
                IsDeleted = false
            }).ToList();

            await _bookRepository.AddBooksBulkAsync(books);
            await _bookRepository.SaveChangesAsync();

            return StatusCode(201, books);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookDTO updateBookDto)
        {
            if (id <= 0) return BadRequest("Invalid book ID.");
            if (string.IsNullOrWhiteSpace(updateBookDto.Title)) return BadRequest("Title cannot be empty.");
            if (string.IsNullOrWhiteSpace(updateBookDto.AuthorName)) return BadRequest("Author name cannot be empty.");
            if (updateBookDto.PublicationYear > DateTime.Now.Year) return BadRequest("Publication year cannot be in the future.");

            var book = await _bookRepository.GetBookByIdAsync(id);
            if (book is null) return NotFound();

            book.Title = updateBookDto.Title;
            book.AuthorName = updateBookDto.AuthorName;
            book.PublicationYear = updateBookDto.PublicationYear;

            await _bookRepository.UpdateBookAsync(book);
            await _bookRepository.SaveChangesAsync();

            return Ok(book);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> SoftDeleteBook(int id)
        {
            if (id <= 0) return BadRequest("Invalid book ID.");

            var result = await _bookRepository.SoftDeleteAsync(id);
            if (!result) return NotFound($"Book with id '{id}' already deleted or not found");

            await _bookRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("Bulk")]
        [Authorize]
        public async Task<IActionResult> SoftDeleteBooksBulk([FromBody] IEnumerable<int> ids)
        {
            if (ids is null) return BadRequest("No ids provided.");

            var distinctIds = ids.Distinct().ToList();
            var result = await _bookRepository.SoftDeleteBulkAsync(distinctIds);
            if(result)
            {
                await _bookRepository.SaveChangesAsync();
                return NoContent();
            }

            var foundBooks = await _bookRepository.GetBooksByIdsAsync(distinctIds);
            var foundBooksIds = foundBooks.Select(x => x.Id).ToList();

            var missingBooksIds = distinctIds.Except(foundBooksIds).ToList();
            return BadRequest($"Books with the following IDs were not found or already deleted: {string.Join(", ", missingBooksIds)}");
        }


    }
}
