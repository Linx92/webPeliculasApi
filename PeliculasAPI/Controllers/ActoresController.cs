using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Helpers;
using PeliculasAPI.Servicios;
using PeliculasAPI.Validaciones;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/actores")]
    public class ActoresController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "actores";

        public ActoresController(ApplicationDbContext context,
            IMapper mapper, IAlmacenadorArchivos almacenadorArchivos):base(context,mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }
        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            return await Get<Actor, ActorDTO>(paginacionDTO);
        }
        [HttpGet("{id}", Name = "obtenerActor")]
        public async Task<ActionResult<ActorDTO>> Get(int id) 
        {
            return await Get<Actor, ActorDTO>(id);
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromForm]ActorCreacionDTO actorCreacionDTO) 
        {
            var actorBD = mapper.Map<Actor>(actorCreacionDTO);
            if (actorCreacionDTO.Foto != null) 
            {
                using (var memoryStream = new MemoryStream()) 
                {
                    await actorCreacionDTO.Foto.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);
                    actorBD.Foto = await almacenadorArchivos.GuardarArchivo(contenido,extension,contenedor,actorCreacionDTO.Foto.ContentType);
                }
            }
            context.Add(actorBD);
            await context.SaveChangesAsync();
            var actorDTO = mapper.Map<ActorDTO>(actorBD);
            return new CreatedAtRouteResult("obtenerActor", new { id = actorDTO.Id }, actorDTO);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreacionDTO actorCreacionDTO) 
        {
            var actorBD = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);
            if (actorBD == null) return NotFound();
            actorBD = mapper.Map(actorCreacionDTO, actorBD);

            if (actorCreacionDTO.Foto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await actorCreacionDTO.Foto.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);
                    actorBD.Foto = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor,actorBD.Foto ,actorCreacionDTO.Foto.ContentType);
                }
            }
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument)
        {
            return await Patch<Actor, ActorPatchDTO>(id, patchDocument);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Actor>(id);
        }
    }
}
