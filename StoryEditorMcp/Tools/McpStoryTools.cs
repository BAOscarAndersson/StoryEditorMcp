using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using StoryEditorMcp;
using System.ComponentModel;

internal class McpStoryTools
{
    [McpServerTool, Description("" +
        "Suggests an edit to a chapter. An edit should not just be a word to edit." +
        "If the edit couldn't be located, try using more of the surrounding text." +
        "The edit must be unique in the text." +
        "If you want to edit a word, edit the sentence in which the word is contained." +
        "The suggested edit should be the complete replacement for the original." +
        "Example, original followed by edit then reason:" +
        "This is an exxample" +
        "This is an example" +
        "One too many x")]
    public async Task<CallToolResult> SuggestAnEditForStory(
        StoryTools storyTools,
        [Description("Chapter number")] int chapter,
        [Description("The text which might need to be edited.")] string original,
        [Description("This is the suggestion for an edit.")] string edit,
        [Description("This is the reason why the edited is needed.")] string reason)
    {
        SuggestedEdit s = new(chapter, original, edit, reason);

        StringOrError t = await storyTools.SuggestEdit(s);

        return t.ToCallToolResult();
    }

    [McpServerTool, Description("Reads a chapter of the story")]
    public async Task<CallToolResult> ReadChapterOfTheStory(
        StoryTools storyTools,
        [Description("Chapter number of chapte to be read.")] int chapter = 0)
    {
        StringOrError t = await storyTools.ReadChapter(chapter);

        return t.ToCallToolResult();
    }

    [McpServerTool, Description("All non-English words in the story.")]
    public async Task<CallToolResult> NonEnglishWords(
        StoryTools storyTools)
    {
        StringOrError t = await storyTools.ReadWords();

        return t.ToCallToolResult();
    }
}
