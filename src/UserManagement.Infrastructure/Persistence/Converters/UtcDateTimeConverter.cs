using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace UserManagement.Infrastructure.Persistence.Converters;

public sealed class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
    toDatabase => toDatabase,
    fromDatabase => DateTime.SpecifyKind(fromDatabase, DateTimeKind.Utc));