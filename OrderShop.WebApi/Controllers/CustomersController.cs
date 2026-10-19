using Microsoft.AspNetCore.Mvc;
using OrderShop.BLL.Interfaces;

namespace OrderShop.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _service;

    public CustomersController(ICustomerService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_service.GetAllCustomers());
}