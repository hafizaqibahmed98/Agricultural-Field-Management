using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BasicStructure.Services.FieldService
{
    public class FieldService : InterfaceFieldService
    {
        private readonly IMapper _mapper;
        public DataContext _context { get; }

        public FieldService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<List<GetFieldDTO>>> AddField(AddFieldDTO field)
        {
            var serviceResponse = new ServiceResponse<List<GetFieldDTO>>();
            try
            {
                var dbFields = await _context.Fields.ToListAsync();
                var newField = new Field()
                {
                    FieldName = field.FieldName,
                    Status = field.Status,
                    GrowerName = field.GrowerName,
                };

                _context.Fields.Add(newField);
                await _context.SaveChangesAsync();
                dbFields = await _context.Fields.ToListAsync();
                serviceResponse.Data = dbFields.Select(u => _mapper.Map<GetFieldDTO>(u)).ToList();
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetFieldDTO>>> DeleteField(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetFieldDTO>>();
            try
            {
                var dbFields = await _context.Fields.ToListAsync();
                Field oldField = dbFields.Find(x => x.Id == id);
                if (oldField == null)
                    throw new Exception("Field Does not Exists");
                _context.Fields.Remove(oldField);
                await _context.SaveChangesAsync();
                dbFields = await _context.Fields.ToListAsync();
                serviceResponse.Data = dbFields.Select(field =>
                new GetFieldDTO()
                {
                    Id = field.Id,
                    FieldName = field.FieldName,
                    Status = field.Status,
                    GrowerName = field.GrowerName,
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

        public async Task<ServiceResponse<List<GetFieldDTO>>> GetAllFields()
        {
            var serviceResponse = new ServiceResponse<List<GetFieldDTO>>();
            var dbFields = await _context.Fields.ToListAsync();

            serviceResponse.Data = dbFields.Select(field =>
                new GetFieldDTO()
                {
                    Id = field.Id,
                    FieldName = field.FieldName,
                    Status = field.Status,
                    GrowerName = field.GrowerName,
                }
            ).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetFieldDTO>> GetById(int id)
        {
            var serviceResponse = new ServiceResponse<GetFieldDTO>();
            try
            {
                var dbFields = await _context.Fields.ToListAsync();
                Field field = dbFields.Find(x => x.Id == id);
                if (field == null)
                    throw new Exception("Field not found");
                serviceResponse.Data = new GetFieldDTO()
                {
                    Id = field.Id,
                    FieldName = field.FieldName,
                    Status = field.Status,
                    GrowerName = field.GrowerName,
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

        public async Task<ServiceResponse<List<GetFieldDTO>>> UpdateField(int id, AddFieldDTO field)
        {
            var serviceResponse = new ServiceResponse<List<GetFieldDTO>>();
            var dbFields = await _context.Fields.ToListAsync();
            Field oldField = dbFields.Find(x => x.Id == id);
            if (oldField == null)
            {
                serviceResponse.Success = false;
                return serviceResponse;
            }
            _mapper.Map(field, oldField);

            await _context.SaveChangesAsync();

            // Retrieve the updated users from the database after the update
            var updatedFields = await _context.Fields.ToListAsync();
            serviceResponse.Data =
                updatedFields.Select(field =>
                new GetFieldDTO()
                {
                    Id = field.Id,
                    FieldName = field.FieldName,
                    Status = field.Status,
                    GrowerName = field.GrowerName,
                }
            ).ToList();
            serviceResponse.Message = "Field updated successfully.";
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCommentDTO>>> AddComment(AddCommentDTO comment)
        {
            var serviceResponse = new ServiceResponse<List<GetCommentDTO>>();
            try
            {
                var ServiceResponse = await GetById(comment.FieldId);
                if (!ServiceResponse.Success)
                {
                    throw new Exception("Field not found");
                }
                var dbComments = await _context.Comments.ToListAsync();
                var newComment = new Comment()
                {
                     Title = comment.Title,
                     Description = comment.Description,
                     FieldId = comment.FieldId,
                     UserId = 4,
                };

                _context.Comments.Add(newComment);
                await _context.SaveChangesAsync();
                dbComments = await _context.Comments.ToListAsync();
                serviceResponse.Data = dbComments.Select(u => _mapper.Map<GetCommentDTO>(u)).ToList();
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCommentDTO>>> GetAllComments()
        {
            var serviceResponse = new ServiceResponse<List<GetCommentDTO>>();
            var dbComments = await _context.Comments
                .Include(comment => comment.Field)
                .Include(comment => comment.User)
                .ToListAsync();

            serviceResponse.Data = dbComments.Select(comment =>
                new GetCommentDTO()
                {
                    Id = comment.Id,
                    Title = comment.Title,
                    Description = comment.Description,
                    FieldName = comment.Field.FieldName,
                    UserName = comment.User.FirstName,
                }
            ).ToList();
            return serviceResponse;
        }
        public async Task<ServiceResponse<List<GetCommentDTO>>> GetCommentsByFieldId(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCommentDTO>>();
            try
            {
                var dbFields = await _context.Fields.ToListAsync();
                Field field = dbFields.Find(x => x.Id == id);
                if (field == null)
                    throw new Exception("Field not found");
                var dbComments = await _context.Comments
                    .Where(comment => comment.FieldId == id)
                    .Include(comment => comment.Field)
                    .Include(comment => comment.User)
                    .ToListAsync();

                serviceResponse.Data = dbComments.Select(comment =>
                    new GetCommentDTO()
                    {
                        Id = comment.Id,
                        Title = comment.Title,
                        Description = comment.Description,
                        FieldName = comment.Field.FieldName,
                        UserName = comment.User.FirstName,
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
        public async Task<ServiceResponse<List<GetCoordinateDTO>>> GetCoordinatesByFieldId(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCoordinateDTO>>();
            try
            {
                var dbFields = await _context.Fields.ToListAsync();
                Field field = dbFields.Find(x => x.Id == id);
                if (field == null)
                    throw new Exception("Field not found");
                var dbCoordinates = await _context.Coordinates
                    .Where(coordinate => coordinate.FieldId == id)
                    .Include(coordinate => coordinate.Field)
                    .OrderBy(coordinate => coordinate.SequenceNumber)
                    .ToListAsync();

                serviceResponse.Data = dbCoordinates.Select(coordinate =>
                    new GetCoordinateDTO()
                    {
                        SequenceNumber = coordinate.SequenceNumber,
                        Latitude = coordinate.Latitude,
                        Longitude = coordinate.Longitude,
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

    }
}
