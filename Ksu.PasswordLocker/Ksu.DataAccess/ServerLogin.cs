//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ksu.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class ServerLogin
    {
        public int ServerLoginId { get; set; }
        public int ServerId { get; set; }
        public int DepartmentId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
    
        public virtual Department Department { get; set; }
        public virtual Server Server { get; set; }
    }
}
