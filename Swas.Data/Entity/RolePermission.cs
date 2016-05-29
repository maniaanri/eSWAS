﻿namespace Swas.Data.Entity
{
    using System;
    using System.Collections.Generic;

    public class RolePermission
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int PermissionId{ get; set; }

        public virtual Role Role { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
