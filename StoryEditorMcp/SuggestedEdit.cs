namespace StoryEditorMcp;

public record SuggestedEdit(
    int Chapter,
    string Original,
    string Edit,
    string Reason,
    string? Accepted = "")
{
    public string AsHtmlComment()
    {
        return $"""

                <!-- 
                Orig: {Original}
                Edit: {Edit}
                Reason: {Reason}
                Accepted: {Accepted}
                -->

                """;
    }

    public static SuggestedEdit? FromHtmlComment(int chapter, string htmlComment)
    {
        try
        {
            string[] t = Lines(htmlComment);

            string origRow = t[1];
            string editRow = t[2];
            string reasonRow = t[3];
            string acceptedRow = t[4];

            var edit = new SuggestedEdit(
                chapter,
                V(origRow),
                V(editRow),
                V(reasonRow),
                V(acceptedRow));

            return edit;

            static string V(string s)
            {
                return s.Split(':')[1].TrimStart().TrimEnd();
            }
        }
        catch
        {
            return null;
        }
    }

    private static string[] Lines(string htmlComment)
    {
        string[] t = htmlComment.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

        if (t.Length == 8)
            return t;

        t = htmlComment.Split("\n", StringSplitOptions.RemoveEmptyEntries);

        if (t.Length == 8)
            return t;

        throw new Exception("new lines are messed up.");
    }
}