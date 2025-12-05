using ControlGastos.API.DTOs;

namespace ControlGastos.API.Services
{
    public interface IUsuarioService
    {
        // Métodos para administradores
        Task<IEnumerable<UsuarioAdminDto>> GetAllUsuariosAsync();
        Task<UsuarioAdminDto?> GetUsuarioByIdAsync(int id);
        Task<UsuarioAdminDto?> UpdateUsuarioAsync(int id, UsuarioUpdateAdminDto dto);
        Task<bool> ActivarUsuarioAsync(int id);
        Task<bool> DesactivarUsuarioAsync(int id);
        Task<bool> CambiarRolAsync(int id, string nuevoRol);
        Task<EstadisticasUsuariosDto> GetEstadisticasAsync();
        Task<bool> DeleteUsuarioAsync(int id);
    }
}
