using RendicionesRegistroCivil.DTO.Tramite;

namespace RendicionesRegistroCivil.Interfaces
{
    public interface ITramite
    {
        Task<IEnumerable<TramiteDTO>> ConsultarTramites();
        Task<TramiteDTO> GetById(int id);
        Task<TramiteDTO> RegistrarTramite(TramiteDTO tramiteDTO);
        Task<TramiteDTO> EditarTramite(TramiteDTO tramiteDTO);
    }
}
