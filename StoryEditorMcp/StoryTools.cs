using System.Text.RegularExpressions;

namespace StoryEditorMcp;

public partial class StoryTools(string path)
{
    readonly string chapters = $@"{path}\Chapters";
    readonly string rejectedEdits = $@"{path}\RejectedEdits";
    readonly string words = $@"{path}\Words.md";

    public async Task<StringOrError> ApplyApprovedSuggestions(int chapter)
    {
        try
        {
            StringOrError t = await ApplyAllApprovedEdits(chapter);

            return t;
        }
        catch (Exception ex)
        {
            return new Error("Could not apply approved suggestions.", ex);
        }
    }

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

    public async Task<StringOrError> ReadRejectedEditsForChapter(int chapter)
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

    public async Task<StringOrError> SuggestEdit(SuggestedEdit suggestedEdit)
    {
        if (ContainsAnyHtmlCrap(suggestedEdit.Edit, suggestedEdit.Reason, suggestedEdit.Original))
            return new Error("Edits should not contain html.");

        try
        {
            string path = PathToChapter(suggestedEdit.Chapter);

            string t = await File.ReadAllTextAsync(path);

            IntOrError i = FindSingleOccurrence(t, suggestedEdit.Original);

            return i switch
            {
                int index => await TryApplySuggestedEdit(t, index, suggestedEdit),
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
        string chapter,
        int index,
        SuggestedEdit suggestedEdit)
    {

        string before = chapter.Substring(0, index);
        string after = chapter.Substring(index);

        string edit = suggestedEdit.AsHtmlComment();

        string editedChapter = before + edit + after;

        string chapterPath = PathToChapter(suggestedEdit.Chapter);

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

    string PathToChapter(int chapter)
    {
        return Path.Combine(chapters, $"Chapter {chapter}.md");
    }

    string PathToRejected(int chapter)
    {
        return Path.Combine(rejectedEdits, $"Chapter {chapter}.md");
    }

    async Task<StringOrError> ApplyAllApprovedEdits(int chapter)
    {
        string path = PathToChapter(chapter);

        (SuggestedEdit, string)? se = null;

        do
        {
            string chapterText = await File.ReadAllTextAsync(path);

            se = ExtractFirstSuggestion(chapterText, chapter);

            if (se is (SuggestedEdit suggestedEdit, string modifiedChapter))
            {
                if (suggestedEdit.Rejected())
                    await MoveToRejected(suggestedEdit, modifiedChapter);
                else
                {
                    IntOrError i = FindSingleOccurrence(modifiedChapter, suggestedEdit.Original);

                    if (i is Error error)
                        return error;
                    else if (i is int index)
                        await ApplyEdit(index, suggestedEdit, modifiedChapter);
                }
            }
        }
        while (se is not null);

        return "Edit applied.";
    }

    async Task MoveToRejected(SuggestedEdit suggestedEdit, string editedChapter)
    {
        string rejectedPath = PathToRejected(suggestedEdit.Chapter);

        File.AppendAllText(rejectedPath, suggestedEdit.AsHtmlComment() + Environment.NewLine);

        string chapterPath = PathToChapter(suggestedEdit.Chapter);

        await File.WriteAllTextAsync(chapterPath, editedChapter);
    }

    async Task ApplyEdit(int index, SuggestedEdit suggestedEdit, string chapter)
    {
        string? edit = suggestedEdit.Edit;

        string before = chapter.Substring(0, index);
        string after = chapter.Substring(index + suggestedEdit.Original.Length);

        string editedChapter = CollapseNewlines(before + edit + after);

        string chapterPath = PathToChapter(suggestedEdit.Chapter);

        await File.WriteAllTextAsync(chapterPath, editedChapter);
    }

    static string CollapseNewlines(string input)
    {
        return ThreeOrMoreNewlines.Replace(input, match =>
        {
            if (match.Value.StartsWith("\r\n")) 
                return "\r\n\r\n";
            else 
                return "\n\n";
        });
    }

    static (SuggestedEdit suggestedEdit, string modifiedChapter)?
        ExtractFirstSuggestion(string chapterText, int chapter)
    {
        if (string.IsNullOrWhiteSpace(chapterText))
            return null;

        Regex? regex = SuggestedEditRegex.HtmlCommentRegex;
        MatchCollection matches = regex.Matches(chapterText);

        Match? match = matches.FirstOrDefault();

        if (match is null)
            return null;

        string? t = match.Value;

        if (t is null)
            return null;

        SuggestedEdit? se = SuggestedEdit.FromHtmlComment(chapter, t);

        if (se is null)
            return null;

        if (!se.ApprovedOrRejected())
            return null;

        int i = chapterText.IndexOf(t);

        if (i == -1)
            return null;

        string modifiedChapter = chapterText.Remove(i, t.Length);

        return (se, modifiedChapter);
    }

    [GeneratedRegex(@"(?:\r\n|\n){3,}")]
    static partial Regex ThreeOrMoreNewlines { get; }
}
