using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            CreateMap<Genero, GeneroDTO>().ReverseMap();
            CreateMap<GeneroCreacionDTO, Genero>();
            CreateMap<IdentityUser, UsuarioDTO>();

            CreateMap<SalaDeCine, SalaDeCineDTO>()
                .ForMember(x => x.Latitud, x => x.MapFrom(y => y.Ubicacion.Y))
                .ForMember(x => x.Longitud, x => x.MapFrom(y => y.Ubicacion.X));
            CreateMap<SalaDeCineDTO, SalaDeCine>()
                .ForMember(x => x.Ubicacion, x => 
                x.MapFrom(y => geometryFactory.CreatePoint(new Coordinate(y.Longitud, y.Latitud))));
            CreateMap<SalaDeCineCreacionDTO, SalaDeCine>()
               .ForMember(x => x.Ubicacion, x =>
               x.MapFrom(y => geometryFactory.CreatePoint(new Coordinate(y.Longitud, y.Latitud))));


            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<ActorCreacionDTO, Actor>()
                .ForMember(x=>x.Foto,options =>options.Ignore());

            CreateMap<ActorPatchDTO, Actor>().ReverseMap();

            CreateMap<Pelicula, PeliculaDTO>().ReverseMap();
            CreateMap<PeliculaCreacionDTO, Pelicula>()
                .ForMember(x => x.Poster, options => options.Ignore())
                .ForMember(x => x.PeliculasGeneros, options => options.MapFrom(MapPeliculasGeneros))
                .ForMember(x => x.PeliculasActores, options => options.MapFrom(MapPeliculasActores));

            CreateMap<Pelicula, PeliculaDetallesDTO>()
                .ForMember(x => x.Actores, options => options.MapFrom(MapPeliculaActores))
                .ForMember(x => x.Generos, options => options.MapFrom(MapPeliculaGeneros));

            CreateMap<PeliculaPatchDTO, Pelicula>().ReverseMap();
        }
        private List<ActorPeliculaDetalleDTO> MapPeliculaActores(Pelicula pelicula, PeliculaDetallesDTO peliculaDetallesDTO)
        {
            var resultados = new List<ActorPeliculaDetalleDTO>();
            if (pelicula.PeliculasActores == null) { return resultados; }

            foreach (var actor in pelicula.PeliculasActores) 
            {
                resultados.Add(new ActorPeliculaDetalleDTO { ActorId = actor.ActorId, 
                    Personaje=actor.Personaje,NombreActor = actor.Actor.Nombre });
            }
            return resultados;
        }
        private List<GeneroDTO> MapPeliculaGeneros(Pelicula pelicula, PeliculaDetallesDTO peliculaDetallesDTO) 
        {
            var resultados = new List<GeneroDTO>();
            if (pelicula.PeliculasGeneros == null) { return resultados; }

            foreach (var genero in pelicula.PeliculasGeneros) 
            {
                resultados.Add(new GeneroDTO
                {
                    Id = genero.GeneroId,
                    Nombre = genero.Genero.Nombre
                });
            }
            return resultados;
        }
        private List<PeliculasGeneros> MapPeliculasGeneros(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        { 
            var resultados = new List<PeliculasGeneros>();
            if (peliculaCreacionDTO.GenerosIDs == null) { return resultados; }
            foreach (var id in peliculaCreacionDTO.GenerosIDs) 
            {
                resultados.Add(new PeliculasGeneros() { GeneroId = id });
            }
            return resultados;
        }
        private List<PeliculasActores> MapPeliculasActores(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var resultados = new List<PeliculasActores>();
            if (peliculaCreacionDTO.Actores == null) { return resultados; }
            foreach (var actor in peliculaCreacionDTO.Actores)
            {
                resultados.Add(new PeliculasActores() { ActorId=actor.ActorId,Personaje=actor.Personaje});
            }
            return resultados;
        }
    }
}
