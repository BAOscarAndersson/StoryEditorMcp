using ModelContextProtocol.Protocol;
using StoryEditorMcp;

internal static class StringOrErrorExtensions
{
    extension(StringOrError stringOrError)
    {
        public CallToolResult ToCallToolResult()
        {
            return stringOrError switch
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
                    IsError = isError,
                };

                t.Content.Add(new TextContentBlock() { Text = s });

                return t;
            }
        }
    }
}