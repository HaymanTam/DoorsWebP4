namespace DoorsWeb.Shared.DTO
{
    /// <summary>Result of uploading a user photo to the API.</summary>
    public class PhotoUploadResult
    {
        /// <summary>Stored file name (server-generated, e.g. a GUID).</summary>
        public string FileName { get; set; } = "";

        /// <summary>Web path to the photo relative to the API host, e.g. /media/user-photo/{file}.</summary>
        public string Path { get; set; } = "";
    }
}
