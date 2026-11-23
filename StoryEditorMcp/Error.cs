namespace StoryEditorMcp;

public record Error(string Reason, Exception? Ex = null);
