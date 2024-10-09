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
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Diagnostics.SymbolStore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using CredipathAPI.Services;

namespace CredipathAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewExpectedvsRealityController : ControllerBase
    {
        private readonly ViewExpectedvsRealityService _viewExpectedvsReality;
        public ViewExpectedvsRealityController(ViewExpectedvsRealityService viewExpectedvsRealityService)
        {
            _viewExpectedvsReality = viewExpectedvsRealityService;
        }

        // GET: api/Route/5
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<Model.ViewExpectedvsReality>>> GetViewExpectedvsRealityService()
        {
            var View = await _viewExpectedvsReality.GetViewExpectedvsRealityAsync();
            return Ok(View);
        }

    }
}
