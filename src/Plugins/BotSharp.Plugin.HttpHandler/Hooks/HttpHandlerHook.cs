using BotSharp.Abstraction.Agents;
using BotSharp.Abstraction.Agents.Settings;
using BotSharp.Abstraction.Functions.Models;
using BotSharp.Abstraction.Repositories;
using BotSharp.Plugin.HttpHandler.Enums;

namespace BotSharp.Plugin.HttpHandler.Hooks;

public class HttpHandlerHook : AgentHookBase
{
    private static string TOOL_ASSISTANT = Guid.Empty.ToString();

    public override string SelfId => string.Empty;

    public HttpHandlerHook(IServiceProvider services, AgentSettings settings)
        : base(services, settings)
    {
    }

    public override void OnAgentLoaded(Agent agent)
    {
        var conv = _services.GetRequiredService<IConversationService>();
        var isConvMode = conv.IsConversationMode();
        var isEnabled = !agent.Tools.IsNullOrEmpty() && agent.Tools.Contains(Tool.HttpHandler);

        if (isConvMode && isEnabled)
        {
            var (prompt, fn) = GetPromptAndFunction();
            if (fn != null)
            {
                if (!string.IsNullOrWhiteSpace(prompt))
                {
                    agent.Instruction += $"\r\n\r\n{prompt}\r\n\r\n";
                }

                if (agent.Functions == null)
                {
                    agent.Functions = new List<FunctionDef> { fn };
                }
                else
                {
                    agent.Functions.Add(fn);
                }
            }
        }

        base.OnAgentLoaded(agent);
    }

    private (string, FunctionDef?) GetPromptAndFunction()
    {
        var fn = "handle_http_request";
        var db = _services.GetRequiredService<IBotSharpRepository>();
        var agent = db.GetAgent(TOOL_ASSISTANT);
        var prompt = agent?.Templates?.FirstOrDefault(x => x.Name.IsEqualTo($"{fn}.fn"))?.Content ?? string.Empty;
        var loadAttachmentFn = agent?.Functions?.FirstOrDefault(x => x.Name.IsEqualTo(fn));
        return (prompt, loadAttachmentFn);
    }
}
