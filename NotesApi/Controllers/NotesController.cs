using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesApi.Data;
using NotesApi.Models;
using System.Security.Claims;

namespace NotesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly NotesContext _context;

        public NotesController(NotesContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetNotes()
        {
            var userId = GetUserId();
            var notes = await _context.Notes.Where(n => n.UserId == userId.ToString()).ToListAsync();
            return Ok(notes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNote(int id)
        {
            var userId = GetUserId();
            var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId.ToString());
            if (note == null) return NotFound();
            return Ok(note);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] NoteDto dto)
        {
            var userId = GetUserId();
            var note = new Note { Content = dto.Content, UserId = userId.ToString() };
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetNote), new { id = note.Id }, note);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] NoteDto dto)
        {
            var userId = GetUserId();
            var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId.ToString());
            if (note == null) return NotFound();
            note.Content = dto.Content;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var userId = GetUserId();
            var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId.ToString());
            if (note == null) return NotFound();
            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    public class NoteDto { public string Content { get; set; } }
}
