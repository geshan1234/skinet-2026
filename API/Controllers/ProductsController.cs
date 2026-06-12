using System;
using AutoMapper;
using Core.Dtos;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly StoreContext _context;
    private readonly IMapper _mapper;

    public ProductsController(StoreContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet] // api/products
    public async Task<ActionResult<IEnumerable<Product>>>  GetProducts()
    {
        return await _context.Products.ToListAsync();
    }

    [HttpGet("{id:int}")] // api/products/5
    public async Task<ActionResult<Product>> GetProductById(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound();

        return product;
    }

    [HttpPost] // api/products
    public async Task<ActionResult<Product>> CreateProduct(ProductCreationDto productDto)
    {
        var product = _mapper.Map<Product>(productDto);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product; 

    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, [FromBody]Product product)
    {
        if (id != product.Id || !isProductExisting(id))
            return BadRequest("cannot update this product!");

        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> ActionResult(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound();

        _context.Products.Remove(product);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    private bool isProductExisting(int id)
    {
        return _context.Products.Any(p => p.Id == id);
    }
}
