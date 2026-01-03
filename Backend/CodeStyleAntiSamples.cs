// Code Style Anti-Samples for B2Connect Backend
// This file demonstrates common StyleCop violations
// DO NOT use these patterns in production code

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace B2Connect.Backend.CodeStyleAntiSamples
{
    // SA1200: Using directives should be placed outside namespace
    // VIOLATION: Usings inside namespace
    public class BadService
    {
        // SA1401: Fields should be private
        // VIOLATION: Public field
        public string publicField;

        // SA1309: Field names should not use Hungarian notation
        // VIOLATION: Hungarian prefix (if not allowed)
        private string strName;

        // SA1202: Elements should be ordered by access
        // VIOLATION: Wrong order (public after private)
        private readonly ILogger<BadService> _logger;

        public BadService(ILogger<BadService> logger)
        {
            // SA1101: Prefix local calls with this
            // VIOLATION: Missing 'this.' for clarity
            _logger = logger;
        }

        // SA1201: Elements should appear in the correct order
        // VIOLATION: Methods before properties
        public async Task<BadEntity> GetByIdAsync(Guid id)
        {
            // SA1503: Braces should not be omitted
            // VIOLATION: Missing braces
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid ID");

            // SA1513: Closing brace should be followed by blank line
            // VIOLATION: No blank line after brace
            return await Task.FromResult(new BadEntity());
        }

        // SA1505: Opening braces should not be followed by blank line
        // VIOLATION: Blank line after opening brace
        public void BadMethod()
        {

            var result = "test";
            // SA1508: Closing braces should not be preceded by blank line
            // VIOLATION: Blank line before closing brace

        }

        // SA1600: Elements should be documented
        // VIOLATION: Missing documentation (if enabled)
        public string UndocumentedProperty { get; set; }
    }

    // SA1402: File may only contain a single type
    // VIOLATION: Multiple types in one file
    public class BadEntity
    {
        // SA1516: Elements should be separated by blank line
        // VIOLATION: No separation between elements
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }

        // SA1501: Statement should not be on a single line
        // VIOLATION: Multiple statements on one line
        public void BadFormatting() { var x = 1; var y = 2; }
    }

    // SA1300: Element should begin with upper case letter
    // VIOLATION: Wrong casing
    public class bad_interface
    {
        // SA1302: Interface names should begin with I
        // VIOLATION: Missing I prefix
        public interface BadInterface
        {
            // SA1304: Non-private readonly fields should begin with upper case letter
            // VIOLATION: Wrong casing
            readonly string badField;
        }
    }
}</ content >
< parameter name = "filePath" >/ Users / holger / Documents / Projekte / B2Connect / backend / CodeStyleAntiSamples.cs