﻿using Microsoft.EntityFrameworkCore;
using NightTasker.Passport.Application.ApplicationContracts.Persistence;
using NightTasker.Passport.Domain.Entities.User;
using NightTasker.Passport.Infrastructure.Repositories.Base;

namespace NightTasker.Passport.Infrastructure.Repositories;

/// <inheritdoc cref="IUserRefreshTokenRepository"/> 
internal class UserRefreshTokenRepository : BaseRepository<UserRefreshToken, Guid>, IUserRefreshTokenRepository
{
    
    public UserRefreshTokenRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    /// <inheritdoc /> 
    public async Task<Guid> CreateToken(Guid userId, CancellationToken cancellationToken)
    {
        var token = new UserRefreshToken() { IsValid = true, UserId = userId };
        await Add(token, cancellationToken);
        return token.Id;
    }
    
    /// <inheritdoc /> 
    public Task<UserRefreshToken?> TryGetValidRefreshToken(Guid refreshTokenId, CancellationToken cancellationToken)
    {
        return Table.FirstOrDefaultAsync(x => x.Id == refreshTokenId && x.IsValid, cancellationToken);
    }
    
    /// <inheritdoc /> 
    public async Task<User?> TryGetUserByRefreshToken(Guid refreshTokenId, CancellationToken cancellationToken)
    {
        var refreshToken = await Table
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == refreshTokenId, cancellationToken);

        return refreshToken?.User;
    }
}