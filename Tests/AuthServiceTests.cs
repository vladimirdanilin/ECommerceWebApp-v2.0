using AuthMicroService.Data.Services;
using AuthMicroService.Models;
using Castle.Core.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnSuccess()
        {
            //Arrange
            var mockUserStore = new Mock<IUserStore<User>>();
            var mockUserManager = new Mock<UserManager<User>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            var mockHttpContextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var mockClaimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();

            var mockSignInManager = new Mock<SignInManager<User>>(
                mockUserManager.Object,
                mockHttpContextAccessor.Object,
                mockClaimsFactory.Object,
                null, null, null, null
            );

            var mockConfigurationSectionKey = new Mock<IConfigurationSection>();
            mockConfigurationSectionKey.Setup(a => a.Value).Returns("SuperSecretKeyForTestingPurposesOnly");

            var mockConfigurationSectionIssuer = new Mock<IConfigurationSection>();
            mockConfigurationSectionIssuer.Setup(a => a.Value).Returns("TestIssuer");

            var mockConfigurationSectionAudience = new Mock<IConfigurationSection>();
            mockConfigurationSectionAudience.Setup(a => a.Value).Returns("TestAudience");


            var mockConfiguration = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            mockConfiguration.Setup(c => c.GetSection("JWT:Key")).Returns(mockConfigurationSectionKey.Object);
            mockConfiguration.Setup(c => c.GetSection("JWT:Issuer")).Returns(mockConfigurationSectionIssuer.Object);
            mockConfiguration.Setup(c => c.GetSection("JWT:Audience")).Returns(mockConfigurationSectionAudience.Object);

            mockConfiguration.Setup(c => c["JWT:Key"]).Returns("SuperSecretKeyForTestingPurposesOnly");
            mockConfiguration.Setup(c => c["JWT:Issuer"]).Returns("TestIssuer");
            mockConfiguration.Setup(c => c["JWT:Audience"]).Returns("TestAudience");


            var loginModel = new LoginModel 
            {
                Email = "testuser@example.com",
                Password = "Test123!"
            };

            var user = new User
            { 
                Id = 1,
                Email = loginModel.Email,
                UserName = loginModel.Email
            };

            mockSignInManager.Setup(x => x.PasswordSignInAsync(loginModel.Email, loginModel.Password, false, false))
                .ReturnsAsync(SignInResult.Success);

            mockUserManager.Setup(x => x.FindByEmailAsync(loginModel.Email))
                .ReturnsAsync(user);

            mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Customer" });

            var authService = new AuthService(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockConfiguration.Object
            );

            //Act
            var result = await authService.LoginUser(loginModel);

            //Assert
            Assert.NotNull(result);
            Assert.DoesNotContain("Login Was NOT Successful", result);
        }

        [Fact]
        public async Task LoginAsync_InvalidCredentials_ReturnsFailureMessage()
        {
            //Arrange
            var mockUserStore = new Mock<IUserStore<User>>();
            var mockUserManager = new Mock<UserManager<User>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            var mockHttpContextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var mockClaimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();

            var mockSignInManager = new Mock<SignInManager<User>>(
                mockUserManager.Object,
                mockHttpContextAccessor.Object,
                mockClaimsFactory.Object,
                null, null, null, null
            );

            var mockConfigurationSectionKey = new Mock<IConfigurationSection>();
            mockConfigurationSectionKey.Setup(a => a.Value).Returns("SuperSecretKeyForTestingPurposesOnly");

            var mockConfigurationSectionIssuer = new Mock<IConfigurationSection>();
            mockConfigurationSectionIssuer.Setup(a => a.Value).Returns("TestIssuer");

            var mockConfigurationSectionAudience = new Mock<IConfigurationSection>();
            mockConfigurationSectionAudience.Setup(a => a.Value).Returns("TestAudience");


            var mockConfiguration = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            mockConfiguration.Setup(c => c.GetSection("JWT:Key")).Returns(mockConfigurationSectionKey.Object);
            mockConfiguration.Setup(c => c.GetSection("JWT:Issuer")).Returns(mockConfigurationSectionIssuer.Object);
            mockConfiguration.Setup(c => c.GetSection("JWT:Audience")).Returns(mockConfigurationSectionAudience.Object);

            mockConfiguration.Setup(c => c["JWT:Key"]).Returns("SuperSecretKeyForTestingPurposesOnly");
            mockConfiguration.Setup(c => c["JWT:Issuer"]).Returns("TestIssuer");
            mockConfiguration.Setup(c => c["JWT:Audience"]).Returns("TestAudience");


            var loginModel = new LoginModel
            {
                Email = "testuser@example.com",
                Password = "Test123!"
            };

            var user = new User
            {
                Id = 1,
                Email = loginModel.Email,
                UserName = loginModel.Email
            };

            mockSignInManager.Setup(x => x.PasswordSignInAsync(loginModel.Email, loginModel.Password, false, false))
                .ReturnsAsync(SignInResult.Success);

            mockSignInManager.Setup(x => x.PasswordSignInAsync(loginModel.Email, loginModel.Password, false, false))
                .ReturnsAsync(SignInResult.Failed);

            var authService = new AuthService(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockConfiguration.Object
            );

            //Act
            var result = await authService.LoginUser(loginModel);

            //Assert
            Assert.Equal("Login Was NOT Successful", result);
        }

        [Fact]
        public async Task RegisterUser_ValidData_ReturnsTrue()
        {
            var mockUserStore = new Mock<IUserStore<User>>();
            var mockUserManager = new Mock<UserManager<User>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            mockUserManager.Setup(x => x.NormalizeEmail(It.IsAny<string>()))
                .Returns((string email) => email.ToUpperInvariant());

            mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var mockHttpContextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var mockClaimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();

            var mockSignInManager = new Mock<SignInManager<User>>(
                mockUserManager.Object,
                mockHttpContextAccessor.Object,
                mockClaimsFactory.Object,
                null, null, null, null
            );

            var mockConfiguration = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();

            var authService = new AuthService(
                mockUserManager.Object,
                mockSignInManager.Object,
                mockConfiguration.Object
            );

            var registerModel = new RegisterModel
            {
                Email = "testuser@example.com",
                
                Password = "Test123!",
                FirstName = "Test",
                LastName = "Test",
                ConfirmPassword = "Test123!"
            };

            var result = await authService.RegisterUser(registerModel);

            Assert.True(result);



        }
    }
}
