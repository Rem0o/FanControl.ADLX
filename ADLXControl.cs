using ADLXWrapper;
using FanControl.Plugins;
using System;

namespace FanControl.ADLX
{
    public class ADLXControl : IPluginControlSensor
    {
        private readonly ManualFanTuning _fanTuning;

        public ADLXControl(GPU gpu, ManualFanTuning fanTuning)
        {
            Name = gpu.Name;
            Id = $"ADLX/Control/{gpu.Name}"; // PUT SOME KIND OF UNIQUE ID
            _fanTuning = fanTuning;
        }

        public string Id { get; }
        public string Name { get; }
        public float? Value { get; private set; } = null;

        public void Reset()
        {
            // TODO
        }

        public void Set(float val)
        {
            _fanTuning.SetFanSpeed((int)Math.Round(val));
            Value = val;
        }

        public void Update()
        {
            // nothing to update
        }
    }
}
