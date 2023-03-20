using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeliculasAPI.Controllers;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasAPI.Tests.PruebasUnitarias
{
    [TestClass]
    public class GenerosControllerTests : BasePruebas
    {
        [TestMethod]
        public async Task ObtenerTodosLosGeneros() 
        {
            //Preparacion
            var nombreBd= Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            contexto.Generos.Add(new Genero() { Nombre = "Genero1" });
            contexto.Generos.Add(new Genero() { Nombre = "Genero2" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBd);
            //Prueba
            var controller = new GenerosController(contexto2, mapper);
            var respuesta = await controller.Get();
            //Verificacion
            var generos = respuesta.Value;
            Assert.AreEqual(2, generos.Count);
        }
        [TestMethod]
        public async Task ObtenerGeneroPorIdNoExistente() 
        {
            var nombreBd = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            var controller = new GenerosController(contexto, mapper);
            var respuesta = await controller.Get(1);

            var resultado = respuesta.Result as StatusCodeResult;
            Assert.AreEqual(404, resultado.StatusCode);
        }
        [TestMethod]
        public async Task ObetnerGeneroPorIdExistente() 
        {
            var nombreBd = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            contexto.Generos.Add(new Genero() { Nombre = "Genero1" });
            contexto.Generos.Add(new Genero() { Nombre = "Genero2" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBd);
            var controller = new GenerosController(contexto2, mapper);

            var Id = 1;
            var respuesta = await controller.Get(Id);
            var resultado = respuesta.Value;
            Assert.AreEqual(Id,resultado.Id);
        }
        [TestMethod]
        public async Task CrearGenero() 
        {
            var nombreBd = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            var nuevoGenero = new GeneroCreacionDTO() { Nombre = "Genero1" };
            var controller = new GenerosController(contexto, mapper);

            var respuesta = await controller.Post(nuevoGenero);
            var resultado = respuesta as CreatedAtRouteResult;
            Assert.IsNotNull(resultado);

            var context2 = ConstruirContext(nombreBd);
            var cantidad = context2.Generos.Count();
            Assert.AreEqual(1, cantidad);
        }
        [TestMethod]
        public async Task ActualizarGenero() 
        {
            var nombreBd = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            contexto.Generos.Add(new Genero() { Nombre = "Genero1" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBd);
            var controller = new GenerosController(contexto2, mapper);

            var generoCreaciontDTO = new GeneroCreacionDTO() { Nombre = "Nuevo Genero" };

            var id = 1;
            var respuesta = await controller.Put(id, generoCreaciontDTO);

            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado.StatusCode);

            var contexto3 = ConstruirContext(nombreBd);
            var existe = await contexto3.Generos.AnyAsync(x => x.Nombre == "Nuevo Genero");
            Assert.IsTrue(existe);
        }
       
        [TestMethod]
        public async Task IntentaBorrarGeneroNoExistente()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            var controller = new GenerosController(contexto, mapper);

            var respuesta = await controller.Delete(1);
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(404, resultado.StatusCode);
        }

        [TestMethod]
        public async Task BorrarGenero()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            contexto.Generos.Add(new Genero() { Nombre = "Género 1" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBD);
            var controller = new GenerosController(contexto2, mapper);

            var respuesta = await controller.Delete(1);
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado.StatusCode);

            var contexto3 = ConstruirContext(nombreBD);
            var existe = await contexto3.Generos.AnyAsync();
            Assert.IsFalse(existe);
        }
    }
}
