using ControlGastos.API.DTOs;

namespace ControlGastos.API.Services
{
    public interface IDepositoService
    {
        Task<IEnumerable<DepositoDto>> GetAllAsync(int usuarioId, bool esAdmin);
        Task<IEnumerable<DepositoDto>> GetByUsuariosAsync(List<int> usuariosIds);
        Task<DepositoDto?> GetByIdAsync(int id, int usuarioId, bool esAdmin);
        Task<DepositoDto> CreateAsync(DepositoCreateDto dto, int usuarioId);
        Task<DepositoDto?> UpdateAsync(int id, DepositoUpdateDto dto, int usuarioId, bool esAdmin);
        Task<bool> DeleteAsync(int id, int usuarioId, bool esAdmin);
    }
}
