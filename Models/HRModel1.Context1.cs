﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ChugenTocc2.ChugenTocc.ChugenTocc.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class HRSQLEntities : DbContext
    {
        public HRSQLEntities()
            : base("name=HRSQLEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<EmployeeAcount> EmployeeAcount { get; set; }
        public virtual DbSet<HRAdmin> HRAdmin { get; set; }
        public virtual DbSet<HRDepartment> HRDepartment { get; set; }
        public virtual DbSet<Record> Record { get; set; }
        public virtual DbSet<RH2> RH2 { get; set; }
        public virtual DbSet<EmpHours> EmpHours { get; set; }
        public virtual DbSet<EmployeeHistory> EmployeeHistory { get; set; }
        public virtual DbSet<EmployeeHR> EmployeeHR { get; set; }
    }
}