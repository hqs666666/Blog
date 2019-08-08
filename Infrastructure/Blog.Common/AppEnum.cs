using System.ComponentModel.DataAnnotations;

namespace Blog.Common
{

    #region System

    public enum DatabaseType
    {
        SqlServer,
        MySql,
    }

    public enum UserType
    {
        [Display(Name = "普通用户")]
        User = 0,

        [Display(Name = "管理人员")]
        Management = 1,
    }

    public enum GrantType
    {
        Password,
        RefreshToken,
        AuthorizationCode,
        Implicit,
        ClientCredentials
    }

    #endregion

}
