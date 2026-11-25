using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using StoryEditorMcp;
using System.ComponentModel;

internal class McpApplyEdit
{
    [McpServerTool, Description("Applies approved suggestions of a chapter.")]
    public async Task<CallToolResult> ApplyEditsToChapter(
    StoryTools storyTools,
    [Description("Chapter number")] int chapter)
    {
        StringOrError t = await storyTools.ApplyApprovedSuggestions(chapter);

        return t.ToCallToolResult();
    }
}
