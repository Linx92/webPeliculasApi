namespace PeliculasAPI.Servicios
{
    public interface IAlmacenadorArchivos
    {
        Task<string> GurdarArchivo(byte[] contenido, string extension, string contenedor, string contentType);
        Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor, string ruta,string contentType);
        Task BorrarArchivo(string ruta, string contenedor);
    }
}
