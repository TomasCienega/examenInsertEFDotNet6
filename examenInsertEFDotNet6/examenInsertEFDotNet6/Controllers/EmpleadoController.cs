using examenInsertEFDotNet6.Context;
using examenInsertEFDotNet6.Models;
using examenInsertEFDotNet6.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace examenInsertEFDotNet6.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly examenInsertEFDotNet6Context _context;
        public EmpleadoController(examenInsertEFDotNet6Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int idDep)
        {
            var vm = new EmpleadoVM();
            ViewBag.IdSeleccionado = idDep;
            try
            {
                vm.ListaDepartamentos = await _context.Departamentos.ToListAsync();
                if (idDep > 0)
                {
                    vm.ListaEmpleados = await _context.Empleados.FromSqlRaw("exec sp_ListarEmpleadoPorIdDep {0}", idDep).ToListAsync();
                }
                else
                {
                    vm.ListaEmpleados = await _context.Empleados.Include(tD => tD.IdDepartamentoNavigation).ToListAsync();
                }
                return View(vm);

            }
            catch (Exception ex)
            {
                vm.ListaEmpleados = new List<Empleado>();
                vm.ListaDepartamentos = new List<Departamento>();
                ViewData["Error"] = "Algo ocurrio con la conexion" + ex.Message;
                return View(vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Guardar(EmpleadoVM vm)
        {
            try
            {
                await _context.Empleados.AddAsync(vm.EmpleadoModelReference);
                int _filasAfectadas = await _context.SaveChangesAsync();
                if (_filasAfectadas > 0)
                {
                    TempData["Mensaje"] = "Usuario Guardado con exito!";
                    TempData["Tipo"] = "success";
                }
                else
                {
                    TempData["Mensaje"] = "Algo paso al guardar el usuario!";
                    TempData["Tipo"] = "warning";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al guardar" + ex.Message;
                TempData["Tipo"] = "danger";
            }
            return RedirectToAction("Index");
        }
    }
}
