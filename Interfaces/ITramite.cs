using RendicionesRegistroCivil.DTO.Tramite;

namespace RendicionesRegistroCivil.Interfaces
{
    public interface ITramite
    {
        Task<IEnumerable<TramiteDTO>> ConsultarTramites();
    }
}
