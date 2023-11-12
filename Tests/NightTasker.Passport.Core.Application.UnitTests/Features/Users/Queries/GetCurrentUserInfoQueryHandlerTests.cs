﻿using Bogus;
using FluentAssertions;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using NightTasker.Passport.Application.ApplicationContracts.Identity;
using NightTasker.Passport.Application.ApplicationContracts.Persistence;
using NightTasker.Passport.Application.Exceptions.BadRequest;
using NightTasker.Passport.Application.Exceptions.Unauthorized;
using NightTasker.Passport.Application.Features.Users.Commands.Create;
using NightTasker.Passport.Application.Features.Users.Models;
using NightTasker.Passport.Application.Features.Users.Queries.GetCurrentUserInfo;
using NightTasker.Passport.Domain.Entities.User;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace NightTasker.Passport.Core.Application.UnitTests.Features.Users.Queries;

[TestFixture]
public class GetCurrentUserInfoQueryHandlerTests
{
    private Faker _faker = null!;
    private GetCurrentUserInfoQueryHandler _sut = null!;
    private IIdentityService _identityService = null!;
    private IMapper _mapper = null!;
    private IUnitOfWork _unitOfWork = null!;

    [SetUp]
    public void Setup()
    {
        _faker = new Faker();
        _identityService = Substitute.For<IIdentityService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _sut = new GetCurrentUserInfoQueryHandler(_identityService, _unitOfWork, _mapper);
    }

    [TestCase]
    public void Handle_UserIsNotAuthenticated_CurrentUserIsNotAuthenticatedUnauthorizedException()
    {
        // Arrange
        _identityService.IsAuthenticated.Returns(false);
        var query = SetupQuery();
        
        // Act
        async Task TestDelegate() => await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.ThrowsAsync<CurrentUserIsNotAuthenticatedUnauthorizedException>(TestDelegate);
    }
    
    [TestCase]
    public void Handle_UserIsAuthenticatedAndCurrentUserIdIsNull_CurrentUserIsNotAuthenticatedUnauthorizedException()
    {
        // Arrange
        _identityService.IsAuthenticated.Returns(true);
        _identityService.CurrentUserId.ReturnsNull();
        var query = SetupQuery();
        
        // Act
        async Task TestDelegate() => await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.ThrowsAsync<CurrentUserIsNotAuthenticatedUnauthorizedException>(TestDelegate);
    }
    
    [TestCase]
    public void Handle_InvalidUserId_InvalidOperationException()
    {
        // Arrange
        _identityService.IsAuthenticated.Returns(true);
        _identityService.CurrentUserId.Returns(_faker.Random.Guid());
        _unitOfWork.UserRepository.TryGetById(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();
        var query = SetupQuery();
        
        // Act
        async Task TestDelegate() => await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.ThrowsAsync<InvalidOperationException>(TestDelegate);
    }
    
    private GetCurrentUserInfoQuery SetupQuery()
    {
        return new GetCurrentUserInfoQuery();
    }
}