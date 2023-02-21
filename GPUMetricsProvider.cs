using ADLXWrapper;
using System;

namespace FanControl.ADLX
{
    public class GPUMetricsProvider : IDisposable
    {
        private readonly PerformanceMonitor _performanceMonitor;
        private readonly GPU _gpu;
        private GPUMetrics _metrics;

        public GPUMetricsProvider(PerformanceMonitor performanceMonitor, GPU gpu)
        {
            _performanceMonitor = performanceMonitor;
            _gpu = gpu;
        }

        public void UpdateMetrics()
        {
            _metrics?.Dispose();
            _metrics = _performanceMonitor.GetGPUMetrics(_gpu);
        }

        public void Dispose() => _metrics?.Dispose();

        public GPUMetrics Current => _metrics;
    }
}
