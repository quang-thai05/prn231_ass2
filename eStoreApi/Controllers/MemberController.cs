using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BussinessObject;
using DataAccess.DataContext;
using Lab2.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Lab2.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class MemberController : Controller
{
    private readonly IConfiguration _config;
    private UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _signInManager;

    public MemberController(IConfiguration config, UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager)
    {
        _config = config;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
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