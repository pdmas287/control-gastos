using ControlGastos.API.DTOs;

namespace ControlGastos.API.Services
{
    public interface ITipoGastoService
    {
        Task<IEnumerable<TipoGastoDto>> GetAllAsync(int usuarioId, bool esAdmin);
        Task<IEnumerable<TipoGastoDto>> GetByUsuariosAsync(List<int> usuariosIds);
        Task<TipoGastoDto?> GetByIdAsync(int id, int usuarioId, bool esAdmin);
        Task<TipoGastoDto> CreateAsync(TipoGastoCreateDto dto, int usuarioId, bool esAdmin);
        Task<TipoGastoDto?> UpdateAsync(int id, TipoGastoUpdateDto dto, int usuarioId, bool esAdmin);
        Task<bool> DeleteAsync(int id, int usuarioId, bool esAdmin);
        Task<string> GetNextCodigoAsync(int usuarioId, bool esAdmin);
    }
}
