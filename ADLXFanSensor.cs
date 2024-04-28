using ADLXWrapper;
using FanControl.Plugins;

namespace FanControl.ADLX
{
    public class ADLXFanSensor : IPluginSensor
    {
        private GPUMetricsProvider _metrics;

        public ADLXFanSensor(GPU gpu, GPUMetricsProvider metricsProvider)
        {
            _metrics = metricsProvider;

            Name = gpu.Name;
            Id = $"ADLX/{gpu.Name}/{gpu.UniqueId}/fan";

            Update();
        }

        public string Id { get; }
        public string Name { get; }
        public float? Value { get; private set; }

        public void Update()
        {
            Value = _metrics.Current.GPUFanSpeed;
        }
    }
}
