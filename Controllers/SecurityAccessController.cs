using FinAxisLeaseBudgeting.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanningAPI.Models;

namespace FinAxisLeaseBudgeting.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SecurityAccessController : ControllerBase
    {

        private readonly FinAxisDbContext _context;

        public SecurityAccessController(FinAxisDbContext context)
        {
            _context = context;
        }

        [HttpGet("role-permissions/{roleId}")]
        public async Task<ActionResult<RolePermissionsResponse>> GetRolePermissions(int roleId)
        {
            var role = await _context.Roles
                .Include(r => r.ScreenPermissions)
                .Include(r => r.FieldPermissions)
                .FirstOrDefaultAsync(r => r.RoleId == roleId);

            if (role == null)
                return NotFound("Role not found");

            var response = new RolePermissionsResponse
            {
                RoleId = role.RoleId
            };

            foreach (var screen in role.ScreenPermissions)
            {
                response.Screens[screen.ScreenCode] = new PermissionAction
                {
                    View = screen.CanView,
                    Edit = screen.CanEdit
                };
            }

            foreach (var field in role.FieldPermissions)
            {
                response.Fields[field.FieldCode] = new PermissionAction
                {
                    View = field.CanView,
                    Edit = field.CanEdit
                };
            }

            return Ok(response);
        }

        [HttpGet("GetUserPermissions/{userId}")]
        public async Task<ActionResult<PermissionResponse>> GetUserPermissions(int userId)
        {
            var user = await _context.Users
                .Include(u => u.UserRole)
                    .ThenInclude(r => r.ScreenPermissions)
                .Include(u => u.UserRole)
                    .ThenInclude(r => r.FieldPermissions)
                .Include(u => u.ScreenOverrides)
                .Include(u => u.FieldOverrides)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return NotFound();

            var response = new PermissionResponse();

            // Screens
            foreach (var roleScreen in user.UserRole.ScreenPermissions)
            {
                var userOverride = user.ScreenOverrides
                    .FirstOrDefault(x => x.ScreenCode == roleScreen.ScreenCode);

                response.Screens[roleScreen.ScreenCode] = new PermissionAction
                {
                    View = PermissionResolver.Resolve(userOverride?.CanView, roleScreen.CanView),
                    Edit = PermissionResolver.Resolve(userOverride?.CanEdit, roleScreen.CanEdit)
                };
            }

            // Fields
            foreach (var roleField in user.UserRole.FieldPermissions)
            {
                var userOverride = user.FieldOverrides
                    .FirstOrDefault(x => x.FieldCode == roleField.FieldCode);

                response.Fields[roleField.FieldCode] = new PermissionAction
                {
                    View = PermissionResolver.Resolve(userOverride?.CanView, roleField.CanView),
                    Edit = PermissionResolver.Resolve(userOverride?.CanEdit, roleField.CanEdit)
                };
            }

            return Ok(response);
        }

        [HttpGet("GetUserPermissionsV1/{userId}")]
        public async Task<ActionResult<PermissionResponse>> GetUserPermissionsV1(int userId)
        {
            var user = await _context.Users
                .Include(u => u.UserRole)
                    .ThenInclude(r => r.ScreenPermissions)
                .Include(u => u.UserRole)
                    .ThenInclude(r => r.FieldPermissions)
                .Include(u => u.ScreenOverrides)
                .Include(u => u.FieldOverrides)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return NotFound();

            var response = new PermissionResponse();

            var allScreenCodes = user.UserRole.ScreenPermissions
                .Select(x => x.ScreenCode)
                .Union(user.ScreenOverrides.Select(x => x.ScreenCode));

            foreach (var screenCode in allScreenCodes)
            {
                var rolePermission = user.UserRole.ScreenPermissions
                    .FirstOrDefault(x => x.ScreenCode == screenCode);

                var userPermission = user.ScreenOverrides
                    .FirstOrDefault(x => x.ScreenCode == screenCode);

                response.Screens[screenCode] = new PermissionAction
                {
                    View = PermissionResolver.Resolve(
                        userPermission?.CanView,
                        rolePermission?.CanView ?? false),

                    Edit = PermissionResolver.Resolve(
                        userPermission?.CanEdit,
                        rolePermission?.CanEdit ?? false)
                };
            }

            var allFieldCodes = user.UserRole.FieldPermissions
    .Select(x => x.FieldCode)
    .Union(user.FieldOverrides.Select(x => x.FieldCode));

            foreach (var fieldCode in allFieldCodes)
            {
                var rolePermission = user.UserRole.FieldPermissions
                    .FirstOrDefault(x => x.FieldCode == fieldCode);

                var userPermission = user.FieldOverrides
                    .FirstOrDefault(x => x.FieldCode == fieldCode);

                response.Fields[fieldCode] = new PermissionAction
                {
                    View = PermissionResolver.Resolve(
                        userPermission?.CanView,
                        rolePermission?.CanView ?? false),

                    Edit = PermissionResolver.Resolve(
                        userPermission?.CanEdit,
                        rolePermission?.CanEdit ?? false)
                };
            }

            return Ok(response);
        }

        [NonController]
        public static class PermissionResolver
        {
            public static bool Resolve(bool? userValue, bool roleValue)
            {
                return userValue ?? roleValue;
            }
        }
        [HttpPost("user-settings")]
        public async Task<IActionResult> SetUserSettings([FromBody] UserSettingsRequest request)
        {
            var user = await _context.Users
                .Include(u => u.ScreenOverrides)
                .Include(u => u.FieldOverrides)
                .FirstOrDefaultAsync(u => u.UserId == request.UserId);

            if (user == null)
                return NotFound("User not found");

            // -----------------------------
            // SCREEN OVERRIDES
            // -----------------------------
            if (request.Screens != null)
            {
                foreach (var screen in request.Screens)
                {
                    var existing = user.ScreenOverrides
                        .FirstOrDefault(x => x.ScreenCode == screen.Key);

                    if (existing == null)
                    {
                        user.ScreenOverrides.Add(new UserScreenPermission
                        {
                            UserId = request.UserId,
                            ScreenCode = screen.Key,
                            CanView = screen.Value.View,
                            CanEdit = screen.Value.Edit
                        });
                    }
                    else
                    {
                        existing.CanView = screen.Value.View;
                        existing.CanEdit = screen.Value.Edit;
                    }
                }
            }

            // -----------------------------
            // FIELD OVERRIDES
            // -----------------------------
            if (request.Fields != null)
            {
                foreach (var field in request.Fields)
                {
                    var existing = user.FieldOverrides
                        .FirstOrDefault(x => x.FieldCode == field.Key);

                    if (existing == null)
                    {
                        user.FieldOverrides.Add(new UserFieldPermission
                        {
                            UserId = request.UserId,
                            FieldCode = field.Key,
                            CanView = field.Value.View,
                            CanEdit = field.Value.Edit
                        });
                    }
                    else
                    {
                        existing.CanView = field.Value.View;
                        existing.CanEdit = field.Value.Edit;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok("User settings updated successfully");
        }

        [HttpPost("role-settings")]
        public async Task<IActionResult> SetRoleSettings([FromBody] RoleSettingsRequest request)
        {
            var role = await _context.Roles
                .Include(r => r.ScreenPermissions)
                .Include(r => r.FieldPermissions)
                .FirstOrDefaultAsync(r => r.RoleId == request.RoleId);

            if (role == null)
                return NotFound("Role not found");

            // -----------------------------
            // SCREEN PERMISSIONS
            // -----------------------------
            if (request.Screens != null)
            {
                foreach (var screen in request.Screens)
                {

                    if (screen.Key == "financialReport")
                    {

                    }

                    var existing = role.ScreenPermissions
                        .FirstOrDefault(x => x.ScreenCode == screen.Key);

                    if (existing == null)
                    {
                        role.ScreenPermissions.Add(new RoleScreenPermission
                        {
                            RoleId = request.RoleId,
                            ScreenCode = screen.Key,
                            CanView = screen.Value.View,
                            CanEdit = screen.Value.Edit
                        });
                    }
                    else
                    {
                        //if (screen.Value.View)
                        existing.CanView = screen.Value.View;

                        //if (screen.Value.Edit)
                        existing.CanEdit = screen.Value.Edit;
                    }
                }
            }

            // -----------------------------
            // FIELD PERMISSIONS
            // -----------------------------
            if (request.Fields != null)
            {
                foreach (var field in request.Fields)
                {
                    var existing = role.FieldPermissions
                        .FirstOrDefault(x => x.FieldCode == field.Key);

                    if (existing == null)
                    {
                        role.FieldPermissions.Add(new RoleFieldPermission
                        {
                            RoleId = request.RoleId,
                            FieldCode = field.Key,
                            CanView = field.Value.View,
                            CanEdit = field.Value.Edit
                        });
                    }
                    else
                    {
                        if (field.Value.View)
                            existing.CanView = field.Value.View;

                        if (field.Value.Edit)
                            existing.CanEdit = field.Value.Edit;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok("Role settings updated successfully");
        }

        [HttpPost("role-permissions/bulk")]
        public async Task<IActionResult> BulkImportRolePermissions(
        [FromBody] BulkRolePermissionRequest request)
        {
            var role = await _context.Roles
                .Include(r => r.ScreenPermissions)
                .Include(r => r.FieldPermissions)
                .FirstOrDefaultAsync(r => r.RoleId == request.RoleId);

            if (role == null)
                return NotFound("Role not found");

            // -----------------------------
            // BULK SCREENS
            // -----------------------------
            if (request.Screens != null)
            {
                foreach (var screen in request.Screens)
                {
                    var existing = role.ScreenPermissions
                        .FirstOrDefault(x => x.ScreenCode == screen.ScreenCode);

                    if (existing == null)
                    {
                        role.ScreenPermissions.Add(new RoleScreenPermission
                        {
                            RoleId = request.RoleId,
                            ScreenCode = screen.ScreenCode,
                            CanView = screen.CanView,
                            CanEdit = screen.CanEdit
                        });
                    }
                    else
                    {
                        existing.CanView = screen.CanView;
                        existing.CanEdit = screen.CanEdit;
                    }
                }
            }

            // -----------------------------
            // BULK FIELDS
            // -----------------------------
            if (request.Fields != null)
            {
                foreach (var field in request.Fields)
                {
                    var existing = role.FieldPermissions
                        .FirstOrDefault(x => x.FieldCode == field.FieldCode);

                    if (existing == null)
                    {
                        role.FieldPermissions.Add(new RoleFieldPermission
                        {
                            RoleId = request.RoleId,
                            FieldCode = field.FieldCode,
                            CanView = field.CanView,
                            CanEdit = field.CanEdit
                        });
                    }
                    else
                    {
                        existing.CanView = field.CanView;
                        existing.CanEdit = field.CanEdit;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok("Bulk role permissions imported successfully");
        }

        [HttpPost("user-permissions/bulk")]
        public async Task<IActionResult> BulkImportUserPermissions(
        [FromBody] BulkUserPermissionRequest request)
        {
            var user = await _context.Users
                .Include(u => u.ScreenOverrides)
                .Include(u => u.FieldOverrides)
                .FirstOrDefaultAsync(u => u.UserId == request.UserId);

            if (user == null)
                return NotFound("User not found");

            // -----------------------------
            // BULK SCREEN OVERRIDES
            // -----------------------------
            if (request.Screens != null)
            {
                foreach (var screen in request.Screens)
                {
                    var existing = user.ScreenOverrides
                        .FirstOrDefault(x => x.ScreenCode == screen.ScreenCode);

                    if (existing == null)
                    {
                        user.ScreenOverrides.Add(new UserScreenPermission
                        {
                            UserId = request.UserId,
                            ScreenCode = screen.ScreenCode,
                            CanView = screen.CanView,
                            CanEdit = screen.CanEdit
                        });
                    }
                    else
                    {
                        existing.CanView = screen.CanView;
                        existing.CanEdit = screen.CanEdit;
                    }
                }
            }

            // -----------------------------
            // BULK FIELD OVERRIDES
            // -----------------------------
            if (request.Fields != null)
            {
                foreach (var field in request.Fields)
                {
                    var existing = user.FieldOverrides
                        .FirstOrDefault(x => x.FieldCode == field.FieldCode);

                    if (existing == null)
                    {
                        user.FieldOverrides.Add(new UserFieldPermission
                        {
                            UserId = request.UserId,
                            FieldCode = field.FieldCode,
                            CanView = field.CanView,
                            CanEdit = field.CanEdit
                        });
                    }
                    else
                    {
                        existing.CanView = field.CanView;
                        existing.CanEdit = field.CanEdit;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok("Bulk user permissions imported successfully");
        }
    }
}