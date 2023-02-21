using ADLXWrapper;
using FanControl.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FanControl.ADLX
{
    public class ADLXPlugin : IPlugin2
    {
        private readonly IPluginLogger _pluginLogger;
        private readonly IPluginDialog _pluginDialog;
        private ADLXWrapper.ADLXWrapper _wrapper;
        private SystemServices _system;
        private IReadOnlyList<GPU> _gpus;
        private GPUTuningService _tuning;
        private PerformanceMonitor _perf;
        private ManualFanTuning[] _fans;

        private bool _initialized;
        private GPUMetricsProvider[] _metricsProviders;

        public ADLXPlugin(IPluginLogger pluginLogger, IPluginDialog pluginDialog)
        {
            _pluginLogger = pluginLogger;
            _pluginDialog = pluginDialog;
        }

        public string Name => "ADLX";

        public void Close()
        {
            lock (this)
            {
                _initialized = false;

                _metricsProviders.ToList().ForEach(x => x.Dispose());
                _fans?.ToList().ForEach(x => x.Dispose());
                _perf?.Dispose();
                _tuning?.Dispose();
                _gpus?.ToList().ForEach(x => x.Dispose());
                _system?.Dispose();
                _wrapper?.Dispose();
            }
        }

        public void Initialize()
        {
            try
            {
                lock (this)
                {
                    _wrapper = new ADLXWrapper.ADLXWrapper();
                    _system = _wrapper.GetSystemServices();
                    _gpus = _system.GetGPUs();

                    _tuning = _system.GetGPUTuningService();
                    _perf = _system.GetPerformanceMonitor();

                    _fans = _gpus.Where(g =>
                    {
                        var supported = _tuning.IsManualFanTuningSupported(g);

                        if (!supported)
                        {
                            _pluginLogger.Log($"Manual fan tuning for {g.Name} is not supported");
                        }
                        return supported;
                    }).Select(_tuning.GetManualFanTuning).ToArray();

                    _metricsProviders = _gpus.Select(x => new GPUMetricsProvider(_perf, x)).ToArray();
                    _initialized = true;
                }
            }
            catch (Exception ex)
            {
                _pluginLogger.Log(ex.ToString());
                Close();
            }
        }

        public void Load(IPluginSensorsContainer _container)
        {
            if (!_initialized)
            {
                return;
            }

            ADLXControl[] controls = _gpus.Zip(_fans, (gpu, fan) => new ADLXControl(gpu, fan)).ToArray();
            ADLXFanSensor[] fanSensors = _gpus.Zip(_metricsProviders, (gpu, m) => new ADLXFanSensor(gpu, m)).ToArray();
            ADLXTemperatureSensor[] hotspots = _gpus.Zip(_metricsProviders, (gpu, m) => new ADLXTemperatureSensor("Hotspot", gpu, () => m.Current.GetHotspotTemperature())).ToArray();
            ADLXTemperatureSensor[] gpuTemps = _gpus.Zip(_metricsProviders, (gpu, m) => new ADLXTemperatureSensor("GPU", gpu, () => m.Current.GetGPUTemperature())).ToArray();

            foreach (var control in controls)
            {
                _container.ControlSensors.Add(control);
            }

            foreach (var fan in fanSensors)
            {
                _container.FanSensors.Add(fan);
            }

            foreach (var temp in hotspots.Concat(gpuTemps))
            {
                _container.TempSensors.Add(temp);
            }
        }

        public void Update()
        {
            if (!_initialized) return;

            foreach (var provider in _metricsProviders)
                provider.UpdateMetrics();
        }
    }
}
