using BussinessObject;
using DataAccess.Repository.Interfaces;
using Lab2.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class CategoryController : Controller
{
    private readonly ICategoryRepository _repository;

    public CategoryController(ICategoryRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllCate()
    {
        try
        {
            var categories = await _repository.GetAll();
            return Ok(categories);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> AddCategory([FromBody] CategoryDto model)
    {
        try
        {
            var cate = new Category()
            {
                CategoryName = model.CategoryName
            };
            await _repository.Add(cate);
            return Ok("Category Added Successfully!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateCategory(int id, [FromBody] CategoryDto model)
    {
        try
        {
            var cate = _repository.FindById(id);
            if (cate == null)
            {
                return NotFound("Category not found");
            }

            cate.CategoryName = model.CategoryName;
            await _repository.Update(cate);
            return Ok("Category updated successfully!");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteCategory(int id)
    {
        try
        {
            var cate = _repository.FindById(id);
            if (cate == null) return NotFound("Category not found!");
            await _repository.Delete(cate);
            return Ok("Deleted Successfully!");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}