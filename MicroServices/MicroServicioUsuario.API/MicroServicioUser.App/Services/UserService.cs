using MicroServicioUser.Dom.Entities;
using MicroServicioUser.Dom.Interfaces;
using MicroServicioUser.Dom.Patterns;
using MicroServicoUser.Inf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServicioUser.App.Services
{
    public class UserService
    {
        private readonly IRepository userRepository;
        public UserService(IRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<Result<int>> Insert(User t)
        {
            t.CreatedDate = DateTime.Now;
            t.LastUpdate = DateTime.Now;
            t.Status = true;

            try
            {
                var id = await userRepository.Insert(t);
                return Result<int>.Success(id);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure("Error al insertar el usuario en la base de datos", ex.Message);
            }
        }

        public async Task<Result<int>> Update(User t)
        {
            t.LastUpdate = DateTime.Now;

            try
            {
                var result = await userRepository.Update(t);
                if (result == 0)
                    return Result<int>.Failure("No se encontró el usuario para actualizar");

                return Result<int>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure("Error al actualizar el usuario en la base de datos", ex.Message);
            }
        }

        public async Task<Result<int>> Delete(int id)
        {
            try
            {
                var result = await userRepository.Delete(id);
                if (result == 0)
                    return Result<int>.Failure("No se encontró el usuario para eliminar");

                return Result<int>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure("Error al eliminar el usuario en la base de datos", ex.Message);
            }
        }

        public async Task<Result<List<User>>> Select()
        {
            try
            {
                var users = await userRepository.GetAll();
                return Result<List<User>>.Success(users.ToList());
            }
            catch (Exception ex)
            {
                return Result<List<User>>.Failure("Error al obtener los usuarios", ex.Message);
            }
        }

        public async Task<Result<List<User>>> Search(string property)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(property))
                {
                    return Result<List<User>>.Failure("El término de búsqueda no puede estar vacío");
                }

                var users = await userRepository.Search(property);
                return Result<List<User>>.Success(users.ToList());
            }
            catch (Exception ex)
            {
                return Result<List<User>>.Failure("Error al buscar usuario", ex.Message);
            }
        }
    }
}
