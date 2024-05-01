using ADLXWrapper;
using System;

namespace FanControl.ADLX
{
    public class GPUMetricsProvider
    {
        private readonly PerformanceMonitor _performanceMonitor;
        private readonly GPU _gpu;
        private GPUMetricsStruct _metrics;

        public GPUMetricsProvider(PerformanceMonitor performanceMonitor, GPU gpu)
        {
            _performanceMonitor = performanceMonitor;
            _gpu = gpu;
            UpdateMetrics();
        }

        public void UpdateMetrics()
        {
            _metrics = _performanceMonitor.GetGPUMetricsStructFromTracking(_gpu);
        }


        public GPUMetricsStruct Current => _metrics;
    }
}
