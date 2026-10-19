namespace OrderShop.WebApi.DTO;

public class CustomerDto
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}