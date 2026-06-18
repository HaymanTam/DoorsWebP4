namespace DoorsWeb.Client.Components
{
    /// <summary>
    /// A lightweight row for <see cref="EntityListModal"/>: just an identifier and a display
    /// name. <see cref="Id"/> is a string so it works for any key shape (Guid, int, or a
    /// composite like "accessLevel/site") — the caller's delegates decide how to interpret it.
    /// </summary>
    public class EntityListRow
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
    }
}
