using ControlGastos.API.DTOs;

namespace ControlGastos.API.Services
{
    public interface IRegistroGastoService
    {
        Task<IEnumerable<RegistroGastoDto>> GetAllAsync(int usuarioId, bool esAdmin);
        Task<IEnumerable<RegistroGastoDto>> GetByUsuariosAsync(List<int> usuariosIds);
        Task<RegistroGastoDto?> GetByIdAsync(int id, int usuarioId, bool esAdmin);
        Task<(RegistroGastoDto registro, ValidacionPresupuestoDto validacion)> CreateAsync(RegistroGastoCreateDto dto, int usuarioId);
        Task<bool> DeleteAsync(int id, int usuarioId, bool esAdmin);
        Task<ValidacionPresupuestoDto> ValidarPresupuestoAsync(RegistroGastoCreateDto dto, int usuarioId);
    }
}
