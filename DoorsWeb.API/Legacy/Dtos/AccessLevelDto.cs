namespace DoorsWeb.API.Legacy.Dtos
{
    // Plain DTO for the legacy "access levels by site" read.
    // Maps directly onto the SQL projection that the old system used
    // (sp_AccessLevelbySite_Read in LegacyDoorsServer\mdlDatabase.bas):
    //
    //   SELECT T_AccessLevel_Header.AccessLevel AS Code,
    //          T_AccessLevel_Header.LocalLevel  AS LocalCode,
    //          T_AccessLevel_Header.Name        AS Name,
    //          T_Sites.Name                     AS SiteName,
    //          T_Sites.Site                     AS SiteCode
    //   FROM T_AccessLevel_Header
    //   INNER JOIN T_Sites ON T_AccessLevel_Header.Site = T_Sites.Site
    //   ORDER BY T_Sites.Name, T_AccessLevel_Header.Name
    //
    // One instance == one row. Scaffolded entities: TAccessLevelHeader, TSites.

    /// <summary>One access level joined to its owning site.</summary>
    public class AccessLevelDto
    {
        /// <summary>Global access-level code. Source: TAccessLevelHeader.AccessLevel.</summary>
        public int Code { get; set; }

        /// <summary>Access-level name. Source: TAccessLevelHeader.Name.</summary>
        public string? Name { get; set; }

        /// <summary>
        /// Per-site local code. Source: TAccessLevelHeader.LocalLevel.
        /// A value of 0 denotes the "All Doors" level.
        /// </summary>
        public int? LocalCode { get; set; }

        /// <summary>Owning site code. Source: TSites.Site.</summary>
        public int SiteCode { get; set; }

        /// <summary>Owning site name. Source: TSites.Name.</summary>
        public string? SiteName { get; set; }
    }
}
