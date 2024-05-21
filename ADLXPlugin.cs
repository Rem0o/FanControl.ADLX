using ADLXWrapper;
using FanControl.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FanControl.ADLX
{
    public class ADLXPlugin : IPlugin2
    {
        private object _lock = new object();
        private readonly IPluginLogger _pluginLogger;
        private readonly IPluginDialog _pluginDialog;
        private ADLXWrapper.ADLXWrapper _wrapper;
        private SystemServices _system;
        private IReadOnlyList<GPU> _gpus;
        private GPUTuningService _tuning;
        private PerformanceMonitor _perf;
        private Dictionary<int, ManualFanTuning> _fans;
        private IDisposable _tracking;
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
            lock (_lock)
            {
                if (!_initialized)
                    return;

                DisposeAll();

                _initialized = false;
            }
        }

        public void Initialize()
        {
            try
            {
                lock (_lock)
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
                            Log($"Manual fan tuning for {g.Name} is not supported");

                        return supported;
                    }).ToDictionary(x => x.UniqueId, x => _tuning.GetManualFanTuning(x));

                    _tracking = _perf.StartTracking(1000, 50);
                    _metricsProviders = _gpus.Select(x => new GPUMetricsProvider(_perf, x)).ToArray();
                    _initialized = true;
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                DisposeAll();
                _initialized = false;
            }
        }

        public void Load(IPluginSensorsContainer _container)
        {
            lock (_lock)
            {
                if (!_initialized)
                {
                    return;
                }

                ADLXControl[] controls = _gpus.Where(x => _fans.ContainsKey(x.UniqueId)).Select(x => new ADLXControl(x, _fans[x.UniqueId])).ToArray();
                ADLXFanSensor[] fanSensors = _gpus.Zip(_metricsProviders, (gpu, m) => new ADLXFanSensor(gpu, m)).ToArray();
                ADLXTemperatureSensor[] hotspots = _gpus.Zip(_metricsProviders, (gpu, m) => new ADLXTemperatureSensor("Hotspot", gpu, () => m.Current.GPUHotspotTemperature)).ToArray();
                ADLXTemperatureSensor[] gpuTemps = _gpus.Zip(_metricsProviders, (gpu, m) => new ADLXTemperatureSensor("GPU", gpu, () => m.Current.GPUTemperature)).ToArray();

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
        }

        public void Update()
        {
            lock (_lock)
            {
                if (!_initialized) return;

                foreach (var provider in _metricsProviders)
                    provider.UpdateMetrics();
            }
        }

        private void Log(string message)
        {
            _pluginLogger.Log($"ADLX plugin: {message}");
        }

        private void DisposeAll()
        {
            _tracking?.Dispose();
            _fans?.Values.ToList().ForEach(x => x.Dispose());
            _perf?.Dispose();
            _tuning?.Dispose();
            _gpus?.ToList().ForEach(x => x.Dispose());
            _system?.Dispose();
            _wrapper?.Dispose();
        }
    }
}
