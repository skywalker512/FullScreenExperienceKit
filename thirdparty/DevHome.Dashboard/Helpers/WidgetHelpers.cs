// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DevHome.Common.Extensions;
using DevHome.Common.Services;
using DevHome.Dashboard.ComSafeWidgetObjects;
using Microsoft.UI.Xaml;
using Microsoft.Windows.Widgets;
using Microsoft.Windows.Widgets.Hosts;
using Serilog;

namespace DevHome.Dashboard.Helpers;

internal sealed class WidgetHelpers
{
    public const string WebExperiencePackPackageId = "9MSSGKG348SP";
    public const string WebExperiencePackageFamilyName = "MicrosoftWindows.Client.WebExperience_cw5n1h2txyewy";
    public const string WidgetsPlatformRuntimePackageId = "9N3RK8ZV2ZR8";
    public const string WidgetsPlatformRuntimePackageFamilyName = "Microsoft.WidgetsPlatformRuntime_8wekyb3d8bbwe";

    public const string WidgetHostName = "FullScreenExperienceWidgets";

    public const double WidgetPxHeightSmall = 146;
    public const double WidgetPxHeightMedium = 304;
    public const double WidgetPxHeightLarge = 462;

    public const double WidgetPxWidth = 300;

    public static WidgetSize GetLargestCapabilitySize(WidgetCapability[] capabilities)
    {
        // Guaranteed to have at least one capability
        var largest = capabilities[0].Size;

        foreach (var cap in capabilities)
        {
            if (cap.Size > largest)
            {
                largest = cap.Size;
            }
        }

        return largest;
    }

    public static WidgetSize GetDefaultWidgetSize(WidgetCapability[] capabilities)
    {
        // The default size of the widget should be prioritized as Medium, Large, Small.
        // This matches the size preferences of the Windows Widget Dashboard.
        if (capabilities.Any(cap => cap.Size == WidgetSize.Medium))
        {
            return WidgetSize.Medium;
        }
        else if (capabilities.Any(cap => cap.Size == WidgetSize.Large))
        {
            return WidgetSize.Large;
        }
        else if (capabilities.Any(cap => cap.Size == WidgetSize.Small))
        {
            return WidgetSize.Small;
        }
        else
        {
            // Return something in case new sizes are added.
            return capabilities[0].Size;
        }
    }

    public static async Task<bool> IsIncludedWidgetProviderAsync(WidgetProviderDefinition provider)
    {
        // Allow everything.
        return true;
    }

    public static string CreateWidgetCustomState(int ordinal)
    {
        var state = new WidgetCustomState
        {
            Host = WidgetHostName,
            Position = ordinal,
        };

        return JsonSerializer.Serialize(state, SourceGenerationContext.Default.WidgetCustomState);
    }

    public static async Task SetPositionCustomStateAsync(ComSafeWidget widget, int ordinal)
    {
        var stateStr = await widget.GetCustomStateAsync();
        var state = JsonSerializer.Deserialize(stateStr, SourceGenerationContext.Default.WidgetCustomState);
        state.Position = ordinal;
        stateStr = JsonSerializer.Serialize(state, SourceGenerationContext.Default.WidgetCustomState);
        await widget.SetCustomStateAsync(stateStr);
    }
}
