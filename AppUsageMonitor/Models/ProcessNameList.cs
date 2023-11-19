using System.Diagnostics;
using System.Linq;

namespace AppUsageMonitor.Models;

public class ProcessNameList {
    private readonly Process[] _processes;

    public ProcessNameList() {
        _processes = Process.GetProcesses();
    }

    public string[] ToStringArray() {
        var processNames = _processes.Select(p => p.ProcessName).ToArray();
        return processNames;
    }
}