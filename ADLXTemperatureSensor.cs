using ADLXWrapper;
using FanControl.Plugins;
using System;

namespace FanControl.ADLX
{
    public class ADLXTemperatureSensor : IPluginSensor
    {
        private readonly Func<double> _getTemp;

        public ADLXTemperatureSensor(string name, GPU gpu, Func<Double> getTemp)
        {

            Name = gpu.Name;
            Id = $"ADLX/Temperature/{gpu.Name}/{name}"; // PUT SOME KIND OF UNIQUE ID

            Update();
            _getTemp = getTemp;
        }

        public string Id { get; }
        public string Name { get; }
        public float? Value { get; private set; }

        public void Update()
        {
            Value = (float)_getTemp();
        }
    }
}
