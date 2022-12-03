namespace chromeopener;
using System.Diagnostics;
using System.IO;

class ChromeOpener{

    public static string PluginMain(){
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
        startInfo.UseShellExecute = true;
        startInfo.RedirectStandardOutput = false;
        startInfo.CreateNoWindow = false;

        using (Process process = Process.Start(startInfo))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                //string result = await reader.ReadToEndAsync();
                return "Chrome Started!";
            }
        }
    }
}
