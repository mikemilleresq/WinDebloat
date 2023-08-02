using System.CommandLine;
static class Program
{
    static async Task Main(string[] args)
    {
        Logging.Init();

        try
        {
            var rootCommand = GetRootCommandAndSetupCommandLineHandling();
            await rootCommand.InvokeAsync(args);
        }
        catch (Exception exception)
        {
            Log.Fatal(exception, "Failed at startup ");
            throw;
        }
    }

    private static RootCommand GetRootCommandAndSetupCommandLineHandling()
    {
        var rootCommand = new System.CommandLine.RootCommand("App to debloat base window installs");
        var exclusionsOption = new System.CommandLine.Option<string[]>(name: "--exclusions", description: "Packages to exclude from the standard list");
        exclusionsOption.AddAlias("e");
        exclusionsOption.AllowMultipleArgumentsPerToken = true;
        exclusionsOption.IsRequired = true;
        rootCommand.AddOption(exclusionsOption);
        rootCommand.SetHandler(async (excluded) =>
        {
            await Inner(excluded!);
        },
        exclusionsOption);
        Log.Information("args passed in " + string.Join("", args));
        return rootCommand;
    }

    static async Task Inner(string[]? exclusions)
    {
        //https://winget.run
        //https://github.com/valinet/ExplorerPatcher

        RemoveChat();
        DisableStartupBoost();
        RemoveTaskBarSearch();
        RemoveWidgets();
        RemoveTaskView();
        EnableDeveloperMode();
        DisableWebSearch();
        MakePowerShelUnrestricted();

        await RemoveIfNotExcluded(exclusions, "Teams Machine-Wide Installer");
        await RemoveIfNotExcluded(exclusions, "Movies & TV");
        await RemoveIfNotExcluded(exclusions, "Xbox TCUI");
        await RemoveIfNotExcluded(exclusions, "Xbox Console Companion");
        await RemoveIfNotExcluded(exclusions, "Xbox Game Bar Plugin");
        await RemoveIfNotExcluded(exclusions, "Xbox Identity Provider");
        await RemoveIfNotExcluded(exclusions, "Xbox Game Speech Window");
        await RemoveIfNotExcluded(exclusions, "Xbox Game Bar");
        await RemoveIfNotExcluded(exclusions, "Xbox");
        await RemoveIfNotExcluded(exclusions, "Microsoft Tips");
        await RemoveIfNotExcluded(exclusions, "MSN Weather");
        await RemoveIfNotExcluded(exclusions, "Windows Media Player");
        await RemoveIfNotExcluded(exclusions, "Mail and Calendar");
        await RemoveIfNotExcluded(exclusions, "Microsoft Whiteboard");
        await RemoveIfNotExcluded(exclusions, "Microsoft Pay");
        await RemoveIfNotExcluded(exclusions, "Skype");
        await RemoveIfNotExcluded(exclusions, "Windows Maps");
        await RemoveIfNotExcluded(exclusions, "Feedback Hub");
        await RemoveIfNotExcluded(exclusions, "Microsoft Photos");
        await RemoveIfNotExcluded(exclusions, "Windows Camera");
        await RemoveIfNotExcluded(exclusions, "Microsoft To Do");
        await RemoveIfNotExcluded(exclusions, "Microsoft People");
        await RemoveIfNotExcluded(exclusions, "Solitaire & Casual Games");
        await RemoveIfNotExcluded(exclusions, "Mixed Reality Portal");
        await RemoveIfNotExcluded(exclusions, "Microsoft Sticky Notes");
        await RemoveIfNotExcluded(exclusions, "News");
        await RemoveIfNotExcluded(exclusions, "Get Help");
        await RemoveIfNotExcluded(exclusions, "Paint 3D");
        await RemoveIfNotExcluded(exclusions, "Paint");
        await RemoveIfNotExcluded(exclusions, "Cortana");
        await RemoveIfNotExcluded(exclusions, "Clipchamp");
        await RemoveIfNotExcluded(exclusions, "Power Automate");
        await RemoveIfNotExcluded(exclusions, "OneNote for Windows 10");

        await WinGet.InstallById("dotPDNLLC.paintdotnet");

    }

    static Task RemoveIfNotExcluded(string[]? exclusions, string packageName) {
        if (exclusions == null || exclusions.Contains(packageName)) return new Task(() => Console.WriteLine($"Package {packageName} skipped due to exclusions"));
        return WinGet.UninstallByName(packageName);
    }

    static void RemoveChat() =>
        Registry.SetValue(
            @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
            "TaskbarMn",
            0);

    static void RemoveWidgets() =>
        Registry.SetValue(
            @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
            "TaskbarDa",
            0);

    static void DisableStartupBoost() =>
        Registry.SetValue(
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge", "StartupBoostEnabled",
            1,
            RegistryValueKind.DWord);

    static void RemoveTaskView() =>
        Registry.SetValue(
            @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
            "ShowTaskViewButton",
            0);

    static void RemoveTaskBarSearch() =>
        Registry.SetValue(
            @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Search",
            "SearchboxTaskbarMode",
            0);

    //https://learn.microsoft.com/en-us/windows/apps/get-started/enable-your-device-for-development
    static void EnableDeveloperMode() =>
        Registry.SetValue(
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Appx",
            "AllowDevelopmentWithoutDevLicense",
            1,
            RegistryValueKind.DWord);

    static void MakePowerShelUnrestricted() =>
        Registry.SetValue(
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\PowerShell\1\ShellIds\Microsoft.PowerShell",
            "ExecutionPolicy",
            "Unrestricted",
            RegistryValueKind.String);

    static void DisableWebSearch() =>
        Registry.SetValue(
            @"HKEY_CURRENT_USER\SOFTWARE\Policies\Microsoft\Windows\Explorer",
            "DisableSearchBoxSuggestions",
            1,
            RegistryValueKind.DWord);
}