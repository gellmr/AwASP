using Microsoft.AspNetCore.Mvc;
using AngularWithASP.Server.Infrastructure;

namespace AngularWithASP.Server.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class EnvNameController : ControllerBase
  {
    private MyEnv Env { get; set; }
    public EnvNameController(MyEnv env){
      Env = env;
    }

    [HttpGet] // GET api/envname
    public ActionResult Get(){
      var response = new { env=Env.EnvironmentName };
      return Ok(response); // Respond with 200 OK, and object value
    }
  }
}
