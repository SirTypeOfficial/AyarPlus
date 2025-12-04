using System.ComponentModel.DataAnnotations;

namespace AyarPlus.API.DTOs;

public class ContactUpdateDto
{
    public int? CompanyId { get; set; }
    public int? UserId { get; set; }

    [MaxLength(50)]
    public string? Type { get; set; }

    [MaxLength(200)]
    public string? Name { get; set; }

    [EmailAddress]
    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? TaxNumber { get; set; }

    [Phone]
    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(20)]
    public string? ZipCode { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [Url]
    [MaxLength(300)]
    public string? Website { get; set; }

    [MaxLength(10)]
    public string? CurrencyCode { get; set; }

    [MaxLength(100)]
    public string? Reference { get; set; }

    [MaxLength(100)]
    public string? CreatedFrom { get; set; }

    public int? CreatedBy { get; set; }

    [MaxLength(100)]
    public string? FileNumber { get; set; }

    public IFormFile? FrontImage { get; set; }
    public IFormFile? BackImage { get; set; }
    
    public bool RemoveFrontImage { get; set; }
    public bool RemoveBackImage { get; set; }
}

