using ADLXWrapper;
using FanControl.Plugins;
using System;

namespace FanControl.ADLX
{
    public class ADLXControl : IPluginControlSensor
    {
        private readonly ManualFanTuning _fanTuning;
        private readonly bool _initialZeroRPM = false;
        private bool _zeroRPMState;
        private int[] _initalState;

        public ADLXControl(GPU gpu, ManualFanTuning fanTuning)
        {
            Name = gpu.Name;
            Id = $"ADLX/{gpu.Name}/{gpu.UniqueId}/Control";
            _fanTuning = fanTuning;

            _initalState = _fanTuning.GetCurrentState();

            if (fanTuning.SupportsZeroRPM)
            {
                _initialZeroRPM = _fanTuning.GetZeroRPMState();
                _zeroRPMState = _initialZeroRPM;
            }
        }

        public string Id { get; }
        public string Name { get; }
        public float? Value { get; private set; } = null;

        private void SetZeroRPM(bool enabled)
        {
            if (!_fanTuning.SupportsZeroRPM)
                return;

            _fanTuning.SetZeroRPM(enabled);
            _zeroRPMState = enabled;
        }

        public void Reset()
        {
            if (_fanTuning.SupportsZeroRPM)
                SetZeroRPM(_initialZeroRPM);

            _fanTuning.SetFanTuningStates(_initalState);
        }

        public void Set(float val)
        {
            var roundedVal = (int)Math.Round(val);

            if (roundedVal == 0)
                SetZeroRPM(true);
            else
            {
                if (_zeroRPMState)
                    SetZeroRPM(false);
            }

            _fanTuning.SetTargetFanSpeed(GetRPM(val));
            Value = roundedVal;
        }

        private int GetRPM(float val) => (int)(_fanTuning.SpeedRange.Max * (val / 100f));

        public void Update() { /* nothing to update */ }
    }
}
