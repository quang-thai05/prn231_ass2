using AutoMapper;
using BussinessObject;
using DataAccess.DataContext;
using DataAccess.Repository.Interfaces;
using Lab2.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class OrderController : Controller
{
    private readonly EStoreDbContext _context;
    private readonly IOrderRepository _repository;
    private readonly IOrderDetailRepository _odRepository;
    private readonly IMapper _mapper;

    public OrderController(EStoreDbContext context, IOrderRepository repository, IOrderDetailRepository odRepository,
        IMapper mapper)
    {
        _context = context;
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
    [Authorize(Roles = "User")]
    public ActionResult GetOrdersByUserId(string userId)
    {
        try
        {
            var orders = _repository.GetOrdersByUserId(userId);
            var result = orders
                .Select(x => new
                {
                    OrderId = x.OrderId,
                    OrderDate = $"{x.OrderDate:dd MMM, yyyy}",
                    RequiredDate = $"{x.RequiredDate: dd MMM, yyyy}",
                    ShippedDate = $"{x.ShippedDate: dd MMM, yyyy}",
                    Freight = x.Freight
                }).ToList();
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("{userId}")]
    [Authorize(Roles = "User")]
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
    [Authorize(Roles = "User")]
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
    [Authorize(Roles = "User")]
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

    [HttpGet("{fromDate:datetime}/{toDate:datetime}")]
    public IActionResult GetSaleReport(DateTime fromDate, DateTime toDate)
    {
        try
        {
            var result = _context.OrderDetails
                .Join(_context.Orders,
                    od => od.OrderId,
                    o => o.OrderId,
                    (od, o) => new { OrderDetail = od, Order = o })
                .Join(
                    _context.Products,
                    od => od.OrderDetail.ProductId,
                    p => p.ProductId,
                    (od, p) => new { od.OrderDetail, od.Order, Product = p }
                )
                .Where(x => x.Order.OrderDate >= fromDate && x.Order.OrderDate <= toDate)
                .GroupBy(
                    x => new
                    {
                        OrderDate = x.Order.OrderDate,
                        ProductName = x.Product.ProductName,
                        UnitPrice = x.OrderDetail.UnitPrice
                    })
                .Select(g => new
                {
                    OrderDate = g.Key.OrderDate,
                    ProductName = g.Key.ProductName,
                    UnitPrice = g.Key.UnitPrice,
                    Quantity = g.Sum(x => x.OrderDetail.Quantity),
                    Sales = g.Sum(x => x.OrderDetail.Quantity) * g.Key.UnitPrice
                })
                .OrderByDescending(x => x.OrderDate)
                .ThenByDescending(x => x.Sales)
                .ToList();

            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}