using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace PeliculasAPI.Helpers
{
    public class PeliculaExisteAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly ApplicationDbContext dbContext;

        public PeliculaExisteAttribute(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context,
            ResourceExecutionDelegate next)
        {
            var peliculaObject = context.HttpContext.Request.RouteValues["peliculaId"];
            if (peliculaObject == null) { return; }

            var peliculaId = int.Parse(peliculaObject.ToString());

            var existePelicula = await dbContext.Peliculas.AnyAsync(x => x.Id == peliculaId);
            if (!existePelicula)
            {
                context.Result = new NotFoundResult();
            }
            else 
            {
                await next();
            }
        }
    }
}
