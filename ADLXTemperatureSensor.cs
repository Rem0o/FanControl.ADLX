using ADLXWrapper;
using FanControl.Plugins;
using System;

namespace FanControl.ADLX
{
    public class ADLXTemperatureSensor : IPluginSensor
    {
        private readonly Func<double> _getTemp;

        public ADLXTemperatureSensor(string name, GPU gpu, Func<double> getTemp)
        {
            Name = $"{name} - {gpu.Name}";
            Id = $"ADLX/{gpu.Name}/{gpu.UniqueId}/Temperature/{name}";
            _getTemp = getTemp;

            Update();
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
