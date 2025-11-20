using System.Xml.XPath;
using MicroServicioUser.Dom.Entities;
using MicroServicioUser.Dom.Patterns;
using MicroServicioUser.Dom.Interfaces;
using MicroServicoUser.Inf.Persistence.Database;

namespace MicroServicoUser.Inf.Repository
{
    public class IdDto
    {
        public int Id { get; set; }
    }
    public class UserRepository : IRepository
    {
        private readonly MySqlConnectionManager _dbConnectionManager;
        public UserRepository(MySqlConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        public async Task<Result<IEnumerable<User>>> GetAll()
        {
            string query = @"
                SELECT
                    id            AS Id,
                    username      AS Username,
                    password_hash AS PasswordHash,
                    first_name    AS FirstName,
                    second_name   AS SecondName,
                    first_last_name     AS LastName,
                    second_last_name AS SecondLastName,
                    email         AS Email,
                    role          AS Role,
                    created_by    AS CreatedBy,
                    created_date  AS CreatedDate,
                    last_update   AS LastUpdate,
                    status        AS Status,
                    first_login   AS FirstLogin
                FROM user
                WHERE status = TRUE
                ORDER BY id DESC;";
            try
            {
                
                return Result<IEnumerable<User>>.Success(_dbConnectionManager.ExecuteQuery<User>(query));
            }
            catch(Exception ex)
            {
                return Result<IEnumerable<User>>.Failure(ex.Message);
            }
            
        }

        public async Task<Result<int>> Insert(User model)
        {
            string query = @"
                INSERT INTO user
                (
                    username,
                    password_hash,
                    first_name,
                    second_name,
                    first_last_name,
                    second_last_name,
                    email,
                    role,
                    created_by,
                    created_date,
                    last_update,
                    status,
                    first_login
                )
                VALUES
                (
                    @Username,
                    MD5(@PasswordHash),
                    @FirstName,
                    @SecondName,
                    @LastName,
                    @SecondLastName,
                    @Email,
                    @Role,
                    @CreatedBy,
                    CURRENT_TIMESTAMP,
                    CURRENT_TIMESTAMP,
                    @Status,
                    @FirstLogin
                );";
            try
            {
                return Result<int>.Success(_dbConnectionManager.ExecuteParameterizedNonQuery(query, model));
            }
            catch (Exception e)
            {
                return Result<int>.Failure(e.Message);
            }
        }
        public async Task<Result<int>> Update(User model)
        {
            string query = @"
                UPDATE user
                SET
                    username      = @Username,
                    password_hash = @PasswordHash,
                    first_name    = @FirstName,
                    second_name   = @SecondName,
                    first_last_name     = @LastName,
                    second_last_name    = @SecondLastName,
                    email         = @Email,
                    role         = @Role,
                    created_by    = @CreatedBy,
                    last_update   = CURRENT_TIMESTAMP,
                    status       = @Status,
                    first_login  = @FirstLogin
                WHERE id = @Id;";
            try
            {
                return Result<int>.Success(_dbConnectionManager.ExecuteParameterizedNonQuery(query, model));
            }
            catch (Exception e)
            {
                return Result<int>.Failure(e.Message);
            }
            
        }

        public async Task<Result<IEnumerable<User>>> Search(string property)
        {
            var probe = new User
            {
                Username = property,
                FirstName = property,
                SecondName = property,
                LastName = property,
                SecondLastName = property,
                Email = property,
                Role = property
            };

            const string query = @"
                SELECT
                    id            ,
                    username      ,
                    password_hash ,
                    first_name    ,
                    second_name   ,
                    first_last_name     ,
                    second_last_name    ,
                    email         ,
                    role          ,
                    created_by    ,
                    created_date  ,
                    last_update   ,
                    status        ,
                    first_login
                FROM user
                WHERE status = TRUE AND (
                    (@Username IS NOT NULL AND username LIKE CONCAT('%', @Username, '%')) OR
                    (@FirstName IS NOT NULL AND first_name LIKE CONCAT('%', @FirstName, '%')) OR
                    (@SecondName IS NOT NULL AND second_name LIKE CONCAT('%', @SecondName, '%')) OR
                    (@LastName IS NOT NULL AND first_last_name LIKE CONCAT('%', @LastName, '%')) OR
                    (@SecondLastName IS NOT NULL AND second_last_name LIKE CONCAT('%', @SecondLastName, '%')) OR
                    (@Email IS NOT NULL AND email LIKE CONCAT('%', @Email, '%')) OR
                    (@Role IS NOT NULL AND role LIKE CONCAT('%', @Role, '%'))
                )
                ORDER BY id DESC;";
            try
            {
                return Result<IEnumerable<User>>.Success(_dbConnectionManager.ExecuteParameterizedQuery<User>(query, probe));
            }
            catch (Exception e)
            {
                return Result<IEnumerable<User>>.Failure(e.Message);
            }
        }

        public async Task<Result<User?>> GetById(int id)
        {
            string query = @"
                SELECT
                    id            AS Id,
                    username      AS Username,
                    password_hash AS PasswordHash,
                    first_name    AS FirstName,
                    second_name   AS SecondName,
                    first_last_name     AS LastName,
                    second_last_name AS SecondLastName,
                    email         AS Email,
                    role          AS Role,
                    created_by    AS CreatedBy,
                    created_date  AS CreatedDate,
                    last_update   AS LastUpdate,
                    status        AS Status,
                    first_login   AS FirstLogin
                FROM user
                WHERE id = @Id AND status = TRUE
                LIMIT 1;";

            var model = new User { Id = id };
            try
            {
                var res = Result<User?>.Success(_dbConnectionManager.ExecuteParameterizedQuery<User>(query, model).FirstOrDefault());
                return res;
            }
            catch (Exception e)
            {
                return Result<User?>.Failure(e.Message);
            }
        }

        public async Task<Result<User?>> GetByUsername(string username)
        {
            const string sql = @"
                SELECT
                    id            AS Id,
                    username      AS Username,
                    password_hash AS PasswordHash,
                    first_name    AS FirstName,
                    second_name   AS SecondName,
                    first_last_name     AS LastName,
                    second_last_name AS SecondLastName,
                    email         AS Email,
                    role          AS Role,
                    created_by    AS CreatedBy,
                    created_date  AS CreatedDate,
                    last_update   AS LastUpdate,
                    status        AS Status,
                    first_login   AS FirstLogin
                FROM user
                WHERE username = @Username
                LIMIT 1;";
            var probe = new User { Username = username };
            try
            {
                return Result<User?>.Success(_dbConnectionManager.ExecuteParameterizedQuery<User>(sql, probe).FirstOrDefault());

            }
            catch (Exception e)
            {
                return Result<User?>.Failure(e.Message);
            }
        }
        public async Task<Result<int>> Delete(int id)
        {
            string query = @"
                UPDATE user
                SET last_update = CURRENT_TIMESTAMP,
                    status = FALSE
                WHERE id = @Id;";
            try
            {
                return Result<int>.Success(_dbConnectionManager.ExecuteParameterizedNonQuery(query, new IdDto() { Id = id}));
            }
            catch (Exception e)
            {
                return Result<int>.Failure(e.Message);
            }
        }
    }
}
