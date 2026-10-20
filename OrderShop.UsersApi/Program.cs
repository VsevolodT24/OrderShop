using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using OrderShop.UsersApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Users") ?? "Data Source=users.db"));

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Создаём базу данных и таблицу User при первом запуске
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<UsersDbContext>().Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

// POST /user – создание пользователя
app.MapPost("/user", async (UserRequest request, UsersDbContext db) =>
{
    List<string> errors = Validation.Validate(request);
    if (errors.Count > 0)
        return Results.BadRequest(new ApiResponse(false, "Ошибка валидации данных.", null, errors));

    string login = request.Login!.Trim();
    if (await db.Users.AnyAsync(u => u.Login == login))
        return Results.Conflict(new ApiResponse(false, $"Пользователь с логином '{login}' уже существует."));

    var user = new User { Login = login, PassHash = request.PassHash!.ToLowerInvariant() };
    db.Users.Add(user);
    try
    {
        await db.SaveChangesAsync();
    }
    catch (DbUpdateException)
    {
        return Results.Conflict(new ApiResponse(false, $"Пользователь с логином '{login}' уже существует."));
    }

    return Results.Created($"/user/{user.Id}",
        new ApiResponse(true, "Пользователь успешно создан.", new UserDto(user.Id, user.Login)));
});

// GET /user/{id} – получение пользователя по id
app.MapGet("/user/{id:int}", async (int id, UsersDbContext db) =>
{
    User? user = await db.Users.FindAsync(id);
    return user is null
        ? Results.NotFound(new ApiResponse(false, $"Пользователь с Id={id} не найден."))
        : Results.Ok(new ApiResponse(true, "Пользователь найден.", new UserDto(user.Id, user.Login)));
});

// PUT /user/{id} – обновление данных пользователя
app.MapPut("/user/{id:int}", async (int id, UserRequest request, UsersDbContext db) =>
{
    List<string> errors = Validation.Validate(request);
    if (errors.Count > 0)
        return Results.BadRequest(new ApiResponse(false, "Ошибка валидации данных.", null, errors));

    User? user = await db.Users.FindAsync(id);
    if (user is null)
        return Results.NotFound(new ApiResponse(false, $"Пользователь с Id={id} не найден."));

    string login = request.Login!.Trim();
    if (await db.Users.AnyAsync(u => u.Login == login && u.Id != id))
        return Results.Conflict(new ApiResponse(false, $"Логин '{login}' уже занят другим пользователем."));

    user.Login = login;
    user.PassHash = request.PassHash!.ToLowerInvariant();
    try
    {
        await db.SaveChangesAsync();
    }
    catch (DbUpdateException)
    {
        return Results.Conflict(new ApiResponse(false, $"Логин '{login}' уже занят другим пользователем."));
    }

    return Results.Ok(new ApiResponse(true, "Данные пользователя обновлены.", new UserDto(user.Id, user.Login)));
});

// DELETE /user/{id} – удаление пользователя
app.MapDelete("/user/{id:int}", async (int id, UsersDbContext db) =>
{
    User? user = await db.Users.FindAsync(id);
    if (user is null)
        return Results.NotFound(new ApiResponse(false, $"Пользователь с Id={id} не найден."));

    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.Ok(new ApiResponse(true, "Пользователь удалён."));
});

app.Run();