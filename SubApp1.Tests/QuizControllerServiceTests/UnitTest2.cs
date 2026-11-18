using SubApp1.Models;
using SubApp1.ViewModels;
using SubApp1.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

// Nødt til å putte denne her da SignInResult er i Microsoft.AspNetCore.Identity, men vi har også en 
// SignInResult i Microsoft.AspNetCore.Mvc. For å unngå navnekonflikt, aliaser vi den fra Identity.
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace SubApp1.Tests;

// Unit tests for AccountController. 3 positive tests and 3 negative tests for Register, Login, and Logout actions.
public class AccountControllerTests
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        // Mock UserManager and SignInManager using NSubstitute
        _userManager = Substitute.For<UserManager<IdentityUser>>(
            Substitute.For<IUserStore<IdentityUser>>(),
            null, null, null, null, null, null, null, null);

        // Mock SignInManager with necessary dependencies
        _signInManager = Substitute.For<SignInManager<IdentityUser>>(
            _userManager,
            Substitute.For<IHttpContextAccessor>(),
            Substitute.For<IUserClaimsPrincipalFactory<IdentityUser>>(),
            null, null, null, null);

        // Create an instance of AccountController with the mocked dependencies
        _controller = new AccountController(_userManager, _signInManager);

        // Mock the UrlHelper to return a local URL for redirection
        var urlHelper = Substitute.For<IUrlHelper>();
        urlHelper.Action(Arg.Any<Microsoft.AspNetCore.Mvc.Routing.UrlActionContext>())
                 .Returns("/Dashboard");
        urlHelper.IsLocalUrl(Arg.Any<string>()).Returns(x => ((string?)x[0])?.StartsWith("/") == true);
        _controller.Url = urlHelper;
    }

    // Positive test for successful registration
    [Fact]
    public async Task Register_WhenSuccessful_AssignsRoleSignsInAndRedirects()   
    {
        // Arrange
        var model = new RegisterViewModel { Email = "bob@johansen.no", Password = "Pass123!", Role = Role.STUDENT };
        _userManager.CreateAsync(Arg.Any<IdentityUser>(), model.Password)
                    .Returns(IdentityResult.Success);
        _userManager.AddToRoleAsync(Arg.Any<IdentityUser>(), "STUDENT")
                    .Returns(IdentityResult.Success);

        // Act
        var result = await _controller.Register(model);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Dashboard", redirect.ControllerName);
        await _userManager.Received(1).AddToRoleAsync(Arg.Any<IdentityUser>(), "STUDENT");
        await _signInManager.Received(1).SignInAsync(Arg.Any<IdentityUser>(), false);
    }

    // Negative test for failed registration
    [Fact]
    public async Task Register_WhenCreateFails_ReturnsViewWithErrors()            
    {
        // Arrange
        var model = new RegisterViewModel { Email = "bob@johansen.no", Password = "weak", Role = Role.TEACHER };
        _userManager.CreateAsync(Arg.Any<IdentityUser>(), model.Password)
                    .Returns(IdentityResult.Failed(new IdentityError { Description = "Password too short" }));

        // Act
        var result = await _controller.Register(model);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, view.Model);
        Assert.Contains(_controller.ModelState[string.Empty]!.Errors,
                        e => e.ErrorMessage == "Password too short");
        await _signInManager.DidNotReceive().SignInAsync(Arg.Any<IdentityUser>(), Arg.Any<bool>());
    }

    // Positive test for successful login
    [Fact]
    public async Task Login_WhenCredentialsValid_RedirectsToDashboard()           
    {
        // Arrange
        var model = new LoginViewModel { Email = "bob@johansen.no", Password = "Pass123!", RememberMe = false };
        _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false)
                      .Returns(SignInResult.Success);

        // Act
        var result = await _controller.Login(model);

        // Assert
        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/Dashboard", redirect.Url);
    }

    // Negative test for failed login
    [Fact]
    public async Task Login_WhenCredentialsInvalid_ReturnsViewWithError()         
    {
        // Arrange
        var model = new LoginViewModel { Email = "bob@johansen.no", Password = "wrong" };
        _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false)
                      .Returns(SignInResult.Failed);

        // Act
        var result = await _controller.Login(model);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, view.Model);
        Assert.Contains(_controller.ModelState[string.Empty]!.Errors,
                        e => e.ErrorMessage == "Invalid login attempt.");
    }

    // Positive test for successful logout
    [Fact]
    public async Task Logout_SignsOutAndRedirectsToWelcome()
    {
        // Arrange
        _signInManager.SignOutAsync().Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Logout();

        // Assert
        await _signInManager.Received(1).SignOutAsync();
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Welcome", redirect.ControllerName);
    }

    // Negative test for when account already exists during registration
    [Fact]
    public async Task Register_WhenAccountAlreadyExists_ReturnsViewWithDuplicateError()
    {
        // Arrange
        var model = new RegisterViewModel { Email = "bob@johansen.no", Password = "Pass123!", Role = Role.STUDENT };
        var duplicateError = new IdentityErrorDescriber().DuplicateUserName(model.Email);
        _userManager.CreateAsync(Arg.Any<IdentityUser>(), model.Password)
                    .Returns(IdentityResult.Failed(duplicateError));

        // Act
        var result = await _controller.Register(model);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, view.Model);   // the user's input is preserved
        Assert.Contains(_controller.ModelState[string.Empty]!.Errors,
                        e => e.ErrorMessage == duplicateError.Description);
        await _userManager.DidNotReceive().AddToRoleAsync(Arg.Any<IdentityUser>(), Arg.Any<string>());
        await _signInManager.DidNotReceive().SignInAsync(Arg.Any<IdentityUser>(), Arg.Any<bool>());
}
}