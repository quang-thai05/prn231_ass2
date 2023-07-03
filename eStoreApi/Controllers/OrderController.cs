using AutoMapper;
using BussinessObject;
using DataAccess.Repository.Interfaces;
using Lab2.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class OrderController : Controller
{
    private readonly IOrderRepository _repository;
    private readonly IOrderDetailRepository _odRepository;
    private readonly IMapper _mapper;

    public OrderController(IOrderRepository repository, IOrderDetailRepository odRepository, IMapper mapper)
    {
        _repository = repository;
        _odRepository = odRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> GetAllOrders()
    {
        try
        {
            var orders = await _repository.GetAll();
            return Ok(orders);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{userId}")]
    public ActionResult GetOrdersByUserId(string userId)
    {
        try
        {
            var orders = _repository.GetOrdersByUserId(userId);
            return Ok(orders);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("{userId}")]
    public async Task<ActionResult> AddOrder(string userId, [FromBody] OrderDto model)
    {
        try
        {
            var order = new Order()
            {
                MemberId = userId,
                OrderDate = model.OrderDate,
                RequiredDate = model.RequiredDate,
                ShippedDate = model.ShippedDate,
                Freight = model.Freight
            };
            await _repository.Add(order);
            await _odRepository.AddOrderDetails(order.OrderId, _mapper.Map<List<OrderDetail>>(model.OrderDetails));
            return Ok("Order Added successfully!");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateOrder(int id, [FromBody] OrderDto model)
    {
        try
        {
            var order = _repository.GetById(id);
            if (order == null) return NotFound("Order not found!");
            order.OrderDate = model.OrderDate;
            order.RequiredDate = model.RequiredDate;
            order.ShippedDate = model.ShippedDate;
            order.Freight = model.Freight;
            await _repository.Update(order);
            return Ok("Order Updated Successfully!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteOrder(int id)
    {
        try
        {
            var order = _repository.GetById(id);
            if (order == null) return NotFound("Order Not Found!");
            await _repository.Delete(order);
            await _odRepository.DeleteOrderDetails(id);
            return Ok("Deleted Successfully!");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}