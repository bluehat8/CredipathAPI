using CredipathAPI.Helpers;
using CredipathAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CredipathAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly PermissionServices _permissionService;

        public PermissionController(PermissionServices permissionService)
        {
            _permissionService = permissionService;
        }

        // GET: api/Permission
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermissionDTO>>> GetPermissions()
        {
            var permissions = await _permissionService.GetPermissionsAsync();
            return Ok(permissions);
        }

        // GET: api/Permission/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PermissionDTO>> GetPermission(int id)
        {
            var permission = await _permissionService.GetPermissionByIdAsync(id);
            if (permission == null) return NotFound();

            return Ok(permission);
        }

        // POST: api/Permission
        [HttpPost]
        public async Task<ActionResult<PermissionDTO>> PostPermission(PermissionDTO permissionDTO)
        {
            var createdPermission = await _permissionService.CreatePermissionAsync(permissionDTO);
            return CreatedAtAction(nameof(GetPermission), new { id = createdPermission.Id }, createdPermission);
        }

        // PUT: api/Permission/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPermission(int id, PermissionDTO permissionDTO)
        {
            var success = await _permissionService.UpdatePermissionAsync(id, permissionDTO);
            if (!success) return NotFound();

            return NoContent();
        }

        // DELETE: api/Permission/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            var success = await _permissionService.DeletePermissionAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
