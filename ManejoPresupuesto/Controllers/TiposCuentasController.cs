using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


namespace ManejoPresupuesto.Controllers
{
    public class TiposCuentasController: Controller
    {
        private readonly ITipoCuentasRepositorio tipoCuentasRepositorio;
        private readonly IServicioUsuario servicioUsuario;

        public TiposCuentasController(ITipoCuentasRepositorio tipoCuentasRepositorio, IServicioUsuario servicioUsuario)
        {
            this.tipoCuentasRepositorio = tipoCuentasRepositorio;
            this.servicioUsuario = servicioUsuario;
        }
        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var tiposCuentas =await tipoCuentasRepositorio.Obtener(usuarioId);
            return View(tiposCuentas);

        }
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }
            tipoCuenta.UsuarioId = servicioUsuario.ObtenerUsuarioId();

            var SiExiste= await tipoCuentasRepositorio.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);
            if (SiExiste)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre {tipoCuenta.Nombre} ya Existe.");
                return View(tipoCuenta);
            }

            await tipoCuentasRepositorio.Crear(tipoCuenta);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int id)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var tipoCuenta = await tipoCuentasRepositorio.ObtenerPorId(id, usuarioId);

            if (tipoCuenta == null)
            {
                return RedirectToAction("NoEncontrado","Home");
            }
            return View(tipoCuenta);

        }

        [HttpPost]
        public async Task<ActionResult> Editar(TipoCuenta tipoCuenta)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var tipoCuentaExiste = await tipoCuentasRepositorio.ObtenerPorId(tipoCuenta.Id, usuarioId);
            if (tipoCuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado","Home");
            }
            //Recién Actualizo
            await tipoCuentasRepositorio.Actualizar(tipoCuenta);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var tipoCuentaExiste = await tipoCuentasRepositorio.ObtenerPorId(id, usuarioId);
            if (tipoCuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuentaExiste);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var tipoCuenta = await tipoCuentasRepositorio.ObtenerPorId(id,usuarioId);
            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await tipoCuentasRepositorio.Borrar(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var yaExisteTipoCenta = await tipoCuentasRepositorio.Existe(nombre, usuarioId);

            if (yaExisteTipoCenta)
            {
                return Json($"El nombre {nombre} ya existe");
            }

            return Json(true);

        }

        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            return Ok();
        }
    }

    
}
