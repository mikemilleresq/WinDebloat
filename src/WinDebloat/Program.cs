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
        exclusionsOption.IsRequired = false;
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

        await UninstallByNameIfNotExcluded(exclusions, "Teams Machine-Wide Installer");
        await UninstallByNameIfNotExcluded(exclusions, "Movies & TV");
        await UninstallByNameIfNotExcluded(exclusions, "Xbox TCUI");
        await UninstallByNameIfNotExcluded(exclusions, "Xbox Console Companion");
        await UninstallByNameIfNotExcluded(exclusions, "Xbox Game Bar Plugin");
        await UninstallByNameIfNotExcluded(exclusions, "Xbox Identity Provider");
        await UninstallByNameIfNotExcluded(exclusions, "Xbox Game Speech Window");
        await UninstallByNameIfNotExcluded(exclusions, "Xbox Game Bar");
        await UninstallByNameIfNotExcluded(exclusions, "Xbox");
        await UninstallByNameIfNotExcluded(exclusions, "Microsoft Tips");
        await UninstallByNameIfNotExcluded(exclusions, "MSN Weather");
        await UninstallByNameIfNotExcluded(exclusions, "Windows Media Player");
        await UninstallByNameIfNotExcluded(exclusions, "Mail and Calendar");
        await UninstallByNameIfNotExcluded(exclusions, "Microsoft Whiteboard");
        await UninstallByNameIfNotExcluded(exclusions, "Microsoft Pay");
        await UninstallByNameIfNotExcluded(exclusions, "Skype");
        await UninstallByNameIfNotExcluded(exclusions, "Windows Maps");
        await UninstallByNameIfNotExcluded(exclusions, "Feedback Hub");
        await UninstallByNameIfNotExcluded(exclusions, "Microsoft Photos");
        await UninstallByNameIfNotExcluded(exclusions, "Windows Camera");
        await UninstallByNameIfNotExcluded(exclusions, "Microsoft To Do");
        await UninstallByNameIfNotExcluded(exclusions, "Microsoft People");
        await UninstallByNameIfNotExcluded(exclusions, "Solitaire & Casual Games");
        await UninstallByNameIfNotExcluded(exclusions, "Mixed Reality Portal");
        await UninstallByNameIfNotExcluded(exclusions, "Microsoft Sticky Notes");
        await UninstallByNameIfNotExcluded(exclusions, "News");
        await UninstallByNameIfNotExcluded(exclusions, "Get Help");
        await UninstallByNameIfNotExcluded(exclusions, "Paint 3D");
        await UninstallByNameIfNotExcluded(exclusions, "Paint");
        await UninstallByNameIfNotExcluded(exclusions, "Cortana");
        await UninstallByNameIfNotExcluded(exclusions, "Clipchamp");
        await UninstallByNameIfNotExcluded(exclusions, "Power Automate");
        await UninstallByNameIfNotExcluded(exclusions, "OneNote for Windows 10");

        await WinGet.InstallById("dotPDNLLC.paintdotnet");

    }

    static Task UninstallByNameIfNotExcluded(string[]? exclusions, string packageName) {
        if (exclusions != null && exclusions.Contains(packageName)) return new Task(() => Console.WriteLine($"Package {packageName} skipped due to exclusions"));
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