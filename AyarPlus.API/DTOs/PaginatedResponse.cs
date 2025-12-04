namespace AyarPlus.API.DTOs;

public class PaginatedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public PaginationMeta Meta { get; set; } = new();
    public PaginationLinks Links { get; set; } = new();
}

public class PaginationMeta
{
    public int CurrentPage { get; set; }
    public int PerPage { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }
}

public class PaginationLinks
{
    public string? First { get; set; }
    public string? Last { get; set; }
    public string? Prev { get; set; }
    public string? Next { get; set; }
}

