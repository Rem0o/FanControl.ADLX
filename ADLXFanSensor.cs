using ADLXWrapper;
using FanControl.Plugins;

namespace FanControl.ADLX
{
    public class ADLXFanSensor : IPluginSensor
    {
        private PerformanceMonitor _perf;
        private GPU _gpu;

        public ADLXFanSensor(GPU gpu, PerformanceMonitor perf)
        {
            _perf = perf;
            _gpu = gpu;

            Name = gpu.Name;
            Id = $"ADLX/Fan/{gpu.Name}"; // PUT SOME KIND OF UNIQUE ID

            Update();
        }

        public string Id { get; }
        public string Name { get; }
        public float? Value { get; private set; }

        public void Update()
        {
            using (var metrics = _perf.GetGPUMetrics(_gpu))
            {
                Value = metrics.GetFanSpeed();
            }
        }
    }
}
