using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace OGAS.Services
{
    // NoOpHostedService.cs
    public class NoOpHostedService : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // 不做任何事，只是让Host有一个持续存在的后台任务
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // 当Host停止时，无需执行其他操作
            return Task.CompletedTask;
        }
    }

}
