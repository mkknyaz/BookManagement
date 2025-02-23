﻿using BookManagement.API.DTOs;
using BookManagement.Application.Books.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookManagement.Application.Models;
using BookManagement.Application.Books.Commands;
using MapsterMapper;
using BookManagement.Application.Books.Commands.BookManagement.Application.Books.Commands;

namespace BookManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public BooksController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBook(int id)
        {
            if (id <= 0) return BadRequest("Invalid book ID.");

            var query = new GetBookQuery(id);
            BookReadModel readModel;
            try
            {
                readModel = await _sender.Send(query);
                var bookDetailsDTO = _mapper.Map<BookDetailsDTO>(readModel);
                return Ok(bookDetailsDTO);

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetBooksTitle([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            if(pageNumber <= 0 || pageSize <= 0) return BadRequest("Page number and page size must be greater than zero.");
            
            var query = new GetBooksTitlesQuery(pageNumber, pageSize);
            var titles = await _sender.Send(query);
            return Ok(titles);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookDTO bookDTO)
        {
            if (bookDTO == null) return BadRequest("Book data is required.");
            if (string.IsNullOrWhiteSpace(bookDTO.Title)) return BadRequest("Title cannot be empty.");
            if (string.IsNullOrWhiteSpace(bookDTO.AuthorName)) return BadRequest("Author name cannot be empty.");
            if (bookDTO.PublicationYear > DateTime.Now.Year) return BadRequest("Publication year cannot be in the future.");

            var command = new CreateBookCommand(bookDTO.Title, bookDTO.PublicationYear, bookDTO.AuthorName);
            try
            {
                BookReadModel readModel = await _sender.Send(command);
                var bookDetailsDTO = _mapper.Map<BookDetailsDTO>(readModel);
                return CreatedAtAction(nameof(GetBook), new { id = bookDetailsDTO.Id }, bookDetailsDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("bulk")]
        [Authorize]
        public async Task<IActionResult> CreateBooksBulk([FromBody] IEnumerable<CreateBookDTO> createBookDTOs)
        {
            if (createBookDTOs is null || !createBookDTOs.Any()) return BadRequest("No books provided.");

            var booksData = createBookDTOs.Select(dto => (dto.Title, dto.PublicationYear, dto.AuthorName));
            var command = new CreateBooksBulkCommand(booksData);
            try
            {
                var createdIds = await _sender.Send(command);
                return StatusCode(201, createdIds);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookDTO updateBookDto)
        {
            if (id <= 0) return BadRequest("Invalid book ID.");
            if (string.IsNullOrWhiteSpace(updateBookDto.Title)) return BadRequest("Title cannot be empty.");
            if (string.IsNullOrWhiteSpace(updateBookDto.AuthorName)) return BadRequest("Author name cannot be empty.");
            if (updateBookDto.PublicationYear > DateTime.Now.Year) return BadRequest("Publication year cannot be in the future.");

            var command = new UpdateBookCommand(id, updateBookDto.Title, updateBookDto.PublicationYear, updateBookDto.AuthorName);
            bool result = await _sender.Send(command);
            if (!result) return NotFound($"Book with id '{id}' not found.");
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> SoftDeleteBook(int id)
        {
            if (id <= 0) return BadRequest("Invalid book ID.");

            var command = new SoftDeleteBookCommand(id);
            bool result = await _sender.Send(command);
            if (!result) return NotFound($"Book with id '{id}' not found or already deleted.");
            return NoContent();
        }

        [HttpDelete("Bulk")]
        [Authorize]
        public async Task<IActionResult> SoftDeleteBooksBulk([FromBody] IEnumerable<int> ids)
        {
            if (ids is null) return BadRequest("No ids provided.");

            var command = new BulkSoftDeleteBooksCommand(ids.Distinct().ToList());
            var missingIds = await _sender.Send(command);
            if (missingIds.Any()) return BadRequest($"Books with the following IDs were not found or already deleted: {string.Join(", ", missingIds)}");
            return NoContent();
        }
    }
}
