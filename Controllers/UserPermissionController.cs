using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CredipathAPI.Data;
using CredipathAPI.Model;
using CredipathAPI.Helpers;
using CredipathAPI.Services;

namespace CredipathAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPermissionController : ControllerBase
    {
        private readonly UserPermissionService _userPermissionService;

        public UserPermissionController(UserPermissionService userPermissionService)
        {
            _userPermissionService = userPermissionService;
        }

        // GET: api/UserPermission
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserPermissionDTO>>> GetUserPermissions()
        {
            var userPermissions = await _userPermissionService.GetUserPermissionsAsync();
            return Ok(userPermissions);
        }

        // GET: api/UserPermission/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserPermissionDTO>> GetUserPermission(int id)
        {
            var userPermission = await _userPermissionService.GetUserPermissionByIdAsync(id);
            if (userPermission == null) return NotFound();

            return Ok(userPermission);
        }

        // POST: api/UserPermission
        [HttpPost]
        public async Task<ActionResult<UserPermissionDTO>> PostUserPermission(UserPermissionDTO userPermissionDTO)
        {
            var createdUserPermission = await _userPermissionService.CreateUserPermissionAsync(userPermissionDTO);
            return CreatedAtAction(nameof(GetUserPermission), new { id = createdUserPermission.Id }, createdUserPermission);
        }

        // PUT: api/UserPermission/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserPermission(int id, UserPermissionDTO userPermissionDTO)
        {
            var success = await _userPermissionService.UpdateUserPermissionAsync(id, userPermissionDTO);
            if (!success) return NotFound();

            return NoContent();
        }

        // DELETE: api/UserPermission/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserPermission(int id)
        {
            var success = await _userPermissionService.DeleteUserPermissionAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
