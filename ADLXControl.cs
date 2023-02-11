using ADLXWrapper;
using FanControl.Plugins;
using System;

namespace FanControl.ADLX
{
    public class ADLXControl : IPluginControlSensor
    {
        private readonly ManualFanTuning _fanTuning;
        private readonly bool _initialZeroRPM = false;

        public ADLXControl(GPU gpu, ManualFanTuning fanTuning)
        {
            Name = gpu.Name;
            Id = $"ADLX/Control/{gpu.Name}"; // PUT SOME KIND OF UNIQUE ID
            _fanTuning = fanTuning;

            _initialZeroRPM = _fanTuning.GetZeroRPMState();
            _fanTuning.SetZeroRPM(true);
        }

        public string Id { get; }
        public string Name { get; }
        public float? Value { get; private set; } = null;

        public void Reset()
        {
            _fanTuning.SetZeroRPM(_initialZeroRPM);
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
