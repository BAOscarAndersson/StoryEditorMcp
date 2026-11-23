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

            var origRow = t[2];
            var editRow = t[3];
            var reasonRow = t[4];
            var acceptedRow = t[5];

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
        string[] t = htmlComment.Split("\r\n");

        if (t.Length == 8)
            return t;

        t = htmlComment.Split("\n");

        if (t.Length == 8)
            return t;

        throw new Exception("new lines are messed up.");
    }
}