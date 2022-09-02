using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services
{
    public interface ITipoCuentasRepositorio
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
    }
    public class TipoCuentasRepositorio: ITipoCuentasRepositorio
    {
        private readonly string connectionString;

        public TipoCuentasRepositorio(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConexionPrin");

        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var cnx = new SqlConnection(connectionString);
            var id = await cnx.QuerySingleAsync<int>
                                           ($@"INSERT TiposCuentas (Nombre, UsuarioId, Orden)
	                                       VALUES (@Nombre, @UsuarioId, 0);
                                           Select scope_identity();", tipoCuenta);
            tipoCuenta.Id = id;
        }

        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using var cnx = new SqlConnection(connectionString);
            var existe = await cnx.QueryFirstOrDefaultAsync<int>(
                @"Select 1 from TiposCuentas where Nombre=@Nombre and UsuarioId=@UsuarioId;", new { nombre, usuarioId });

            return existe == 1;
        }
        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId){
            using var cnx = new SqlConnection(connectionString);
            return await cnx.QueryAsync<TipoCuenta>(@"SELECT Id,Nombre ,Orden 
                                            FROM TiposCuentas 
                                            WHERE UsuarioId=@UsuarioId", new { usuarioId });
        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var cnx = new SqlConnection(connectionString);
            await cnx.ExecuteAsync(@"UPDATE t SET T.Nombre=@Nombre FROM TiposCuentas t WHERE Id=@id",tipoCuenta);
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var cnx = new SqlConnection(connectionString);
            return await cnx.QueryFirstOrDefaultAsync<TipoCuenta>(@"
                            SELECT tc.Id, tc.Nombre, tc.Orden FROM TiposCuentas tc WHERE tc.Id=@Id AND tc.UsuarioId=@UsuarioId",
                            new { id, usuarioId });
        }

        public async Task Borrar(int id)
        {
            using var cnx = new SqlConnection(connectionString);
            await cnx.ExecuteAsync(@"Delete from TiposCuentas Where id=@id ", new { id});
        }

    }
}
