using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using StoryEditorMcp;
using System.ComponentModel;

internal class McpStoryTools
{
    [McpServerTool, Description("Suggests an edit to a chapter. An edit should not just be a word to edit. If you want to edit a word, edit the sentence in which the word is contained.")]
    public async Task<CallToolResult> SuggestAnEditForStory(
    StoryTools chapterReader,
        [Description("Chapter number")] int chapter,
        [Description("The text which might need to be edited.")] string original,
        [Description("This is the suggestion for an edit.")] string edit,
        [Description("This is the reason why the edited is needed.")] string reason)
    {
        SuggestedEdit s = new(chapter, original, edit, reason);

        StringOrError t = await chapterReader.SuggestEdit(s);

        return t.ToCallToolResult();
    }

    [McpServerTool, Description("Reads a chapter of the story")]
    public async Task<CallToolResult> ReadChapterOfTheStory(
        StoryTools chapterReader,
        [Description("Chapter number of chapte to be read.")] int chapter = 0)
    {
        StringOrError t = await chapterReader.ReadChapter(chapter);

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
