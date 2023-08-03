using System.CommandLine;
static class Program
{
    static string[] _inclusions = { 
        "Teams Machine-Wide Installer", 
        "Movies & TV",
        "Xbox TCUI",
        "Xbox Console Companion",
        "Xbox Game Bar Plugin",
        "Xbox Identity Provider",
        "Xbox Game Speech Window",
        "Xbox Game Bar",
        "Xbox",
        "Microsoft Tips",
        "MSN Weather",
        "Windows Media Player",
        "Mail and Calendar",
        "Microsoft Whiteboard",
        "Microsoft Pay",
        "Skype",
        "Windows Maps",
        "Feedback Hub",
        "Microsoft Photos",
        "Windows Camera",
        "Microsoft To Do",
        "Microsoft People",
        "Solitaire & Casual Games",
        "Mixed Reality Portal",
        "Microsoft Sticky Notes",
        "News",
        "Get Help",
        "Paint 3D",
        "Paint",
        "Cortana",
        "Clipchamp",
        "Power Automate",
        "OneNote for Windows 10"
    };

    static async Task Main(string[] args)
    {
        Logging.Init();
        try
        {
            var rootCommand = GetRootCommandAndSetupCommandLineHandling();
            Log.Information("args passed in " + string.Join("", args));
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

        var inclusionOption = new System.CommandLine.Option<string[]>(name: "--inclusions", description: "Packages to include in the standard list");
        inclusionOption.AddAlias("i");
        inclusionOption.AllowMultipleArgumentsPerToken = true;
        inclusionOption.IsRequired = false;

        rootCommand.AddOption(exclusionsOption);
        rootCommand.AddOption(inclusionOption);
        rootCommand.SetHandler(async (excluded, included) =>
        {
            await Inner(excluded!,included!);
        },
        exclusionsOption, inclusionOption);
        return rootCommand;
    }

    static async Task Inner(string[]? exclusions, string[]? inclusions)
    {
        //https://winget.run
        //https://github.com/valinet/ExplorerPatcher
        if (inclusions == null || inclusions.Length ==0) inclusions = _inclusions;
        RemoveChat();
        DisableStartupBoost();
        RemoveTaskBarSearch();
        RemoveWidgets();
        RemoveTaskView();
        EnableDeveloperMode();
        DisableWebSearch();
        MakePowerShelUnrestricted();

        foreach (var packageName in inclusions)   {
            if (exclusions == null |! exclusions!.Contains(packageName)) {
                await WinGet.UninstallByName(packageName);
            }
        }

        await WinGet.InstallById("dotPDNLLC.paintdotnet");

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