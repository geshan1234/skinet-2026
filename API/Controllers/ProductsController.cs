using System;
using API.RequestHelpers;
using AutoMapper;
using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class ProductsController(IGenericRepository<Product> repo) : BaseApiController
{

    [HttpGet] // api/products
    public async Task<ActionResult<IReadOnlyList<Product>>>  GetProducts([FromQuery]ProductSpecParams specParams)
    {
        var spec = new ProductSpecification(specParams);

        return await CreatePagedResult<Product>(repo, spec, specParams.PageIndex, specParams.PageSize);
    }

    [HttpGet("types")] // api/products/types
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        var spec = new TypeListSpecification();
        var types = await repo.ListAsync(spec);
        return Ok(types);
    }

    [HttpGet("brands")] // api/products/brands
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        var spec = new BrandListSpecification();
        var brands = await repo.ListAsync(spec);
        return Ok(brands);
    }

    [HttpGet("{id:int}")] // api/products/5
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await repo.GetByIdAsync(id);
        if (product is null) return NotFound();

        return product;
    }

    [HttpPost] // api/products
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        repo.Add(product);
        if (await repo.SaveAllAsync())
        {
            return CreatedAtAction("GetProduct", new {id = product.Id}, product);
        }

        return BadRequest("Problem when creating product"); 
    }

    [HttpPut("{id:int}")] // api/products/5
    public async Task<ActionResult> UpdateProduct(int id, [FromBody]Product product)
    {
        if (id != product.Id || !await ProductExist(id))
            return BadRequest("cannot update this product!");

        repo.Update(product);
        if (await repo.SaveAllAsync())
        {
            return CreatedAtAction("GetProduct", new{id = product.Id}, product);
        }

        return BadRequest("Problem when updating the product");
    }

    [HttpDelete("{id:int}")] // api/products/5
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await repo.GetByIdAsync(id);
        if (product is null) return NotFound();

        repo.Remove(product);

        if (await repo.SaveAllAsync())
        {
            return NoContent();
        }

        return BadRequest("Problem when deleting the product");
    }

    private async Task<bool> ProductExist(int id)
    {
        return await repo.Exists(id);
    }
}
