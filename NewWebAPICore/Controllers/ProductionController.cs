using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewWebAPICore.DTO_s;
using NewWebAPICore.Filters;
using System.Security.Claims;
using WebAPICore.Data;
using WebAPICore.Model;

[Route("api/product")]
[Authorize]
[ApiController]
[ServiceFilter(typeof(AuditLogFilter))]
[ServiceFilter(typeof(CustomResultFilter))]
[ServiceFilter(typeof(ExceptionFilter))]
public class ProductionController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductionController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!TryGetUserId(out int userId))
        {
            return Unauthorized(new APIResponseDTO_s<string>(
                false, "User is not authenticated", null));
        }

        var products = await _context.Productions.Where(p => p.UserId == userId).Select(pp=> new CreateproductDTO
                                                    {
                                                        Id = pp.Id,
                                                        Name = pp.Name,
                                                        Price = pp.Price
                                                    }).ToListAsync();

        return Ok(new APIResponseDTO_s<IEnumerable<CreateproductDTO>>(
            true, "Products fetched successfully", products));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (!TryGetUserId(out int userId))
        {
            return Unauthorized(new APIResponseDTO_s<string>(
                false, "User is not authenticated", null));
        }

        var product = await _context.Productions.
            Where(p => p.UserId == userId && p.Id == id).Select(pp => new CreateproductDTO
            {
                Id = pp.Id,
                Name = pp.Name,
                Price = pp.Price
            }).SingleAsync();

        if (product == null)
        {
            return NotFound(new APIResponseDTO_s<string>(
                false, "Product not found", null));
        }

        return Ok(new APIResponseDTO_s<CreateproductDTO>(
            true, "Product fetched successfully", product));
    }

    [Authorize(Roles = "Admin,Manager,User")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateproductDTO dto)
    {
        if (!TryGetUserId(out int userId))
        {
            return Unauthorized(new APIResponseDTO_s<string>(
                false, "User is not authenticated", null));
        }

        if (dto.Price <= 0)
        {
            return BadRequest(new APIResponseDTO_s<string>(
                false, "Price must be greater than 0", null));
        }

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest(new APIResponseDTO_s<string>(
                false, "Product name is required", null));
        }

        var product = new Production
        {
            Name = dto.Name,
            Price = dto.Price,
            UserId = userId
        };

        await _context.Productions.AddAsync(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
             new CreateproductDTO { Id = product.Id },
            new APIResponseDTO_s<CreateproductDTO>(
                true, "Product created successfully", new CreateproductDTO { Id = product.Id, Name = product.Name, Price = product.Price }));
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CreateproductDTO product)
    {
        if (!TryGetUserId(out int userId))
        {
            return Unauthorized(new APIResponseDTO_s<string>(
                false, "User is not authenticated", null));
        }

        if (id != product.Id)
        {
            return BadRequest(new APIResponseDTO_s<string>(
                false, "Product ID mismatch", null));
        }

        var dbProduct = await _context.Productions
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (dbProduct == null)
        {
            return NotFound(new APIResponseDTO_s<string>(
                false, "Product not found", null));
        }

        dbProduct.Name = product.Name;
        dbProduct.Price = product.Price;

        await _context.SaveChangesAsync();

        return Ok(new APIResponseDTO_s<CreateproductDTO>(
            true, "Product updated successfully", new CreateproductDTO { Id = dbProduct.Id, Name = dbProduct.Name, Price = dbProduct.Price }));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!TryGetUserId(out int userId))
        {
            return Unauthorized(new APIResponseDTO_s<string>(
                false, "User is not authenticated", null));
        }

        var product = await _context.Productions
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (product == null)
        {
            return NotFound(new APIResponseDTO_s<string>(
                false, "Product not found", null));
        }

        _context.Productions.Remove(product);
        await _context.SaveChangesAsync();

        return Ok(new APIResponseDTO_s<string>(
            true, "Product deleted successfully", null));
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, CreateproductDTO p)
    {
        if (!TryGetUserId(out int userId))
        {
            return Unauthorized(new APIResponseDTO_s<string>(
                false, "User is not authenticated", null));
        }

        if (id != p.Id)
        {
            return BadRequest(new APIResponseDTO_s<string>(
                false, "Product ID mismatch", null));
        }

        var product = await _context.Productions
            .FirstOrDefaultAsync(pp => pp.Id == p.Id && pp.UserId == userId);

        if (product == null)
        {
            return NotFound(new APIResponseDTO_s<string>(
                false, "Product not found", null));
        }

            if (!string.IsNullOrWhiteSpace(p.Name))
                product.Name = p.Name;

            if (p.Price <= 0)
            {
                return BadRequest(new APIResponseDTO_s<string>(
                    false, "Price must be greater than 0", null));
            }

            product.Price = p.Price;

        await _context.SaveChangesAsync();

        return Ok(new APIResponseDTO_s<CreateproductDTO>(
            true, "Product updated successfully", new CreateproductDTO { Id = product.Id, Name = product.Name, Price = product.Price }));
    }


    private bool TryGetUserId(out int userId)
    {
        userId = 0;
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null && int.TryParse(claim.Value, out userId);
    }

    //[HttpGet("exception")]
    //public IActionResult ThrowException()
    //{
    //    throw new Exception("This is a test exception from API");
    //}

    [HttpGet("version")]
    public IActionResult Version()
    {
        return Ok("DEPLOY TEST SUCCESS" + DateTime.Now);
    }

    [HttpGet("CheckDateTime")]
    public IActionResult Datetime()
    {
        return Ok("Current Time" + DateTime.Now);
    }

    [HttpGet("CheckAPI")]
    public IActionResult checkapi()
    {
        return Ok("API Working");
    }
}
