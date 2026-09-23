using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace back_stock.Models;

public partial class DbContexts : DbContext
{
    public DbContexts()
    {
    }

    public DbContexts(DbContextOptions<DbContexts> options)
        : base(options)
    {
    }

    public virtual DbSet<MastBrand> MastBrands { get; set; }

    public virtual DbSet<MastCustomer> MastCustomers { get; set; }

    public virtual DbSet<MastDocType> MastDocTypes { get; set; }

    public virtual DbSet<MastEmployee> MastEmployees { get; set; }

    public virtual DbSet<MastGroup> MastGroups { get; set; }

    public virtual DbSet<MastItem> MastItems { get; set; }

    public virtual DbSet<MastItemGroup> MastItemGroups { get; set; }

    public virtual DbSet<MastItemWarehouse> MastItemWarehouses { get; set; }

    public virtual DbSet<MastLocation> MastLocations { get; set; }

    public virtual DbSet<MastReason> MastReasons { get; set; }

    public virtual DbSet<MastStatus> MastStatuses { get; set; }

    public virtual DbSet<MastTransType> MastTransTypes { get; set; }

    public virtual DbSet<MastUom> MastUoms { get; set; }

    public virtual DbSet<MastVendor> MastVendors { get; set; }

    public virtual DbSet<MastWarehouse> MastWarehouses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // => optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MastBrand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mast_brand_pk");

            entity.ToTable("mast_brand");

