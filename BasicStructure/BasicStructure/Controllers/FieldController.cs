using Microsoft.AspNetCore.Authorization;
using BasicStructure.Services.FieldService;

namespace BasicFlowApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FieldController : ControllerBase
    {
        private readonly InterfaceFieldService _fieldService;
        private readonly IAuthService _authService;


        public FieldController(InterfaceFieldService fieldService, IAuthService authService)
        {
            this._fieldService = fieldService;
            this._authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<GetFieldDTO>>>> GetAllFields()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            // Decode the token
            User user = _authService.DecodeToken(token);
            bool permissionAllowed = await _authService.CheckPermission(user, 1);
            if (permissionAllowed)
            {
                return Ok(await _fieldService.GetAllFields());
            }
            return BadRequest("Forbidden");
        }

        [HttpGet("id")]
        public async Task<ActionResult<ServiceResponse<GetFieldDTO>>> GetFieldById(int id)
        {
            return Ok(await _fieldService.GetById(id));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetFieldDTO>>>> AddField(AddFieldDTO field)
        {
            return Ok(await _fieldService.AddField(field));
        }

        [HttpPut("id")]
        public async Task<ActionResult<ServiceResponse<List<GetFieldDTO>>>> UpdateFieldById(int id, AddFieldDTO field)
        {
            return Ok(await _fieldService.UpdateField(id, field));
        }

        [HttpDelete("id")]
        public async Task<ActionResult<ServiceResponse<List<GetFieldDTO>>>> DeleteFieldById(int id)
        {
            return Ok(await _fieldService.DeleteField(id));
        }

        [HttpPost("Comment")]
        public async Task<ActionResult<ServiceResponse<List<GetCommentDTO>>>> AddComment(AddCommentDTO comment)
        {
            return Ok(await _fieldService.AddComment(comment));
        }

        [HttpGet("Comment")]
        public async Task<ActionResult<ServiceResponse<List<GetCommentDTO>>>> GetAllComments()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            // Decode the token
            User user = _authService.DecodeToken(token);
            bool permissionAllowed = await _authService.CheckPermission(user, 1);
            if (permissionAllowed)
            {
                return Ok(await _fieldService.GetAllComments());
            }
            return BadRequest("Forbidden");
        }
        [HttpGet("CommentsByField")]
        public async Task<ActionResult<ServiceResponse<List<GetCommentDTO>>>> GetCommentsByFieldId(int id)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            // Decode the token
            User user = _authService.DecodeToken(token);
            bool permissionAllowed = await _authService.CheckPermission(user, 1);
            if (permissionAllowed)
            {
                return Ok(await _fieldService.GetCommentsByFieldId(id));
            }
            return BadRequest("Forbidden");
        }

        [HttpGet("CoordinatesByField")]
        public async Task<ActionResult<ServiceResponse<List<GetCoordinateDTO>>>> GetCoordinatesByFieldId(int id)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            // Decode the token
            User user = _authService.DecodeToken(token);
            bool permissionAllowed = await _authService.CheckPermission(user, 1);
            if (permissionAllowed)
            {
                return Ok(await _fieldService.GetCoordinatesByFieldId(id));
            }
            return BadRequest("Forbidden");
        }

    }
}
