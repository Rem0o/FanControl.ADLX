using ADLXWrapper;
using FanControl.Plugins;
using System;

namespace FanControl.ADLX
{
    public class ADLXControl : IPluginControlSensor
    {
        private readonly ManualFanTuning _fanTuning;
        private readonly bool _initialZeroRPM = false;
        private bool _zeroRPMSet;

        public ADLXControl(GPU gpu, ManualFanTuning fanTuning)
        {
            Name = gpu.Name;
            Id = $"ADLX/Control/{gpu.Name}"; // PUT SOME KIND OF UNIQUE ID
            _fanTuning = fanTuning;

            _initialZeroRPM = _fanTuning.GetZeroRPMState();
            _zeroRPMSet = _initialZeroRPM;
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
            if (val == 0)
            {
                _zeroRPMSet = true;
                _fanTuning.SetZeroRPM(true);
            }
            else if (_zeroRPMSet)
            {
                _fanTuning.SetZeroRPM(false);
            }

            if (_fanTuning.SpeedRange.Min > val)
            {
                val = _fanTuning.SpeedRange.Min;
            }

            _fanTuning.SetFanSpeed((int)Math.Round(val));
            Value = val;
        }

        public void Update()
        {

            // nothing to update
        }
    }
}
