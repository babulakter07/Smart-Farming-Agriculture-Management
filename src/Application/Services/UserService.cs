using Firming_Solution.Application.DTOs;
using Firming_Solution.Domain.Entities;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Application.Services;

public class UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
{
    public async Task<List<UserListDto>> GetAllAsync(CancellationToken ct = default)
    {
        var users = await db.Users
            .Where(u => !u.IsDeleted)
            .Include(u => u.UserFarms).ThenInclude(uf => uf.Farm)
            .AsNoTracking()
            .ToListAsync(ct);

        return users.Select(u => new UserListDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            Role = u.Role,
            IsActive = u.IsActive,
            CreatedDate = u.CreatedDate,
            AssignedFarms = u.UserFarms.Where(uf => !uf.IsDeleted).Select(uf => uf.Farm.FarmName).ToList()
        }).ToList();
    }

    public async Task<(bool Success, IEnumerable<string> Errors)> CreateAsync(UserCreateDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            Role = dto.Role,
            IsActive = true
        };
        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description));

        await userManager.AddToRoleAsync(user, dto.Role.ToString());

        foreach (var farmId in dto.FarmIds)
            db.UserFarms.Add(new UserFarm { UserId = user.Id, FarmId = farmId });

        await db.SaveChangesAsync();
        return (true, []);
    }

    public async Task<(bool Success, IEnumerable<string> Errors)> UpdateAsync(UserEditDto dto)
    {
        var user = await userManager.FindByIdAsync(dto.Id);
        if (user == null) return (false, ["User not found"]);

        user.FullName = dto.FullName;
        user.Email = dto.Email;
        user.UserName = dto.Email;
        user.IsActive = dto.IsActive;

        if (user.Role != dto.Role)
        {
            await userManager.RemoveFromRoleAsync(user, user.Role.ToString());
            user.Role = dto.Role;
            await userManager.AddToRoleAsync(user, dto.Role.ToString());
        }

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description));

        // Sync farm assignments
        var existing = await db.UserFarms.Where(uf => uf.UserId == dto.Id).ToListAsync();
        foreach (var uf in existing) { uf.IsDeleted = true; }
        foreach (var farmId in dto.FarmIds)
        {
            var existing2 = existing.FirstOrDefault(e => e.FarmId == farmId);
            if (existing2 != null) existing2.IsDeleted = false;
            else db.UserFarms.Add(new UserFarm { UserId = user.Id, FarmId = farmId });
        }
        await db.SaveChangesAsync();
        return (true, []);
    }

    public async Task<bool> SoftDeleteAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return false;
        user.IsDeleted = true;
        user.IsActive = false;
        await userManager.UpdateAsync(user);
        return true;
    }

    public async Task<UserEditDto?> GetEditDtoAsync(string id, CancellationToken ct = default)
    {
        var u = await db.Users.Include(x => x.UserFarms).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (u == null) return null;
        return new UserEditDto
        {
            Id = u.Id,
            FullName = u.FullName ?? "",
            Email = u.Email ?? "",
            Role = u.Role,
            IsActive = u.IsActive,
            FarmIds = u.UserFarms.Where(uf => !uf.IsDeleted).Select(uf => uf.FarmId).ToList()
        };
    }
}
