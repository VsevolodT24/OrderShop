using System.Text.RegularExpressions;

namespace OrderShop.UsersApi;

public static class Validation
{
    private static readonly Regex LoginRegex = new(@"^[A-Za-z0-9_.\-]{3,50}$", RegexOptions.Compiled);
    
    // SHA-256 в шестнадцатеричной записи: ровно 64 символа 0-9 a-f
    private static readonly Regex Sha256Regex = new(@"^[0-9a-fA-F]{64}$", RegexOptions.Compiled);

    public static List<string> Validate(UserRequest? request)
    {
        var errors = new List<string>();
        
        if (request is null)
        {
            errors.Add("Тело запроса пустое или не является корректным JSON.");
            return errors;
        }

        if (string.IsNullOrWhiteSpace(request.Login))
            errors.Add("Поле Login обязательно.");
        else if (!LoginRegex.IsMatch(request.Login.Trim()))
            errors.Add("Login: от 3 до 50 символов, допустимы латинские буквы, цифры и символы _ . -");

        if (string.IsNullOrWhiteSpace(request.PassHash))
            errors.Add("Поле PassHash обязательно.");
        else if (!Sha256Regex.IsMatch(request.PassHash))
            errors.Add("PassHash должен быть хешем SHA-256 в шестнадцатеричном виде (64 символа). Пароль в открытом виде передавать нельзя.");

        return errors;
    }
}