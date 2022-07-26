﻿using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elemental.Services
{
    public class SidebarService
    {
        public const int LARGE_SCREEN_SIZE = 1025;
        public const int MEDIUM_SCREEN_SIZE = 600;

        public const int SIDEBAR_WIDTH = 250;
        public const int SIDEBAR_COLLAPSE_WIDTH = 56;
        
        public static event Func<Task> OnResize;

        public static ILogger Logger = null;

        [JSInvokable]
        public static async Task OnBrowserResize()
        {
            try
            {
                await OnResize?.Invoke();
            }
            catch (Exception ex) {
                Logger?.LogWarning(ex, "Error resizing AElemental navigation bar");
            }
        }

        public static async Task<int> GetInnerHeight(IJSRuntime jsRuntime)
        {
            return await jsRuntime.InvokeAsync<int>("initializeSidebar.getInnerHeight");
        }

        public static async Task<int> GetInnerWidth(IJSRuntime jsRuntime)
        {
            return await jsRuntime.InvokeAsync<int>("initializeSidebar.getInnerWidth");
        }
    }

    public enum BrowserSize
    {
        Small = 0,
        Medium = 1,
        Large = 2
    }

}
