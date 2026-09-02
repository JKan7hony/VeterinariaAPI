using VeterinariaAPI.Models;

namespace VeterinariaAPI.Repositories
{
    public interface IUsuarioRepository
    {
        Task<List<Usuario>> ObtenerTodosAsync();
        Task<Usuario?> ObtenerPorIdAsync(int id);
        Task<Usuario?> ObtenerPorEmailAsync(string email);
        Task CrearAsync(Usuario usuario);
        Task EliminarAsync(Usuario usuario);
        Task GuardarCambiosAsync();
    }
}