using System.Diagnostics.Metrics;

namespace Demoulas.ProfitSharing.Api
{
    public class MetricLogger : IHostedService
    {
        private readonly ILogger<MetricLogger> _logger;
        private readonly MeterListener _listener;

        public MetricLogger(ILogger<MetricLogger> logger)
        {
            _logger = logger;

            _listener = new MeterListener
            {
                InstrumentPublished = (instrument, listener) =>
                {
                    // Log instrument metadata
                    _logger.LogInformation("Discovered Instrument: {Name}, Type: {Type}, Unit: {Unit}, Description: {Description}",
                        instrument.Name,
                        instrument.GetType().Name,
                        instrument.Unit ?? "none",
                        instrument.Description ?? "none");

                    // Hook into the measurement callbacks based on instrument type
                    switch (instrument)
                    {
                        case ObservableCounter<long>:
                        case Counter<long>:
                        case ObservableGauge<long>:
                        case Histogram<long>:
                            listener.EnableMeasurementEvents(instrument, (ref Measurement<long> measurement, object? state) =>
                            {
                                _logger.LogInformation("📏 {InstrumentName} (long): {Value}", instrument.Name, measurement.Value);
                            });
                            break;

                        case ObservableCounter<int>:
                        case Counter<int>:
                        case ObservableGauge<int>:
                        case Histogram<int>:
                            listener.EnableMeasurementEvents(instrument, (ref Measurement<int> measurement, object? state) =>
                            {
                                _logger.LogInformation("📏 {InstrumentName} (int): {Value}", instrument.Name, measurement.Value);
                            });
                            break;

                        case ObservableCounter<double>:
                        case Counter<double>:
                        case ObservableGauge<double>:
                        case Histogram<double>:
                            listener.EnableMeasurementEvents(instrument, (ref Measurement<double> measurement, object? state) =>
                            {
                                _logger.LogInformation("📏 {InstrumentName} (double): {Value}", instrument.Name, measurement.Value);
                            });
                            break;

                        case ObservableCounter<float>:
                        case Counter<float>:
                        case ObservableGauge<float>:
                        case Histogram<float>:
                            listener.EnableMeasurementEvents(instrument, (ref Measurement<float> measurement, object? state) =>
                            {
                                _logger.LogInformation("📏 {InstrumentName} (float): {Value}", instrument.Name, measurement.Value);
                            });
                            break;

                        default:
                            listener.EnableMeasurementEvents(instrument);
                            _logger.LogWarning("⚠️ Unsupported instrument type: {Name}, {Type}", instrument.Name, instrument.GetType().Name);
                            break;
                    }
                }
            };
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _listener.Start();
            _logger.LogInformation("✅ Metric discovery and logging started.");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _listener.Dispose();
            _logger.LogInformation("🛑 Metric listener stopped.");
            return Task.CompletedTask;
        }
    }
}
