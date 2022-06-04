﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Extensions.Hosting;
using WPFUI.Demo.Views;
using WPFUI.Mvvm.Contracts;

namespace WPFUI.Demo.Services;

/// <summary>
/// Managed host of the application.
/// </summary>
public class ApplicationHostService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IPageService _pageService;
    private readonly IThemeService _themeService;
    private readonly ITaskbarService _taskbarService;
    private readonly INotifyIconService _notifyIconService;

    private INavigationWindow _navigationWindow;

    public ApplicationHostService(IServiceProvider serviceProvider,
        IPageService pageService, IThemeService themeService,
        ITaskbarService taskbarService, INotifyIconService notifyIconService)
    {
        // If you want, you can do something with these services at the beginning of loading the application.
        _serviceProvider = serviceProvider;
        _pageService = pageService;
        _themeService = themeService;
        _taskbarService = taskbarService;
        _notifyIconService = notifyIconService;
    }

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await HandleActivationAsync();
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    /// <summary>
    /// Creates main window during activation.
    /// </summary>
    private async Task HandleActivationAsync()
    {
        await Task.CompletedTask;

        if (!Application.Current.Windows.OfType<Container>().Any())
        {
            _navigationWindow = _serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow;
            _navigationWindow!.ShowWindow();

            // NOTICE: You can set this service directly in the window 
            // _navigationWindow.SetPageService(_pageService);

            // NOTICE: In the case of this window, we navigate to the Dashboard after loading with Container.InitializeUi()
            // _navigationWindow.Navigate(typeof(Views.Pages.Dashboard));

            await Task.CompletedTask;
        }

        var notifyIcon = _serviceProvider.GetService(typeof(INotifyIconService)) as INotifyIconService;

        if (!notifyIcon!.IsRegistered)
        {
            var parentMainWindow = new WindowInteropHelper(_navigationWindow as Window);

            notifyIcon.Register(parentMainWindow.Handle);
        }
    }
}