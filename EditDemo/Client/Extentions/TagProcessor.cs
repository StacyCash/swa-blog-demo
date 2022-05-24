namespace Client.Extentions;

public static class TagProcessor
{
    public static string[] CleanTags(this string[] tags){
        return tags.ToList().Select(tag => tag.Trim()).Where(tag => !string.IsNullOrWhiteSpace(tag)).ToArray();
    }
}
