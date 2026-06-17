using TabBlazor;

namespace DoorsWeb.Client.Components
{
    /// <summary>
    /// One node in the User Properties → Permissions tree. The tree mirrors the
    /// main-navigation buttons: each node corresponds to a nav item, and <see cref="Checked"/>
    /// grants the user access to it.
    /// </summary>
    public class PermissionNode
    {
        public string Name { get; set; } = "";
        public IIconType IconType { get; set; } = DoorsWeb.Client.TablerIcons.Photo;
        public bool Checked { get; set; } = true;
        public bool Expanded { get; set; } = true;
        public List<PermissionNode> Children { get; set; } = new();
    }
}
