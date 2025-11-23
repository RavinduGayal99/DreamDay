using DreamDay.Services;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DreamDay
{
    public class SystemUsageHub : Hub
    {
        private readonly SystemUsageService _usageService;

        public SystemUsageHub(SystemUsageService usageService)
        {
            _usageService = usageService;
        }

        public async Task GetSystemUsage()
        {
            var cpuUsage = _usageService.GetCurrentCpuUsage();
            var memUsage = _usageService.GetAvailableMemory();
            // Send the data to the client that requested it
            await Clients.Caller.SendAsync("ReceiveSystemUsage", cpuUsage, memUsage);
        }
    }
}