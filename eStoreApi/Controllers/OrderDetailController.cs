using AutoMapper;
using BussinessObject;
using DataAccess.Repository.Interfaces;
using Lab2.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class OrderDetailController : Controller
{
    private readonly IOrderDetailRepository _repository;
    private readonly IMapper _mapper;

    public OrderDetailController(IOrderDetailRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllOrderDetail()
    {
        try
        {
            var orderDetail = await _repository.GetAll();
            return Ok(orderDetail);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult> AddOrderDetails([FromBody] List<OrderDetailDto> models)
    {
        try
        {
            await _repository.AddOrderDetails(_mapper.Map<List<OrderDetail>>(models));
            return Ok("Order Detail Added Successfully");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteOrderDetails(int id)
    {
        try
        {
            var orderDetails = _repository.GetByOrderId(id);
            if (orderDetails.Count == 0) return NotFound("Don't have order details for this order");
            await _repository.DeleteOrderDetails(orderDetails);
            return Ok("deleted successfully!");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}