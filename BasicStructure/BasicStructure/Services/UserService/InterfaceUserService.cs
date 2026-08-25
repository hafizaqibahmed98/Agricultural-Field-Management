namespace BasicStructure.Services.UserService
{
    public interface InterfaceUserService
    {
        Task<ServiceResponse<List<GetUserDTO>>> GetAllUsers();

        Task<ServiceResponse<GetUserDTO>> GetById(int id);

        Task<ServiceResponse<List<GetUserDTO>>> AddUser(AddUserDTO user);

        Task<ServiceResponse<List<GetUserDTO>>> UpdateUser(int id, AddUserDTO user);

        Task<ServiceResponse<List<GetUserDTO>>> DeleteUser(int id);
    }
}
