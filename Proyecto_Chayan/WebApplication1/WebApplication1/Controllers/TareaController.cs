using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareaController : ControllerBase
    {
        //configurando el entorno para usar la cadena de coneccion , _config es la llave para usar la cadena de conexion
        private IConfiguration _Config;

        public TareaController(IConfiguration Config)
        {
            _Config = Config;
        }

        [HttpGet]
        public async Task<ActionResult<List<Tarea>>> GetAllLTareas()
        {
            using var conexion = new SqlConnection(_Config.GetConnectionString("DefaultConnection"));
            conexion.Open();
            var oTarea = conexion.Query<Tarea>("LeerTodasLasTareas", commandType: System.Data.CommandType.StoredProcedure);
            return Ok(oTarea);
        }


        [HttpPost]
        // obteniendo el objeto de usuario de la informacion proporcionada por Swagger
        public async Task<ActionResult<Tarea>> CreateTarea(Tarea ta)
        {
            try
            {
                using var conexion = new SqlConnection(_Config.GetConnectionString("DefaultConnection"));
                conexion.Open();
                var parametro = new DynamicParameters();
                parametro.Add("@Titulo", ta.Titulo);
                parametro.Add("@Descripcion", ta.Descripcion);

                var oTarea = conexion.Query<Tarea>("CrearTarea", parametro, commandType: System.Data.CommandType.StoredProcedure);

                // Verificar si la operación fue exitosa (por ejemplo, si oLibro no es nulo)
                if (oTarea != null)
                {

                    var mensaje = "tarea creada exitosamente.";
                    return Ok(mensaje);
                }
                else
                {

                    var mensaje = "No se pudo crear la tarea.";
                    return BadRequest(new { mensaje });
                }
            }
            catch (Exception ex)
            {

                var mensaje = "Se produjo un error al crear la tarea: " + ex.Message;
                return StatusCode(500, new { mensaje });
            }
        }

        [HttpDelete("{ID}")]
        // obteniendo id del libro a eliminar (este id es de la clase Libros)
        public async Task<ActionResult> DeleteAuto(int ID)
        {   
            //manejo de errores con trycach
            try
            {
                using (var conexion = new SqlConnection(_Config.GetConnectionString("DefaultConnection")))
                {
                    await conexion.OpenAsync();

                    var parametro = new DynamicParameters();
                    parametro.Add("@ID", ID);
                    await conexion.ExecuteAsync("EliminarTarea", parametro, commandType: CommandType.StoredProcedure);

                    return Ok("Tarea eliminada correctamente.");
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Error al eliminar la tarea: {ex.Message}");
            }
        }
    }
}
