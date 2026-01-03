using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrontDesk.Shared;
using FrontDesk.Shared.Dto;
using FrontDesk.Shared.Enums;
using FrontDesk.Shared.Validation;
using FrontDesk.Ui.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FrontDesk.Ui.ViewModels;

public partial class MainWindowViewModel : ObservableValidator
{
    private readonly IApiClient _apiClient;
    private const string DEFAULT_MESSAGE = "Select a user type to proceed.";

#pragma warning disable
    /// <summary>
    /// So the XAML designer doesnt break.
    /// </summary>
    public MainWindowViewModel() { }
#pragma warning restore

    public MainWindowViewModel(IApiClient apiClient)
    {
        _apiClient = apiClient;

        UserTypeCommand = new RelayCommand<string>(OnUserTypeSelected);
        ActionTypeCommand = new RelayCommand<string>(OnActionTypeSelected);
        SubmitCommand = new AsyncRelayCommand(SubmitFormAsync);
        ResetCommand = new RelayCommand(Reset);
    }

    public RelayCommand<string> UserTypeCommand { get; }
    public RelayCommand<string> ActionTypeCommand { get; }
    public AsyncRelayCommand SubmitCommand { get; }
    public RelayCommand ResetCommand { get; }

    // UserType selection
    [ObservableProperty]
    private bool _isUserTypeSelected;
    [ObservableProperty]
    private UserType _userType;
    [ObservableProperty]
    private bool _isStaffVisible = true;
    [ObservableProperty]
    private bool _isVisitorVisible = true;

    // Entry/Exit selection (Visitor only)
    [ObservableProperty]
    private bool _isVisitActionSelected;
    [ObservableProperty]
    private VisitAction _visitAction;
    [ObservableProperty]
    private bool _isEntryVisible;
    [ObservableProperty]
    private bool _isExitVisible;

    // Request
    [ObservableProperty]
    private bool _isPinVisible;
    [ObservableProperty]
    [Required]
    private string? _pin;
    [ObservableProperty]
    private bool _isVisitorInputVisible;
    [ObservableProperty]
    [Required]
    private string? _name;
    [ObservableProperty]
    private string? _company;
    [ObservableProperty]
    private string? _phoneNumber;
    [ObservableProperty]
    [Required]
    private string? _reason;
    [ObservableProperty]
    private bool _isSubmitVisible;
    [ObservableProperty]
    private bool _isSubmitEnabled = true;
    [ObservableProperty]
    private bool _isAwaitingResponse;

    // Info
    [ObservableProperty]
    private bool _isInfoVisible = true;
    [ObservableProperty]
    private string _infoMessage = DEFAULT_MESSAGE;

    // Response
    [ObservableProperty]
    private bool _isResponseVisible;
    [ObservableProperty]
    private string _responseMessage = string.Empty;

    private void OnUserTypeSelected(string? userType)
    {
        // Prevent reselection.
        if (!IsStaffVisible || !IsVisitorVisible)
            return;

        if (string.Equals(userType, "staff"))
        {
            UserType = UserType.Staff;
            IsVisitorVisible = false;
            IsInfoVisible = false;
            IsPinVisible = true;
            IsSubmitVisible = true;
        }
        else
        {
            UserType = UserType.Visitor;
            IsStaffVisible = false;
            IsEntryVisible = true;
            IsExitVisible = true;
            InfoMessage = "Select an action to proceed.";
        }
    }

    private void OnActionTypeSelected(string? entryType)
    {
        // Prevent reselection.
        if (!IsEntryVisible || !IsExitVisible)
            return;

        if (string.Equals(entryType, "entry"))
        {
            VisitAction = VisitAction.Entry;
            IsExitVisible = false;
            IsVisitorInputVisible = true;
        }
        else
        {
            VisitAction = VisitAction.Exit;
            IsEntryVisible = false;
            IsPinVisible = true;
        }

        IsInfoVisible = false;
        IsSubmitVisible = true;
    }

    private async Task SubmitFormAsync()
    {
        // Clear any previous response.
        IsResponseVisible = false;

        if (!IsValidInput())
            return;

        var request = new VisitRequest()
        {
            UserType = UserType,
            VisitAction = VisitAction,
            Pin = Pin ?? "000000",
            Name = Name ?? string.Empty,
            Company = Company,
            PhoneNumber = PhoneNumber,
            Reason = Reason ?? string.Empty,
        };

        try
        {
            IsAwaitingResponse = true;
            var response = await _apiClient.SendVisitRequestAsync(request);
            // A slight delay to show UI animation regardless of response speed.
            await Task.Delay(2000);
            IsAwaitingResponse = false;

            await ShowResponse(response);
        }
        catch (Exception ex)
        {
            IsAwaitingResponse = false;
            IsInfoVisible = true;
            InfoMessage = $"Error: {ex.Message}";
        }
    }

    private bool IsValidInput()
    {
        // Hide any previous infomessage
        IsInfoVisible = false;

        // Staff - require pin
        // Visitors - require pin on exit
        if (UserType == UserType.Staff ||
            (UserType == UserType.Visitor && VisitAction == VisitAction.Exit))
        {
            if (!PinValidator.IsValid(Pin))
            {
                InfoMessage = Const.VALID_PIN_REQUIRED;
                IsInfoVisible = true;
                return false;
            }
        }

        // Visitors - require name and reason on entry
        if (UserType == UserType.Visitor && VisitAction == VisitAction.Entry)
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Reason))
            {
                InfoMessage = "Name and Reason required";
                IsInfoVisible = true;
                return false;
            }
        }

        return true;
    }

    private async Task ShowResponse(VisitResponse response)
    {
        if (!response.IsAccepted)
        {
            ResponseMessage = "Invalid Pin";
            IsResponseVisible = true;
            return;
        }

        var status = response.VisitAction == VisitAction.Entry ? "Entry accepted" : "Exit accepted";
        ResponseMessage = $"{response.Username} {status}";
        if (response.VisitorPin is not null)
        {
            ResponseMessage += $" Pin required on Exit: {response.VisitorPin}";
        }

        IsResponseVisible = true;
        IsSubmitEnabled = false;
    }

    private void Reset()
    {
        // UserType selection
        IsStaffVisible = true;
        IsVisitorVisible = true;

        // Entry/Exit selection (Visitor only)
        IsEntryVisible = false;
        IsExitVisible = false;

        // Request
        IsPinVisible = false;
        Pin = null;
        IsVisitorInputVisible = false;
        Name = null;
        Company = string.Empty;
        PhoneNumber = string.Empty;
        Reason = null;
        IsSubmitVisible = false;
        IsSubmitEnabled = true;

        // Response
        IsResponseVisible = false;
        ResponseMessage = string.Empty;

        // Info
        IsInfoVisible = true;
        InfoMessage = DEFAULT_MESSAGE;
    }
}