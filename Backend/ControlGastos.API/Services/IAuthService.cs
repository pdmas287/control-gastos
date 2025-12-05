using ControlGastos.API.DTOs;

namespace ControlGastos.API.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegistrarUsuarioAsync(RegistroUsuarioDto registroDto);
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<UsuarioDto?> ObtenerUsuarioActualAsync(int usuarioId);
        Task<bool> CambiarPasswordAsync(int usuarioId, CambiarPasswordDto cambiarPasswordDto);
        Task ActualizarUltimoAccesoAsync(int usuarioId);
    }
}
