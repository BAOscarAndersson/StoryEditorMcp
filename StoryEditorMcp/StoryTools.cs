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
            string filePath = PathToChapter(chapter);

            return await File.ReadAllTextAsync(filePath);
        }
        catch (Exception ex)
        {
            return new Error("Chapter does not exist.", ex);
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
            return new Error("No edits for the chapter found.", ex);
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
            return new Error("No words found.", ex);
        }
    }

    string PathToChapter(int chapter)
    {
        return Path.Combine(chapters, $"Chapter {chapter}.md");
    }

   public async Task<StringOrError> SuggestEdit(
        int chapter,
        string textToEdit,
        string suggestedEdit,
        string reason)
    {
        if (ContainsAnyHtmlCrap(suggestedEdit, reason))
            return new Error("Edits should not contain html.");

        try
        {
            string path = PathToChapter(chapter);

            string t = await File.ReadAllTextAsync(path);

            var i = FindSingleOccurrence(t, textToEdit);

            return i switch
            {
                int index => await TryApplySuggestedEdit(chapter, t, index, textToEdit, suggestedEdit, reason),
                Error error => error
            };
        }
        catch (Exception ex)
        {
            return new Error("Could not add edit suggestion.", ex);
        }
    }

    static bool ContainsAnyHtmlCrap(params string[] strings)
    {
        foreach (string s in strings)
            if (ContainsAnyHtmlCrap(s))
                return true;

        return false;
    }

    static bool ContainsAnyHtmlCrap(string s)
    {
        const string htmlChars = "<>&";
        return s.Any(htmlChars.Contains);
    }

    static IntOrError FindSingleOccurrence(string chapter, string toEdit)
    {
        if (string.IsNullOrEmpty(toEdit)) 
            return new Error("The edit can not be empty.");

        int firstIndex = chapter.IndexOf(toEdit);
        if (firstIndex == -1)
            return new Error("Could not find the edit in the chapter.");

        int secondIndex = chapter.IndexOf(toEdit, firstIndex + 1);

        if (secondIndex == -1)
            return firstIndex;
        else
            return new Error("There are multiple occurrences of the edit. It must be unique.");
    }

    async Task<StringOrError> TryApplySuggestedEdit(
        int chapterNr,
        string chapter,
        int index,
        string textToEdit,
        string editSuggestion,
        string reason)
    {

        string before = chapter.Substring(0, index);
        string after = chapter.Substring(index);

        string edit = $"""

            <!-- 
            Orig: {textToEdit} 
            Edit: {editSuggestion} 
            Reason: {reason} 
            Accepted: 
            -->

            """;
            

        string editedChapter = before + edit + after;

        string chapterPath = PathToChapter(chapterNr);

        try
        {
            await File.WriteAllTextAsync(chapterPath, editedChapter);

            return "Suggested edit added.";
        }
        catch (Exception ex)
        {
            return new Error("Could not add the suggested edit due to file system error.", ex);
        }
    }
}
