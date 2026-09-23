namespace AngularWithASP.Server.Infrastructure
{
  public class MyEnv
  {
    private IConfiguration _config;
    public MyEnv(IHostEnvironment env, IConfiguration c){ // Receive arguments through DI.
      _config = c;
      EnvironmentName = env.EnvironmentName;
    }
    public string EnvironmentName{ get; set; }
  }
}
