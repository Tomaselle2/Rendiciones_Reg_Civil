using RendicionesRegistroCivil.DTO.Rendicion;

namespace RendicionesRegistroCivil.Interfaces
{
    public interface IRendicion
    {
        Task<RendicionDTO> GetbyId(int id);
        Task<IEnumerable<RendicionDTO>> GetAll(int anio);
        Task<RendicionDTO> Create(CrearRendicionDTO dto);
        Task<RendicionDTO?> Update(int id, RendicionDTO rendicion);
    }
}