            entity.HasIndex(e => e.NameTh, "mast_brand_unique").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.CreateBy)
                .HasColumnType("character varying")
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.NameEn)
                .HasColumnType("character varying")
                .HasColumnName("name_en");
            entity.Property(e => e.NameTh)
                .HasColumnType("character varying")
                .HasColumnName("name_th");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");
        });

        modelBuilder.Entity<MastCustomer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mast_customer_pkey");

            entity.ToTable("mast_customer");

            entity.HasIndex(e => e.CustCode, "mast_customer_cust_code_key").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.ContactName)
                .HasMaxLength(255)
                .HasColumnName("contact_name");
            entity.Property(e => e.CreateBy)
                .HasColumnType("character varying")
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.CustCode)
                .HasMaxLength(50)
                .HasColumnName("cust_code");
            entity.Property(e => e.CustName)
                .HasMaxLength(255)
                .HasColumnName("cust_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.Remark).HasColumnName("remark");
            entity.Property(e => e.Tel)
                .HasMaxLength(50)
                .HasColumnName("tel");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");
        });

        modelBuilder.Entity<MastDocType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mast_doc_type_pkey");

            entity.ToTable("mast_doc_type");

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.CreateBy)
                .HasColumnType("character varying")
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.TransTypeId)
                .HasColumnType("character varying")
                .HasColumnName("trans_type_id");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");

            entity.HasOne(d => d.TransType).WithMany(p => p.MastDocTypes)
                .HasForeignKey(d => d.TransTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mast_doc_type_fk");
        });

        modelBuilder.Entity<MastEmployee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mast_employee_pkey");

            entity.ToTable("mast_employee");

            entity.HasIndex(e => e.EmpCode, "mast_employee_emp_code_key").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.CreateBy)
                .HasColumnType("character varying")
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.EmpCode)
                .HasMaxLength(50)
                .HasColumnName("emp_code");
            entity.Property(e => e.FName)
                .HasMaxLength(255)
                .HasColumnName("f_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.LName)
                .HasMaxLength(255)
                .HasColumnName("l_name");
            entity.Property(e => e.Tel)
                .HasMaxLength(50)
                .HasColumnName("tel");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");
        });

        modelBuilder.Entity<MastGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mast_group_pk");

            entity.ToTable("mast_group");

            entity.HasIndex(e => e.NameTh, "mast_group_name_th_key").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.CreateBy)
                .HasColumnType("character varying")
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.NameEn)
                .HasColumnType("character varying")
                .HasColumnName("name_en");
            entity.Property(e => e.NameTh)
                .HasColumnType("character varying")
                .HasColumnName("name_th");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");
        });

        modelBuilder.Entity<MastItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mast_item_pkey");

            entity.ToTable("mast_item");

            entity.HasIndex(e => e.ItemCode, "mast_item_item_code_key").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.BrandId)
                .HasColumnType("character varying")
                .HasColumnName("brand_id");
            entity.Property(e => e.BuyPrice)
                .HasPrecision(18, 2)
                .HasColumnName("buy_price");
            entity.Property(e => e.CreateBy)
                .HasColumnType("character varying")
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.IsLotno)
                .HasDefaultValue(false)
                .HasColumnName("is_lotno");
            entity.Property(e => e.IsSerialno)
                .HasDefaultValue(false)
                .HasColumnName("is_serialno");
            entity.Property(e => e.ItemCode)
                .HasColumnType("character varying")
                .HasColumnName("item_code");
            entity.Property(e => e.ItemName)
                .HasColumnType("character varying")
                .HasColumnName("item_name");
            entity.Property(e => e.LocationId)
                .HasColumnType("character varying")
                .HasColumnName("location_id");
            entity.Property(e => e.MaxAlter).HasColumnName("max_alter");
            entity.Property(e => e.MinAlter).HasColumnName("min_alter");
            entity.Property(e => e.SellPrice)
                .HasPrecision(18, 2)
                .HasColumnName("sell_price");
            entity.Property(e => e.UomId)
                .HasColumnType("character varying")
                .HasColumnName("uom_id");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");

            entity.HasOne(d => d.Brand).WithMany(p => p.MastItems)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("mast_item_brand_fk");

            entity.HasOne(d => d.Location).WithMany(p => p.MastItems)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mast_item_location_fk");

            entity.HasOne(d => d.Uom).WithMany(p => p.MastItems)
                .HasForeignKey(d => d.UomId)
                .HasConstraintName("mast_item_mast_uom_fk");
        });

        modelBuilder.Entity<MastItemGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mast_item_group_pkey");

            entity.ToTable("mast_item_group");

            entity.HasIndex(e => new { e.ItemId, e.GroupId }, "mast_item_group_item_id_group_id_key").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.CreateBy)
                .HasColumnType("character varying")
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.GroupId)
                .HasColumnType("character varying")
                .HasColumnName("group_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.ItemId)
                .HasColumnType("character varying")
                .HasColumnName("item_id");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");

            entity.HasOne(d => d.Group).WithMany(p => p.MastItemGroups)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("mast_item_group_group_fk");

            entity.HasOne(d => d.Item).WithMany(p => p.MastItemGroups)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("mast_item_group_item_fk");
        });

        modelBuilder.Entity<MastItemWarehouse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mast_item_warehouse_pkey");

            entity.ToTable("mast_item_warehouse");

            entity.HasIndex(e => new { e.ItemId, e.WarehouseId }, "mast_item_warehouse_item_id_warehouse_id_key").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.CreateBy)
                .HasColumnType("character varying")
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.ItemId)
                .HasColumnType("character varying")
                .HasColumnName("item_id");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");
            entity.Property(e => e.WarehouseId)
                .HasColumnType("character varying")
                .HasColumnName("warehouse_id");

            entity.HasOne(d => d.Item).WithMany(p => p.MastItemWarehouses)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("mast_item_warehouse_item_fk");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.MastItemWarehouses)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("mast_item_warehouse_fk");
        });

        modelBuilder.Entity<MastLocation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mast_location_pkey");

            entity.ToTable("mast_location");

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.CreateBy)
                .HasColumnType("character varying")
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");
        });

        modelBuilder.Entity<MastReason>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mast_reason_pkey");

            entity.ToTable("mast_reason");

            entity.HasIndex(e => e.Name, "mast_reason_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.CreateBy)
                .HasColumnType("character varying")
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");
        });

        modelBuilder.Entity<MastStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mast_status_pkey");

            entity.ToTable("mast_status");

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.Code)
                .HasColumnType("character varying")
                .HasColumnName("code");
            entity.Property(e => e.CreateBy)
                .HasColumnType("character varying")
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.NameEn)
                .HasMaxLength(255)
                .HasColumnName("name_en");
            entity.Property(e => e.NameTh)
                .HasMaxLength(255)
                .HasColumnName("name_th");
            entity.Property(e => e.OrderNo).HasColumnName("order_no");
            entity.Property(e => e.TransTypeId)
                .HasColumnType("character varying")
                .HasColumnName("trans_type_id");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");

            entity.HasOne(d => d.TransType).WithMany(p => p.MastStatuses)
                .HasForeignKey(d => d.TransTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mast_status_fk");
        });

        modelBuilder.Entity<MastTransType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mast_trans_type_pkey");

            entity.ToTable("mast_trans_type");

            entity.HasIndex(e => e.Name, "mast_trans_type_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.CreateBy)
                .HasColumnType("character varying")
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");
        });

        modelBuilder.Entity<MastUom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mast_uom_pkey");

            entity.ToTable("mast_uom");

            entity.HasIndex(e => e.Name, "mast_uom_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.CreateBy)
                .HasColumnType("character varying")
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");
        });

        modelBuilder.Entity<MastVendor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mast_vendor_pkey");

            entity.ToTable("mast_vendor");

            entity.HasIndex(e => e.VendCode, "mast_vendor_vend_code_key").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.ContactName)
                .HasMaxLength(255)
                .HasColumnName("contact_name");
            entity.Property(e => e.CreateBy)
                .HasColumnType("character varying")
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.Remark).HasColumnName("remark");
            entity.Property(e => e.Tel)
                .HasMaxLength(50)
                .HasColumnName("tel");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");
            entity.Property(e => e.VendCode)
                .HasMaxLength(50)
                .HasColumnName("vend_code");
            entity.Property(e => e.VendName)
                .HasMaxLength(255)
                .HasColumnName("vend_name");
        });

        modelBuilder.Entity<MastWarehouse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mast_warehouse_pkey");

            entity.ToTable("mast_warehouse");

            entity.HasIndex(e => e.Code, "mast_warehouse_code_key").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.CreateBy)
                .HasColumnType("character varying")
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");
            entity.Property(e => e.WarehouseName)
                .HasMaxLength(255)
                .HasColumnName("warehouse_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
