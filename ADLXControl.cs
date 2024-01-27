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

        public ADLXControl(GPU gpu, ManualFanTuning fanTuning)
        {
            Name = gpu.Name;
            Id = $"ADLX/{gpu.Name}/{gpu.UniqueId}/Control";
            _fanTuning = fanTuning;

            _initialZeroRPM = _fanTuning.GetZeroRPMState();
            _zeroRPMState = _initialZeroRPM;
        }

        public string Id { get; }
        public string Name { get; }
        public float? Value { get; private set; } = null;

        private void SetZeroRPM(bool val)
        {
            _fanTuning.SetZeroRPM(val);
            _zeroRPMState = val;
        }

        public void Reset()
        {
            this.SetZeroRPM(_initialZeroRPM);
            // TODO
        }

        public void Set(float val)
        {
            if (val == 0)
            {
                this.SetZeroRPM(true);
            }
            else
            {
                if (_zeroRPMState)
                {
                    this.SetZeroRPM(false);
                }

                if (_fanTuning.SpeedRange.Min > val)
                {
                    val = _fanTuning.SpeedRange.Min;
                }
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
