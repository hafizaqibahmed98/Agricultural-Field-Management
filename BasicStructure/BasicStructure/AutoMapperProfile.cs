namespace BasicStructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, GetUserDTO>();
            CreateMap<AddUserDTO, User>();
            CreateMap<Field, GetFieldDTO>();
            CreateMap<AddFieldDTO, Field>();
            CreateMap<Comment, GetCommentDTO>();
            CreateMap<AddCommentDTO, Comment>();
        }
    }
}
