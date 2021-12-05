using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace EzRabbitMQ
{
    /// <summary>
    /// Validation service will initialize validator at start and plays validation again instance
    /// </summary>
    public class ValidationService
    {
        private readonly ILogger<ValidationService> _logger;

        /// <summary>
        /// Contains registered validators instances
        /// </summary>
        private readonly Dictionary<Type, IValidator> _validators = new();

        /// <summary>
        /// Validation service will seek validator at start and run validation on registered validator types
        /// </summary>
        public ValidationService(ILogger<ValidationService> logger)
        {
            _logger = logger;

            InitializeValidators();
        }

        private void InitializeValidators()
        {
            var validators =
                from validatorType in GetType().Assembly.GetTypes()
                where typeof(IValidator).IsAssignableFrom(validatorType)
                let validatedType = validatorType.BaseType?.GetGenericArguments().FirstOrDefault()
                where validatedType is not null
                select (validatorType, validatedType);

            foreach (var (validatorType, validatedType) in validators)
            {
                var validatorInstance = (IValidator) Activator.CreateInstance(validatorType)!;

                _validators.Add(validatedType!, validatorInstance);
            }
        }

        /// <summary>
        /// ValidateAndThrow will throw an exception on validation fail.
        /// </summary>
        /// <param name="instance">instance you want to validate, the system will try to find an implementation of IValidator validating the instance you are passing.</param>
        /// <typeparam name="T">instance type of the data you want to validate</typeparam>
        /// <exception cref="ValidationException">Thrown when validation fails</exception>
        public void ValidateAndThrow<T>(T instance)
        {
            var instanceType = typeof(T);
            if (_validators.TryGetValue(instanceType, out var validator))
            {
                (validator as IValidator<T>)?.ValidateAndThrow(instance);  
            }
            else
            {
                _logger.LogWarning("Unable to validate instance {InstanceType}, not matching validator found", instanceType);
            }
        }
    }
}