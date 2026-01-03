using Avalonia.Headless.XUnit;
using FrontDesk.Shared;
using FrontDesk.Shared.Enums;
using FrontDesk.Ui.ViewModels;
using FrontDesk.Ui.Views;
using Xunit;

namespace FrontDesk.UiTests;

public class MainViewTests
{
    [AvaloniaFact]
    public async Task ShowsError_WhenStaffPinMissing()
    {
        // Arrange
        var vm = new MainWindowViewModel(new MockApiClient());
        var view = new MainWindow
        {
            DataContext = vm
        };

        view.Show();

        // Act
        vm.UserType = UserType.Staff;
        vm.Pin = "";
        await vm.SubmitCommand.ExecuteAsync(null);

        // Assert
        Assert.True(vm.IsInfoVisible);
        Assert.Equal(Const.VALID_PIN_REQUIRED, vm.InfoMessage);
    }

    [AvaloniaFact]
    public async Task ShowsResponse_WhenVisitorEntry()
    {
        // Arrange
        var vm = new MainWindowViewModel(new MockApiClient());
        var view = new MainWindow
        {
            DataContext = vm
        };

        view.Show();

        // Act
        vm.UserType = UserType.Visitor;
        vm.VisitAction = VisitAction.Entry;
        vm.Name = "TestVisitor";
        vm.Reason = "Test reason";
        await vm.SubmitCommand.ExecuteAsync(null);

        // Assert
        Assert.Contains("TestVisitor", vm.ResponseMessage);
        Assert.Contains("Entry accepted", vm.ResponseMessage);
        Assert.Contains("123456", vm.ResponseMessage);
        Assert.True(vm.IsResponseVisible);
        Assert.False(vm.IsSubmitEnabled);
    }
}
