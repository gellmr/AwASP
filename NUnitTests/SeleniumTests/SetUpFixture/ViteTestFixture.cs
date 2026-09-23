using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using NUnitTests.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;


namespace NUnitTests.SeleniumTests
{
  // The Visual Studio debugger attaches to the test host process after the initial NUnit setup phase, but before test methods run. This means it will hit breakpoints for a test method, but it does not attach early enough to hit breakpoints within a SetUpFixture class. To allow breakpoints to be hit for the SetUpFixture, uncomment the DebugTest class below. Then you can right-click and Debug the MyTest class, in Test Explorer, and it will run and hit breakpoints within the SetUpFixture. Don't try to also run normal tests this way. Once you have finished debugging the SetUpFixture, comment out the DebugTest class again.

  //[TestFixture]
  //public class DebugTest()
  //{
  //  [Test]
  //  public void MyTest()
  //  {
  //    // Add a breakpoint here
  //    System.Diagnostics.Debugger.Break();
  //  }
  //}

  // This class is responsible for managing the lifecycle of both the Vite client and the .NET Core backend.
  // It uses the [SetUpFixture] attribute to run setup/teardown once for the entire test assembly.
  [SetUpFixture]
  public class ViteTestFixture
  {
    private Process? backendProcess; // Process for the .NET Core backend server.
    private Process? viteProcess;    // Process for the Vite front-end server.

    private const string? vitePort = "4200";
    private const string? dotNetPort = "7225";

    private const string viteUrl = "https://localhost:" + vitePort;   //  Vite SPA proxy
    private const string backendUrl = "https://localhost:" + dotNetPort; // .NET Core backend server

    private void GetAppSettings()
    {
      try
      {
        string? projRootDir = MgUtility.FindProjectRoot();
        if (projRootDir == null) { throw new Exception("Could not find test project root directory");};
        string jsonFile = projRootDir + "/appSettings.json";
        IConfiguration? configuration = new ConfigurationBuilder()
            .SetBasePath(TestContext.CurrentContext.TestDirectory)
            .AddJsonFile(jsonFile, optional: false, reloadOnChange: true)
            .Build();
        TestConfiguration.Config = configuration;
      }
      catch (Exception)
      {
        throw;
      }
    }

    private ProcessStartInfo GetDefProcessInfo(ProcessStartInfo arg)
    {
      return new ProcessStartInfo{
        FileName               = arg.FileName,
        Arguments              = arg.Arguments,
        WorkingDirectory       = arg.WorkingDirectory,
        RedirectStandardOutput = true,
        RedirectStandardError  = true,
        UseShellExecute        = false,
        CreateNoWindow         = true
      };
    }

    private async Task StartVite(){
      TerminateExistingProcesses("node");
      // This runs "npm start" in the given working directory. It will cause Vite to launch.
      viteProcess = new Process{
        StartInfo = GetDefProcessInfo( new ProcessStartInfo{
          FileName = "C:/Program Files/nodejs/npm.cmd",
          Arguments = "start",
          //WorkingDirectory = "../angularwithasp.client", // Root of the SPA project.
          WorkingDirectory = "C:/Users/Michael Gell/source/repos/AwASP/angularwithasp.client", // Root of the SPA project.
        })
      };
      StartProcess(viteProcess, "ViteSPA", "C:/temp/ViteSPA.txt");
      await WaitForServerReady(viteUrl); // Wait for the Vite server to be ready.
    }

    private async Task StartBackend(){
      TerminateExistingProcesses("dotnet");
      // This executes the program "dotnet" in the given working directory, with some command line arguments.
      backendProcess = new Process{
        StartInfo = GetDefProcessInfo(new ProcessStartInfo{
          FileName = "dotnet",
          //Arguments = $"run --urls \"{backendUrl}\" --launch-profile https",
          //Arguments = $"run --urls \"{backendUrl}\"",
          Arguments = "run",
          WorkingDirectory = "C:/Users/Michael Gell/source/repos/AwASP/AngularWithASP.Server", // Root of the backend server project.
        })
      };
      StartProcess(backendProcess, "Backend", "C:/temp/Backend.txt");
      await WaitForServerReady(backendUrl); // Wait for the backend to be ready before starting the frontend.
    }

    private void StartProcess(Process process, string? debugPrefix, string logfilepath)
    {
      Console.WriteLine("Starting " + debugPrefix + " process...");

      var logFile = new StreamWriter(logfilepath, true); // `true` for append mode

      process.OutputDataReceived += (sender, e) =>
      {
        if (e.Data != null)
        {
          // Write the output to both the console and the log file
          Console.WriteLine($"[{debugPrefix}]: {e.Data}");
          logFile.WriteLine($"[{debugPrefix}]: {e.Data}");
          logFile.Flush(); // Flush the buffer to write immediately
        }
      };

      process.ErrorDataReceived += (sender, e) =>
      {
        if (e.Data != null)
        {
          // Write the error output to both the console and the log file
          Console.WriteLine($"[{debugPrefix} Error]: {e.Data}");
          logFile.WriteLine($"[{debugPrefix} Error]: {e.Data}");
          logFile.Flush();
        }
      };
      process.Start();
      process.BeginOutputReadLine();
      process.BeginErrorReadLine();
    }

    // This method runs once before any tests in the assembly.
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
      GetAppSettings();
      //await StartBackend();
      //await StartVite();
      Console.WriteLine("Both servers are ready. Tests can now begin.");
    }

    // This method runs once after all tests in the assembly have completed.
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
      Console.WriteLine("Stopping servers...");

      // Gracefully kill the Vite process if it's still running.
      if (viteProcess != null && !viteProcess.HasExited)
      {
        try { viteProcess.Kill(true); viteProcess.Dispose(); } catch { }
      }

      // Gracefully kill the backend process if it's still running.
      if (backendProcess != null && !backendProcess.HasExited)
      {
        try { backendProcess.Kill(true); backendProcess.Dispose(); } catch { }
      }

      Console.WriteLine("Servers have been shut down.");
    }

    // A helper method to find and terminate existing server processes.
    private void TerminateExistingProcesses(string processName){
      // "dotnet" - Find and kill existing .NET processes.
      // "node"   - Find and kill existing Node.js processes (Vite runs on node).
      Int32 millis = 7000;
      foreach (var process in Process.GetProcessesByName(processName)){
        try { process.Kill(true); process.WaitForExit(millis); } catch { }
      }
    }

    // This is a helper method to wait for a server to be ready.
    private async Task WaitForServerReady(string url)
    {
      using (var client = new HttpClient())
      {
        const int maxRetries = 30; // 30 seconds timeout
        const int delayMs = 1000;  // 1 second delay

        for (int i = 0; i < maxRetries; i++)
        {
          try
          {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            Console.WriteLine($"Server at {url} is ready!");
            return; // Success!
          }
          catch (HttpRequestException)
          {
            Console.WriteLine($"Waiting for server at {url}... Attempt {i + 1}");
            await Task.Delay(delayMs);
          }
        }
        throw new TimeoutException($"Timed out waiting for server at {url} to start.");
      }
    }
  }
}
