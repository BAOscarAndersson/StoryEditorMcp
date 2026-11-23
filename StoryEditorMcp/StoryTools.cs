namespace StoryEditorMcp;

public class StoryTools(string path)
{
    readonly string chapters = $@"{path}\Chapters";
    readonly string rejectedEdits = $@"{path}\RejectedEdits";
    readonly string words = $@"{path}\Words.md";

    public async Task<StringOrError> ReadChapter(int chapter)
    {
        try
        {
            string filePath = Path.Combine(chapters, $"Chapter {chapter}.md");

            return await File.ReadAllTextAsync(filePath);
        }
        catch (Exception ex)
        {
            return new Error(ex, "Chapter does not exist.");
        }
    }

    public async Task<StringOrError> ReadRejectedEditsChapter(int chapter)
    {
        try
        {
            string filePath = Path.Combine(rejectedEdits, $"Chapter {chapter}.md");

            return await File.ReadAllTextAsync(filePath);
        }
        catch (Exception ex)
        {
            return new Error(ex, "No edits for the chapter found.");
        }
    }

    public async Task<StringOrError> ReadWords()
    {
        try
        {
            return await File.ReadAllTextAsync(words);
        }
        catch (Exception ex)
        {
            return new Error(ex, "No words found.");
        }
    }
}
