using ControlGastos.API.DTOs;

namespace ControlGastos.API.Services
{
    public interface IFondoMonetarioService
    {
        Task<IEnumerable<FondoMonetarioDto>> GetAllAsync(int usuarioId, bool esAdmin);
        Task<IEnumerable<FondoMonetarioDto>> GetByUsuariosAsync(List<int> usuariosIds);
        Task<FondoMonetarioDto?> GetByIdAsync(int id, int usuarioId, bool esAdmin);
        Task<FondoMonetarioDto> CreateAsync(FondoMonetarioCreateDto dto, int usuarioId);
        Task<FondoMonetarioDto?> UpdateAsync(int id, FondoMonetarioUpdateDto dto, int usuarioId, bool esAdmin);
        Task<bool> DeleteAsync(int id, int usuarioId, bool esAdmin);
    }
}
