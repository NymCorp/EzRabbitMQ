using FluentValidation;

namespace EzRabbitMQ
{
    /// <summary>
    /// Fanout producer options
    /// </summary>
    public record FanoutProducerOptions : IProducerOptions
    {
        /// <summary>
        /// Fanout producer options
        /// </summary>
        /// <param name="exchangeName">Exchange name, fallback on default exchange name if not defined</param>
        public FanoutProducerOptions(string? exchangeName = null)
        {
            ExchangeName = exchangeName ?? ExchangeType.Fanout.Name();
        }

        /// <inheritdoc />
        public string RoutingKey => string.Empty;

        /// <inheritdoc />
        public string ExchangeName { get; }

        /// <inheritdoc />
        public ProducerProperties Properties { get; } = new();
    }
    
    // ReSharper disable once UnusedType.Global
    internal class FanoutProducerOptionsValidator : AbstractValidator<FanoutProducerOptions>
    {
        // no validation needed
    }
}