using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BussinessObject;
using DataAccess.DataContext;
using Lab2.DTOs;
using Lab2.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Lab2.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class MemberController : Controller
{
    private readonly IConfiguration _config;
    private readonly EStoreDbContext _context;
    private UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _signInManager;

    public MemberController(IConfiguration config, UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, EStoreDbContext context)
    {
        _config = config;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult> Login([FromBody] LoginDto model)
    {
        if (model.Email.Equals(_config["AdminAcc:Email"]))
        {
            if (!model.Password.Equals(_config["AdminAcc:Password"])) return Unauthorized();
            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, _config["AdminAcc:Email"]),
                new(ClaimTypes.Name, _config["AdminAcc:Username"]),
                new(ClaimTypes.Role, "Admin"),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var adminToken = GetToken(claims);
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(adminToken),
                expiration = adminToken.ValidTo
            });
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password)) return Unauthorized();
        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            new("UserId", user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

        var token = GetToken(authClaims);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo
        });
    }

    [HttpPost]
    public async Task<ActionResult> Registration([FromBody] RegisterDto model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists != null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Status = "Error", Message = "User already exists!" });
        }

        var user = new User()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Status = "Error", Message = "User creation failed! Please check user details and try again." });
        }

        if (!await _roleManager.RoleExistsAsync("User"))
        {
            await _roleManager.CreateAsync(new IdentityRole("User"));
        }

        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        if (await _roleManager.RoleExistsAsync("User"))
        {
            await _userManager.AddToRoleAsync(user, "User");
        }

        return Ok(new { Status = "Success", Message = "User created successfully!" });
    }

    [HttpGet]
    public async Task<ActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    [HttpGet]
    public ActionResult GetCurrentUser()
    {
        try
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity is null) return NotFound();

            var userClaims = identity.Claims;
            var name = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value;
            var email = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value;
            var userId = userClaims.FirstOrDefault(o => o.Type == "UserId")?.Value;
            var role = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value;

            return Ok(new
            {
                username = name,
                email = email,
                userId = userId,
                role = role
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{userId}")]
    [Authorize(Roles = "User")]
    public ActionResult GetUserProfile(string userId)
    {
        try
        {
            var user = _context.Users.FirstOrDefault(x => x.Id.Equals(userId));
            if (user is null) return NotFound();
            return Ok(new
            {
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber
            });
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpPut("{userId}")]
    [Authorize(Roles = "User")]
    public ActionResult UpdateUserProfile(string userId, [FromBody] UserProfileDto model)
    {
        try
        {
            var user = _context.Users.FirstOrDefault(x => x.Id.Equals(userId));
            if (user is null) return NotFound();
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.Phone;
            _context.Update(user);
            var result = _context.SaveChanges();
            if (result == 0) return BadRequest("Update Failed");
            return Ok("Updated Successfully!");
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Issuer"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}