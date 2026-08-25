namespace BasicStructure.Services.FieldService
{
    public interface InterfaceFieldService
    {
        Task<ServiceResponse<List<GetFieldDTO>>> GetAllFields();

        Task<ServiceResponse<GetFieldDTO>> GetById(int id);

        Task<ServiceResponse<List<GetFieldDTO>>> AddField(AddFieldDTO field);

        Task<ServiceResponse<List<GetFieldDTO>>> UpdateField(int id, AddFieldDTO field);

        Task<ServiceResponse<List<GetFieldDTO>>> DeleteField(int id);
        Task<ServiceResponse<List<GetCommentDTO>>> AddComment(AddCommentDTO comment);
        Task<ServiceResponse<List<GetCommentDTO>>> GetAllComments();
        Task<ServiceResponse<List<GetCommentDTO>>> GetCommentsByFieldId(int id);
        Task<ServiceResponse<List<GetCoordinateDTO>>> GetCoordinatesByFieldId(int id);


    }
}
