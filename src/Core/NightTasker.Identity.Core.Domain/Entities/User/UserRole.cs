﻿using Microsoft.AspNetCore.Identity;
using NightTasker.Common.Core.Abstractions;

namespace NightTasker.Identity.Domain.Entities.User;

/// <summary>
/// Роль пользователя.
/// </summary>
public class UserRole : IdentityUserRole<Guid>, IEntity, IDateTimeOffsetModification
{
    /// <inheritdoc />
    public DateTimeOffset CreatedDateTimeOffset { get; set; }
    
    /// <inheritdoc />
    public DateTimeOffset UpdatedDateTimeOffset { get; set; }
}