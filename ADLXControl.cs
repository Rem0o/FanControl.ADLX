using ADLXWrapper;
using FanControl.Plugins;
using System;

namespace FanControl.ADLX
{
    public class ADLXControl : IPluginControlSensor
    {
        private readonly ManualFanTuning _fanTuning;
        private bool _zeroRPMState;
        private bool _supportTargetFanSpeed;

        public ADLXControl(GPU gpu, ManualFanTuning fanTuning)
        {
            Name = gpu.Name;
            Id = $"ADLX/{gpu.Name}/{gpu.UniqueId}/Control";
            _fanTuning = fanTuning;

            if (fanTuning.SupportsTargetFanSpeed && fanTuning.SpeedRange.Max > 0)
            {
                _supportTargetFanSpeed = true;
            }
        }

        public string Id { get; }
        public string Name { get; }
        public float? Value { get; private set; } = null;

        private void SetZeroRPM(bool enabled)
        {
            if (!_fanTuning.SupportsZeroRPM)
                return;

            if (enabled == _zeroRPMState)
                return;

            _fanTuning.SetZeroRPM(enabled);
            _zeroRPMState = enabled;
        }

        public void Reset()
        {
            _fanTuning.Reset();
        }

        public void Set(float val)
        {
            var roundedVal = (int)Math.Round(val);

            if (roundedVal == 0)
                SetZeroRPM(true);
            else
                SetZeroRPM(false);

            if (roundedVal == Value)
                return;

            if (_supportTargetFanSpeed)
                _fanTuning.SetTargetFanSpeed(GetRPM(val));
            else
                _fanTuning.SetFanTuningStates2(roundedVal);

            Value = roundedVal;
        }

        private int GetRPM(float val) => (int)(_fanTuning.SpeedRange.Max * (val / 100f));

        public void Update() { /* nothing to update */ }
    }
}
