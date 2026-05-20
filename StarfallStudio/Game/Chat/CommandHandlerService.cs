using StarfallStudio.Services;
using StarfallStudio.UI;
using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using System;

namespace StarfallStudio.Game.Chat;

public class CommandHandlerService : IDisposable
{
    private const string StarfallStudioCommandName = "/brio";
    private const string XATCommandName = "/xat";
    private const string MCDFCommandName = "/mcdf";

    private readonly ICommandManager _commandManager;
    private readonly IChatGui _chatGui;
    private readonly UIManager _uiManager;
    private readonly Mediator _mediator;

    public CommandHandlerService(ICommandManager commandManager, IChatGui chatGui, UIManager uiManager, Mediator mediator)
    {
        _commandManager = commandManager;
        _chatGui = chatGui;
        _uiManager = uiManager;
        _mediator = mediator;

        _commandManager.AddHandler(StarfallStudioCommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Toggles the StarfallStudio window.",
            ShowInHelp = true,
        });
        _commandManager.AddHandler(XATCommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Toggles the StarfallStudio window.",
            ShowInHelp = false,
        });
        _commandManager.AddHandler(MCDFCommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Toggles StarfallStudio's MCDF window.",
            ShowInHelp = false,
        });
    }

    private void OnCommand(string command, string arguments)
    {
        if(command == MCDFCommandName)
        {
            _uiManager.ToggleMCDFWindow();
            return;
        }

        if(arguments.Length == 0)
            arguments = "window";

        var argumentList = arguments.Split(' ', 2);

        switch(argumentList[0].ToLowerInvariant())
        {
            case "window":
                _uiManager.ToggleMainWindow();
                break;

            case "settings":
                _uiManager.ToggleSettingsWindow();
                break;

            case "about":
                _uiManager.ToggleWelcomeWindow();
                break;

            case "mcdf":
                _uiManager.ToggleMCDFWindow();
                break;

            case "mediator":
                _mediator.PrintSubscriberInfo();
                break;

            case "help":
            default:
                PrintHelp();
                break;
        }

    }

    private void PrintHelp()
    {
        _chatGui.Print("Valid StarfallStudio Commands Are:");
        _chatGui.Print("<none> - Toggle main StarfallStudio window");
        _chatGui.Print("window - Toggle main StarfallStudio window");
        _chatGui.Print("settings - Toggle StarfallStudio settings window");
        _chatGui.Print("about - Toggle StarfallStudio info window");
        _chatGui.Print("help - Print this help prompt");
    }

    public void Dispose()
    {
        _commandManager.RemoveHandler(StarfallStudioCommandName);
        _commandManager.RemoveHandler(XATCommandName);
        _commandManager.RemoveHandler(MCDFCommandName);
    }
}
