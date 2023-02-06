using ADLXWrapper;
using FanControl.Plugins;

namespace FanControl.ADLX
{
    public class ADLXFanSensor : IPluginSensor
    {
        private GPUMetrics _metrics;

        public ADLXFanSensor( GPU gpu, GPUMetrics metrics )
        {
            _metrics = metrics;

            Name = gpu.Name;
            Id = $"ADLX/Fan/{gpu.Name}"; // PUT SOME KIND OF UNIQUE ID

            Update();
        }

        public string Id { get; }
        public string Name { get; }
        public float? Value { get; private set; }

        public void Update()
        {
            Value = _metrics.GetFanSpeed();
        }
    }
}
