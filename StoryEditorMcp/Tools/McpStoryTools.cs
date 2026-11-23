using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using StoryEditorMcp;
using System.ComponentModel;

internal class McpStoryTools
{
    [McpServerTool, Description("Suggests an edit to a chapter.")]
    public async Task<CallToolResult> SuggestAnEditForStory(
    StoryTools chapterReader,
        [Description("Chapter number")] int chapter,
        [Description("The text which might need to be edited.")] string original,
        [Description("This is the suggestion for an edit.")] string edit,
        [Description("This is the reason why the edited is needed.")] string reason)
    {
        SuggestedEdit s = new(chapter, original, edit, reason);

        StringOrError t = await chapterReader.SuggestEdit(s);

        return ResultHandler(t);
    }

    [McpServerTool, Description("Reads a chapter of the story")]
    public async Task<CallToolResult> ReadChapterOfTheStory(
        StoryTools chapterReader,
        [Description("Chapter number")] int chapter = 0)
    {
        StringOrError t = await chapterReader.ReadChapter(chapter);

        return ResultHandler(t);
    }

    [McpServerTool, Description("All non-English words in the story.")]
    public async Task<CallToolResult> NonEnglishWords(
        StoryTools storyTools)
    {
        StringOrError t = await storyTools.ReadWords();

        return ResultHandler(t);
    }

    static CallToolResult ResultHandler(StringOrError t)
    {
        return t switch
        {
            string s => SuccessHandler(s),
            Error e => ErrorHandler(e)
        };

        CallToolResult SuccessHandler(string content)
        {
            return StringOrErrorToResult(content);
        }

        CallToolResult ErrorHandler(Error error)
        {
            return StringOrErrorToResult(error.Reason, true);
        }

        CallToolResult StringOrErrorToResult(string s, bool? isError = false)
        {
            CallToolResult t = new()
            {
                IsError = isError
            };
            TextContentBlock c = new() { Text = s };
            t.Content.Add(c);

            return t;
        }
    }
}
