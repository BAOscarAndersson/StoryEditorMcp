using Microsoft.AspNetCore.Mvc;
using ModelContextProtocol.Server;
using StoryEditorMcp;
using System.ComponentModel;

internal class McpStoryTools
{
    [McpServerTool, Description("Reads a chapter of the story")]
    public async Task<string> ReadChapterOfTheStory(
        StoryTools chapterReader,
        [Description("Chapter number")] int chapter = 0)
    {
        StringOrError t = await chapterReader.ReadChapter(chapter);

        return ResultHandler(t);
    }

    [McpServerTool, Description("All non-English words in the story.")]
    public async Task<string> NonEnglishWords(
        StoryTools storyTools)
    {
        StringOrError t = await storyTools.ReadWords();

        return ResultHandler(t);
    }

    static string ResultHandler(StringOrError t)
    {
        return t switch
        {
            string s => s,
            Error e => ErrorHandler(e)
        };

        string ErrorHandler(Error error)
        {
            return error.Reason;
        }
    }
}
