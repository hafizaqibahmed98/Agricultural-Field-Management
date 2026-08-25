using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BasicStructure.Services.UserService
{
    public class UserService : InterfaceUserService
    {
        private readonly IMapper _mapper;
        public DataContext _context { get; }

        public UserService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<List<GetUserDTO>>> AddUser(AddUserDTO user)
        {
            var serviceResponse = new ServiceResponse<List<GetUserDTO>>();
            try
            {
                var dbUsers = await _context.Users.ToListAsync();
                var newUser = new User()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    RoleId = user.RoleId,
                    
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                dbUsers = await _context.Users.ToListAsync();
                serviceResponse.Data = dbUsers.Select(u => _mapper.Map<GetUserDTO>(u)).ToList();
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetUserDTO>>> DeleteUser(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetUserDTO>>();
            try
            {
                var dbUsers = await _context.Users.ToListAsync();
                User oldUser = dbUsers.Find(x => x.Id == id);
                if (oldUser == null)
                    throw new Exception("User Does not Exists");
                _context.Users.Remove(oldUser);
                await _context.SaveChangesAsync();
                dbUsers = await _context.Users.ToListAsync();
                serviceResponse.Data = dbUsers.Select(user =>
                new GetUserDTO()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    RoleId = user.RoleId,
                }
               ).ToList();
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetUserDTO>>> GetAllUsers()
        {
            var serviceResponse = new ServiceResponse<List<GetUserDTO>>();
            var dbUsers = await _context.Users.ToListAsync();

            serviceResponse.Data = dbUsers.Select(user =>
                new GetUserDTO()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    RoleId = user.RoleId,
                }
            ).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetUserDTO>> GetById(int id)
        {
            var serviceResponse = new ServiceResponse<GetUserDTO>();
            try
            {
                var dbUsers = await _context.Users.ToListAsync();
                User user = dbUsers.Find(x => x.Id == id);
                if (user == null)
                    throw new Exception("User not found");
                serviceResponse.Data = new GetUserDTO()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    RoleId = user.RoleId,
                };
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetUserDTO>>> UpdateUser(int id, AddUserDTO user)
        {
            var serviceResponse = new ServiceResponse<List<GetUserDTO>>();
            var dbUsers = await _context.Users.ToListAsync();
            User oldUser = dbUsers.Find(x => x.Id == id);
            if (oldUser == null)
            {
                serviceResponse.Success = false;
                return serviceResponse;
            }
            _mapper.Map(user, oldUser);

            await _context.SaveChangesAsync();

            // Retrieve the updated users from the database after the update
            var updatedUsers = await _context.Users.ToListAsync();
            serviceResponse.Data =
                updatedUsers.Select(user =>
                new GetUserDTO()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    RoleId = user.RoleId,
                }
            ).ToList();
            serviceResponse.Message = "User updated successfully.";
            return serviceResponse;
        }
    }
}
