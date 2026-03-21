using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

Codex_test_run_process.main();

public static class Codex_test_run_process
{
    public static object main()
    {
        ((Func<object>)(() => {
                ((Func<string, object>)((result) => Console.WriteLine(string.Concat("dotnet version: ", result))))(((Func<string>)(() => { var _psi = new System.Diagnostics.ProcessStartInfo("dotnet", "--version") { RedirectStandardOutput = true, UseShellExecute = false }; var _p = System.Diagnostics.Process.Start(_psi)!; var _o = _p.StandardOutput.ReadToEnd(); _p.WaitForExit(); return _o; }))());
                return null;
            }))();
        return null;
    }

}
