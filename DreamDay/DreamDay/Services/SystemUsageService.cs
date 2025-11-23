using System.Diagnostics;

namespace DreamDay.Services
{
    // IMPORTANT: This service will only work on Windows where performance counters are available.
    public class SystemUsageService
    {
        private readonly PerformanceCounter _cpuCounter;
        private readonly PerformanceCounter _memoryCounter;

        public SystemUsageService()
        {
            // Initialize performance counters. This might throw an exception on non-Windows OS.
            try
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _memoryCounter = new PerformanceCounter("Memory", "Available MBytes");
            }
            catch (Exception)
            {
                // Handle cases where performance counters are not available (e.g., Linux/macOS without special setup)
                _cpuCounter = null;
                _memoryCounter = null;
            }
        }

        public float GetCurrentCpuUsage()
        {
            // Return a value or a dummy value if the counter is not available
            if (_cpuCounter == null) return 0;
            return _cpuCounter.NextValue();
        }

        public float GetAvailableMemory()
        {
            if (_memoryCounter == null) return 0;
            return _memoryCounter.NextValue();
        }
    }
}