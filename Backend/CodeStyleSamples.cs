// Code Style Samples for B2Connect Backend
// This file demonstrates StyleCop-compliant code patterns
// Based on backend/stylecop.json configuration

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2Connect.Backend.CodeStyleSamples
{
    /// <summary>
    /// Sample class demonstrating proper StyleCop compliance.
    /// Note: Documentation is disabled in stylecop.json, but shown here for completeness.
    /// </summary>
    public class SampleService
    {
        private readonly ILogger<SampleService> _logger;
        private readonly IRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="repository">The repository instance.</param>
        public SampleService(ILogger<SampleService> logger, IRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Sample method demonstrating proper async pattern and error handling.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<SampleEntity> GetByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Retrieving entity with ID: {Id}", id);

                var entity = await _repository.GetByIdAsync(id);

                if (entity == null)
                {
                    _logger.LogWarning("Entity with ID {Id} not found", id);
                    throw new NotFoundException($"Entity with ID {id} not found");
                }

                return entity;
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error retrieving entity with ID {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Sample method with LINQ demonstrating proper formatting.
        /// </summary>
        /// <param name="filter">The filter criteria.</param>
        /// <returns>A collection of filtered entities.</returns>
        public IEnumerable<SampleEntity> GetFiltered(string filter)
        {
            return _repository.GetAll()
                .Where(e => e.Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                .OrderBy(e => e.Name)
                .ToList();
        }
    }

    /// <summary>
    /// Sample entity demonstrating proper property ordering and naming.
    /// </summary>
    public class SampleEntity
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the creation timestamp.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last modification timestamp.
        /// </summary>
        public DateTime? ModifiedAt { get; set; }

        private string _privateField;

        /// <summary>
        /// Sample method demonstrating proper formatting.
        /// </summary>
        public void SampleMethod()
        {
            // Method implementation
            _privateField = "value";

            if (string.IsNullOrEmpty(Name))
            {
                throw new ArgumentException("Name cannot be null or empty", nameof(Name));
            }
        }
    }

    /// <summary>
    /// Sample interface demonstrating proper interface formatting.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Gets an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        Task<SampleEntity> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <returns>A collection of all entities.</returns>
        IQueryable<SampleEntity> GetAll();
    }

    /// <summary>
    /// Sample exception class.
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public NotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}