using Microsoft.EntityFrameworkCore;

namespace OrderShop.UsersApi;

/// <summary>Таблица User: Id (PK), Login (уникальный), PassHash.</summary>
public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string PassHash { get; set; } = string.Empty;
}

public class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Login).IsRequired().HasMaxLength(50);
            entity.Property(u => u.PassHash).IsRequired().HasMaxLength(64); // Хеш SHA-256 = 64 символа
            entity.HasIndex(u => u.Login).IsUnique(); // логин уникален на уровне БД
        });
    }
}

/// <summary>Тело запроса POST/PUT: {"Login": "...", "PassHash": "..."}.</summary>
public record UserRequest(string? Login, string? PassHash);

/// <summary>Пользователь в ответе сервера. Хеш пароля клиенту не возвращается.</summary>
public record UserDto(int Id, string Login);

/// <summary>Единый формат JSON-ответа сервера.</summary>
public record ApiResponse(bool Success, string Message, UserDto? User = null, IEnumerable<string>? Errors = null);