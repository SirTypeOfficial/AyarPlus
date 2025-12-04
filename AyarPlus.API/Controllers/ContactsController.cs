using AyarPlus.API.Data;
using AyarPlus.API.DTOs;
using AyarPlus.API.Models;
using AyarPlus.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AyarPlus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ContactsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;

    public ContactsController(ApplicationDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<Contact>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<Contact>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? type = null)
    {
        if (page < 1) page = 1;
        if (perPage < 1) perPage = 10;
        if (perPage > 100) perPage = 100;

        var query = _context.Contacts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => 
                (c.Name != null && c.Name.Contains(search)) ||
                (c.Email != null && c.Email.Contains(search)) ||
                (c.Phone != null && c.Phone.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(c => c.Type == type);
        }

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)perPage);

        var data = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync();

        var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

        return Ok(new PaginatedResponse<Contact>
        {
            Data = data,
            Meta = new PaginationMeta
            {
                CurrentPage = page,
                PerPage = perPage,
                Total = total,
                TotalPages = totalPages
            },
            Links = new PaginationLinks
            {
                First = $"{baseUrl}?page=1&perPage={perPage}",
                Last = totalPages > 0 ? $"{baseUrl}?page={totalPages}&perPage={perPage}" : null,
                Prev = page > 1 ? $"{baseUrl}?page={page - 1}&perPage={perPage}" : null,
                Next = page < totalPages ? $"{baseUrl}?page={page + 1}&perPage={perPage}" : null
            }
        });
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Contact), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Contact>> GetById(int id)
    {
        var contact = await _context.Contacts.FindAsync(id);

        if (contact == null)
            return NotFound(new { error = "Contact not found" });

        return Ok(contact);
    }

    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Contact), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Contact>> CreateJson([FromBody] ContactCreateDto dto)
    {
        return await CreateContact(dto);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(Contact), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Contact>> CreateForm([FromForm] ContactCreateDto dto)
    {
        return await CreateContact(dto);
    }

    private async Task<ActionResult<Contact>> CreateContact(ContactCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var contact = new Contact
        {
            CompanyId = dto.CompanyId,
            UserId = dto.UserId,
            Type = dto.Type,
            Name = dto.Name,
            Email = dto.Email,
            TaxNumber = dto.TaxNumber,
            Phone = dto.Phone,
            Address = dto.Address,
            City = dto.City,
            ZipCode = dto.ZipCode,
            State = dto.State,
            Country = dto.Country,
            Website = dto.Website,
            CurrencyCode = dto.CurrencyCode,
            Reference = dto.Reference,
            CreatedFrom = dto.CreatedFrom,
            CreatedBy = dto.CreatedBy,
            FileNumber = dto.FileNumber
        };

        try
        {
            contact.FrontImagePath = await _fileService.SaveFileAsync(dto.FrontImage, "contacts");
            contact.BackImagePath = await _fileService.SaveFileAsync(dto.BackImage, "contacts");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = contact.Id }, contact);
    }

    [HttpPut("{id:int}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Contact), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Contact>> UpdateJson(int id, [FromBody] ContactUpdateDto dto)
    {
        return await UpdateContact(id, dto);
    }

    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(Contact), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Contact>> UpdateForm(int id, [FromForm] ContactUpdateDto dto)
    {
        return await UpdateContact(id, dto);
    }

    private async Task<ActionResult<Contact>> UpdateContact(int id, ContactUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var contact = await _context.Contacts.FindAsync(id);

        if (contact == null)
            return NotFound(new { error = "Contact not found" });

        contact.CompanyId = dto.CompanyId;
        contact.UserId = dto.UserId;
        contact.Type = dto.Type;
        contact.Name = dto.Name;
        contact.Email = dto.Email;
        contact.TaxNumber = dto.TaxNumber;
        contact.Phone = dto.Phone;
        contact.Address = dto.Address;
        contact.City = dto.City;
        contact.ZipCode = dto.ZipCode;
        contact.State = dto.State;
        contact.Country = dto.Country;
        contact.Website = dto.Website;
        contact.CurrencyCode = dto.CurrencyCode;
        contact.Reference = dto.Reference;
        contact.CreatedFrom = dto.CreatedFrom;
        contact.CreatedBy = dto.CreatedBy;
        contact.FileNumber = dto.FileNumber;

        try
        {
            if (dto.RemoveFrontImage && !string.IsNullOrEmpty(contact.FrontImagePath))
            {
                _fileService.DeleteFile(contact.FrontImagePath);
                contact.FrontImagePath = null;
            }
            else if (dto.FrontImage != null)
            {
                _fileService.DeleteFile(contact.FrontImagePath);
                contact.FrontImagePath = await _fileService.SaveFileAsync(dto.FrontImage, "contacts");
            }

            if (dto.RemoveBackImage && !string.IsNullOrEmpty(contact.BackImagePath))
            {
                _fileService.DeleteFile(contact.BackImagePath);
                contact.BackImagePath = null;
            }
            else if (dto.BackImage != null)
            {
                _fileService.DeleteFile(contact.BackImagePath);
                contact.BackImagePath = await _fileService.SaveFileAsync(dto.BackImage, "contacts");
            }
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        await _context.SaveChangesAsync();

        return Ok(contact);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var contact = await _context.Contacts.FindAsync(id);

        if (contact == null)
            return NotFound(new { error = "Contact not found" });

        contact.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}/permanent")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePermanent(int id)
    {
        var contact = await _context.Contacts
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contact == null)
            return NotFound(new { error = "Contact not found" });

        _fileService.DeleteFile(contact.FrontImagePath);
        _fileService.DeleteFile(contact.BackImagePath);

        _context.Contacts.Remove(contact);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id:int}/restore")]
    [ProducesResponseType(typeof(Contact), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Contact>> Restore(int id)
    {
        var contact = await _context.Contacts
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt.HasValue);

        if (contact == null)
            return NotFound(new { error = "Deleted contact not found" });

        contact.DeletedAt = null;
        await _context.SaveChangesAsync();

        return Ok(contact);
    }
}
