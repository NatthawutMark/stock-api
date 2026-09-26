using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace stock_api.Models;

public partial class DbContexts : DbContext
{
    public DbContexts()
    {
    }

    public DbContexts(DbContextOptions<DbContexts> options)
        : base(options)
    {
    }

    public virtual DbSet<DocBookingDetail> DocBookingDetails { get; set; }

    public virtual DbSet<DocBookingLog> DocBookingLogs { get; set; }

    public virtual DbSet<DocDisposalDetail> DocDisposalDetails { get; set; }

    public virtual DbSet<DocIssueDetail> DocIssueDetails { get; set; }

    public virtual DbSet<DocIssuePickingLog> DocIssuePickingLogs { get; set; }

    public virtual DbSet<DocReceiveDetail> DocReceiveDetails { get; set; }

    public virtual DbSet<DocReceivePutawayLog> DocReceivePutawayLogs { get; set; }

    public virtual DbSet<DocReturnDetail> DocReturnDetails { get; set; }

    public virtual DbSet<DocTran> DocTrans { get; set; }

    public virtual DbSet<DocTransLog> DocTransLogs { get; set; }

    public virtual DbSet<DocTransferFromDetail> DocTransferFromDetails { get; set; }

    public virtual DbSet<DocTransferToDetail> DocTransferToDetails { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<InventoryDamage> InventoryDamages { get; set; }

    public virtual DbSet<InventoryDamageLog> InventoryDamageLogs { get; set; }

    public virtual DbSet<InventoryMoveLog> InventoryMoveLogs { get; set; }

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

    public virtual DbSet<Refreshtoken> Refreshtokens { get; set; } = null!;

    public virtual DbSet<SysMenu> SysMenus { get; set; }

    public virtual DbSet<SysMenuUser> SysMenuUsers { get; set; }

    public virtual DbSet<SysRole> SysRoles { get; set; }

    public virtual DbSet<SysRoleUser> SysRoleUsers { get; set; }

    public virtual DbSet<UserAuthen> UserAuthens { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Server=localhost;port=5432;Database=Stock_Management;User Id=postgres;Password=P@ssP@STGRESQLw0rd;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DocBookingDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("doc_booking_detail_pkey");

            entity.ToTable("doc_booking_detail");

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
            entity.Property(e => e.DocId)
                .HasColumnType("character varying")
                .HasColumnName("doc_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.ItemId)
                .HasColumnType("character varying")
                .HasColumnName("item_id");
            entity.Property(e => e.ItemName)
                .HasColumnType("character varying")
                .HasColumnName("item_name");
            entity.Property(e => e.ItemQty)
                .HasPrecision(10, 2)
                .HasColumnName("item_qty");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");

            entity.HasOne(d => d.Doc).WithMany(p => p.DocBookingDetails)
                .HasForeignKey(d => d.DocId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("booking_doc_id_fk");

            entity.HasOne(d => d.Item).WithMany(p => p.DocBookingDetails)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("booking_item_id_fk");
        });

        modelBuilder.Entity<DocBookingLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("doc_booking_log_pkey");

            entity.ToTable("doc_booking_log");

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.BookingQty)
                .HasPrecision(10, 2)
                .HasColumnName("booking_qty");
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
            entity.Property(e => e.IssueDocId)
                .HasColumnType("character varying")
                .HasColumnName("issue_doc_id");
            entity.Property(e => e.ItemDetailId)
                .HasColumnType("character varying")
                .HasColumnName("item_detail_id");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");

            entity.HasOne(d => d.IssueDoc).WithMany(p => p.DocBookingLogs)
                .HasForeignKey(d => d.IssueDocId)
                .HasConstraintName("bookinglog_issue_doc_id_fk");

            entity.HasOne(d => d.ItemDetail).WithMany(p => p.DocBookingLogs)
                .HasForeignKey(d => d.ItemDetailId)
                .HasConstraintName("bookinglog_item_detail_id_fk");
        });

        modelBuilder.Entity<DocDisposalDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("doc_disposal_detail_pkey");

            entity.ToTable("doc_disposal_detail");

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
            entity.Property(e => e.DocId)
                .HasColumnType("character varying")
                .HasColumnName("doc_id");
            entity.Property(e => e.InvDmgId)
                .HasColumnType("character varying")
                .HasColumnName("inv_dmg_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.ItemQty)
                .HasPrecision(10, 2)
                .HasColumnName("item_qty");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");

            entity.HasOne(d => d.Doc).WithMany(p => p.DocDisposalDetails)
                .HasForeignKey(d => d.DocId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("doc_disp_doc_id");

            entity.HasOne(d => d.InvDmg).WithMany(p => p.DocDisposalDetails)
                .HasForeignKey(d => d.InvDmgId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("doc_disp_inv_dmg_id");
        });

        modelBuilder.Entity<DocIssueDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("doc_issue_detail_pkey");

            entity.ToTable("doc_issue_detail");

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
            entity.Property(e => e.DocId)
                .HasColumnType("character varying")
                .HasColumnName("doc_id");
            entity.Property(e => e.InvId)
                .HasColumnType("character varying")
                .HasColumnName("inv_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.ItemQty)
                .HasPrecision(10, 2)
                .HasColumnName("item_qty");
            entity.Property(e => e.PickingQty)
                .HasPrecision(10, 2)
                .HasColumnName("picking_qty");
            entity.Property(e => e.ReturnQty)
                .HasPrecision(10, 2)
                .HasColumnName("return_qty");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");

            entity.HasOne(d => d.Doc).WithMany(p => p.DocIssueDetails)
                .HasForeignKey(d => d.DocId)
                .HasConstraintName("iss_doc_id_fk");

            entity.HasOne(d => d.Inv).WithMany(p => p.DocIssueDetails)
                .HasForeignKey(d => d.InvId)
                .HasConstraintName("iss_inv_id_fk");
        });

        modelBuilder.Entity<DocIssuePickingLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("doc_issue_picking_log_pkey");

            entity.ToTable("doc_issue_picking_log");

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
            entity.Property(e => e.ItemDetailId)
                .HasColumnType("character varying")
                .HasColumnName("item_detail_id");
            entity.Property(e => e.PickingQty)
                .HasPrecision(10, 2)
                .HasColumnName("picking_qty");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");

            entity.HasOne(d => d.ItemDetail).WithMany(p => p.DocIssuePickingLogs)
                .HasForeignKey(d => d.ItemDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("docisspicklog_item_detail_id_fk");
        });

        modelBuilder.Entity<DocReceiveDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("doc_receive_detail_pkey");

            entity.ToTable("doc_receive_detail");

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
            entity.Property(e => e.DocId)
                .HasColumnType("character varying")
                .HasColumnName("doc_id");
            entity.Property(e => e.ExpireDate).HasColumnName("expire_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.ItemId)
                .HasColumnType("character varying")
                .HasColumnName("item_id");
            entity.Property(e => e.ItemName)
                .HasColumnType("character varying")
                .HasColumnName("item_name");
            entity.Property(e => e.ItemQty)
                .HasPrecision(10, 2)
                .HasColumnName("item_qty");
            entity.Property(e => e.LocationId)
                .HasColumnType("character varying")
                .HasColumnName("location_id");
            entity.Property(e => e.LotNo)
                .HasMaxLength(100)
                .HasColumnName("lot_no");
            entity.Property(e => e.MfgDate).HasColumnName("mfg_date");
            entity.Property(e => e.PutawayQty)
                .HasPrecision(10, 2)
                .HasColumnName("putaway_qty");
            entity.Property(e => e.SerialNo)
                .HasMaxLength(100)
                .HasColumnName("serial_no");
            entity.Property(e => e.UomId)
                .HasColumnType("character varying")
                .HasColumnName("uom_id");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");

            entity.HasOne(d => d.Doc).WithMany(p => p.DocReceiveDetails)
                .HasForeignKey(d => d.DocId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("rec_doc_id_fk");

            entity.HasOne(d => d.Item).WithMany(p => p.DocReceiveDetails)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("rec_item_id_fk");

            entity.HasOne(d => d.Location).WithMany(p => p.DocReceiveDetails)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("rec_location_id_fk");

            entity.HasOne(d => d.Uom).WithMany(p => p.DocReceiveDetails)
                .HasForeignKey(d => d.UomId)
                .HasConstraintName("rec_uom_id_fk");
        });

        modelBuilder.Entity<DocReceivePutawayLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("doc_receive_putaway_log_pkey");

            entity.ToTable("doc_receive_putaway_log");

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
            entity.Property(e => e.ItemDetailId)
                .HasColumnType("character varying")
                .HasColumnName("item_detail_id");
            entity.Property(e => e.PutawayQty)
                .HasPrecision(10, 2)
                .HasColumnName("putaway_qty");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");

            entity.HasOne(d => d.ItemDetail).WithMany(p => p.DocReceivePutawayLogs)
                .HasForeignKey(d => d.ItemDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("docrecputlog_item_detail_id_fk");
        });

        modelBuilder.Entity<DocReturnDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("doc_return_detail_pkey");

            entity.ToTable("doc_return_detail");

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
            entity.Property(e => e.DocId)
                .HasColumnType("character varying")
                .HasColumnName("doc_id");
            entity.Property(e => e.InvId)
                .HasColumnType("character varying")
                .HasColumnName("inv_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.ItemQty)
                .HasPrecision(10, 2)
                .HasColumnName("item_qty");
            entity.Property(e => e.ReasonId)
                .HasColumnType("character varying")
                .HasColumnName("reason_id");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");

            entity.HasOne(d => d.Doc).WithMany(p => p.DocReturnDetails)
                .HasForeignKey(d => d.DocId)
                .HasConstraintName("doc_return_doc_id_fk");

            entity.HasOne(d => d.Inv).WithMany(p => p.DocReturnDetails)
                .HasForeignKey(d => d.InvId)
                .HasConstraintName("doc_return_inv_id_fk");

            entity.HasOne(d => d.Reason).WithMany(p => p.DocReturnDetails)
                .HasForeignKey(d => d.ReasonId)
                .HasConstraintName("doc_return_reason_id_fk");
        });

        modelBuilder.Entity<DocTran>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("doc_trans_pkey");

            entity.ToTable("doc_trans");

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.BookingId)
                .HasColumnType("character varying")
                .HasColumnName("booking_id");
            entity.Property(e => e.CreateBy)
                .HasColumnType("character varying")
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.CustId)
                .HasColumnType("character varying")
                .HasColumnName("cust_id");
            entity.Property(e => e.DocDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("doc_date");
            entity.Property(e => e.DocIssueReturnId)
                .HasColumnType("character varying")
                .HasColumnName("doc_issue_return_id");
            entity.Property(e => e.DocNo)
                .HasColumnType("character varying")
                .HasColumnName("doc_no");
            entity.Property(e => e.DocTypeId)
                .HasColumnType("character varying")
                .HasColumnName("doc_type_id");
            entity.Property(e => e.EmpId)
                .HasColumnType("character varying")
                .HasColumnName("emp_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.RefDocNo)
                .HasColumnType("character varying")
                .HasColumnName("ref_doc_no");
            entity.Property(e => e.Remark).HasColumnName("remark");
            entity.Property(e => e.StatusFromId)
                .HasColumnType("character varying")
                .HasColumnName("status_from_id");
            entity.Property(e => e.StatusId)
                .HasColumnType("character varying")
                .HasColumnName("status_id");
            entity.Property(e => e.StatusToId)
                .HasColumnType("character varying")
                .HasColumnName("status_to_id");
            entity.Property(e => e.TransTypeId)
                .HasColumnType("character varying")
                .HasColumnName("trans_type_id");
            entity.Property(e => e.TransferFromWarehouseId)
                .HasColumnType("character varying")
                .HasColumnName("transfer_from_warehouse_id");
            entity.Property(e => e.TransferToWarehouseId)
                .HasColumnType("character varying")
                .HasColumnName("transfer_to_warehouse_id");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");
            entity.Property(e => e.VendId)
                .HasColumnType("character varying")
                .HasColumnName("vend_id");
            entity.Property(e => e.WarehouseId)
                .HasColumnType("character varying")
                .HasColumnName("warehouse_id");

            entity.HasOne(d => d.Cust).WithMany(p => p.DocTrans)
                .HasForeignKey(d => d.CustId)
                .HasConstraintName("mast_cust_id_fk");

            entity.HasOne(d => d.DocType).WithMany(p => p.DocTrans)
                .HasForeignKey(d => d.DocTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mast_doc_type_id_fk");

            entity.HasOne(d => d.Emp).WithMany(p => p.DocTrans)
                .HasForeignKey(d => d.EmpId)
                .HasConstraintName("mast_emp_id_fk");

            entity.HasOne(d => d.Status).WithMany(p => p.DocTrans)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mast_staus_id_fk");

            entity.HasOne(d => d.TransType).WithMany(p => p.DocTrans)
                .HasForeignKey(d => d.TransTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mast_tran_type_id_fk");

            entity.HasOne(d => d.Vend).WithMany(p => p.DocTrans)
                .HasForeignKey(d => d.VendId)
                .HasConstraintName("mast_vend_id_fk");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.DocTrans)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("mast_warehouse_id_fk");
        });

        modelBuilder.Entity<DocTransLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("req_trans_log_pkey");

            entity.ToTable("doc_trans_log");

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
            entity.Property(e => e.DocId)
                .HasColumnType("character varying")
                .HasColumnName("doc_id");
            entity.Property(e => e.FromStatusId)
                .HasColumnType("character varying")
                .HasColumnName("from_status_id");
            entity.Property(e => e.FromUserId)
                .HasColumnType("character varying")
                .HasColumnName("from_user_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.Remark).HasColumnName("remark");
            entity.Property(e => e.ToStatusId)
                .HasColumnType("character varying")
                .HasColumnName("to_status_id");
            entity.Property(e => e.ToUserId)
                .HasColumnType("character varying")
                .HasColumnName("to_user_id");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");

            entity.HasOne(d => d.Doc).WithMany(p => p.DocTransLogs)
                .HasForeignKey(d => d.DocId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("doc_id_fk");
        });

        modelBuilder.Entity<DocTransferFromDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("doc_transfer_from_detail_pkey");

            entity.ToTable("doc_transfer_from_detail");

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
            entity.Property(e => e.DocId)
                .HasColumnType("character varying")
                .HasColumnName("doc_id");
            entity.Property(e => e.InvId)
                .HasColumnType("character varying")
                .HasColumnName("inv_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.ItemQty)
                .HasPrecision(10, 2)
                .HasColumnName("item_qty");
            entity.Property(e => e.PickingQty)
                .HasPrecision(10, 2)
                .HasColumnName("picking_qty");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");

            entity.HasOne(d => d.Doc).WithMany(p => p.DocTransferFromDetails)
                .HasForeignKey(d => d.DocId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("doc_transfer_from_doc_id");

            entity.HasOne(d => d.Inv).WithMany(p => p.DocTransferFromDetails)
                .HasForeignKey(d => d.InvId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("doc_transfer_from_inv_id");
        });

        modelBuilder.Entity<DocTransferToDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("doc_transfer_to_detail_pkey");

            entity.ToTable("doc_transfer_to_detail");

            entity.HasIndex(e => e.NewInvId, "doc_transfer_to_detail_new_inv_id_key").IsUnique();

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
            entity.Property(e => e.DocId)
                .HasColumnType("character varying")
                .HasColumnName("doc_id");
            entity.Property(e => e.InvId)
                .HasColumnType("character varying")
                .HasColumnName("inv_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.ItemQty)
                .HasPrecision(10, 2)
                .HasColumnName("item_qty");
            entity.Property(e => e.LocationId)
                .HasColumnType("character varying")
                .HasColumnName("location_id");
            entity.Property(e => e.NewInvId)
                .HasColumnType("character varying")
                .HasColumnName("new_inv_id");
            entity.Property(e => e.PutawayQty)
                .HasPrecision(10, 2)
                .HasColumnName("putaway_qty");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");

            entity.HasOne(d => d.Doc).WithMany(p => p.DocTransferToDetails)
                .HasForeignKey(d => d.DocId)
                .HasConstraintName("doc_transfer_to_doc_id");

            entity.HasOne(d => d.Inv).WithMany(p => p.DocTransferToDetails)
                .HasForeignKey(d => d.InvId)
                .HasConstraintName("doc_transfer_to_inv_id");

            entity.HasOne(d => d.Location).WithMany(p => p.DocTransferToDetails)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("doc_transfer_to_loc_id");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("inventory_pkey");

            entity.ToTable("inventory");

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
            entity.Property(e => e.ExpireDate).HasColumnName("expire_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.ItemId)
                .HasColumnType("character varying")
                .HasColumnName("item_id");
            entity.Property(e => e.ItemPrice)
                .HasPrecision(10, 2)
                .HasColumnName("item_price");
            entity.Property(e => e.ItemQty)
                .HasPrecision(10, 2)
                .HasColumnName("item_qty");
            entity.Property(e => e.LocationId)
                .HasColumnType("character varying")
                .HasColumnName("location_id");
            entity.Property(e => e.LotNo)
                .HasMaxLength(100)
                .HasColumnName("lot_no");
            entity.Property(e => e.MfgDate).HasColumnName("mfg_date");
            entity.Property(e => e.SerialNo)
                .HasMaxLength(100)
                .HasColumnName("serial_no");
            entity.Property(e => e.UomId)
                .HasColumnType("character varying")
                .HasColumnName("uom_id");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");
            entity.Property(e => e.WarehouseId)
                .HasColumnType("character varying")
                .HasColumnName("warehouse_id");

            entity.HasOne(d => d.Item).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inv_item_id_fk");

            entity.HasOne(d => d.Location).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("inv_location_id_fk");

            entity.HasOne(d => d.Uom).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.UomId)
                .HasConstraintName("inv_uom_id_fk");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("inv_warehouse_id_fk");
        });

        modelBuilder.Entity<InventoryDamage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("inventory_damage_pkey");

            entity.ToTable("inventory_damage");

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
            entity.Property(e => e.DocId)
                .HasColumnType("character varying")
                .HasColumnName("doc_id");
            entity.Property(e => e.InvId)
                .HasColumnType("character varying")
                .HasColumnName("inv_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.ItemQty)
                .HasPrecision(10, 2)
                .HasColumnName("item_qty");
            entity.Property(e => e.LocationId)
                .HasColumnType("character varying")
                .HasColumnName("location_id");
            entity.Property(e => e.ReasonId)
                .HasColumnType("character varying")
                .HasColumnName("reason_id");
            entity.Property(e => e.UomId)
                .HasColumnType("character varying")
                .HasColumnName("uom_id");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");
            entity.Property(e => e.WarehouseId)
                .HasColumnType("character varying")
                .HasColumnName("warehouse_id");

            entity.HasOne(d => d.Doc).WithMany(p => p.InventoryDamages)
                .HasForeignKey(d => d.DocId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inv_dmg_doc_id_fk");

            entity.HasOne(d => d.Inv).WithMany(p => p.InventoryDamages)
                .HasForeignKey(d => d.InvId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inv_dmg_inv_id_fk");

            entity.HasOne(d => d.Location).WithMany(p => p.InventoryDamages)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("inv_dmg_loc_id_fk");

            entity.HasOne(d => d.Reason).WithMany(p => p.InventoryDamages)
                .HasForeignKey(d => d.ReasonId)
                .HasConstraintName("inv_dmg_reason_id_fk");

            entity.HasOne(d => d.Uom).WithMany(p => p.InventoryDamages)
                .HasForeignKey(d => d.UomId)
                .HasConstraintName("inv_dmg_uom_id_fk");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.InventoryDamages)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("inv_dmg_wh_id_fk");
        });

        modelBuilder.Entity<InventoryDamageLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("inventory_damage_log_pkey");

            entity.ToTable("inventory_damage_log");

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
            entity.Property(e => e.InvDmgId)
                .HasColumnType("character varying")
                .HasColumnName("inv_dmg_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.ItemQty)
                .HasPrecision(10, 2)
                .HasColumnName("item_qty");
            entity.Property(e => e.MoveLocationId)
                .HasColumnType("character varying")
                .HasColumnName("move_location_id");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");

            entity.HasOne(d => d.InvDmg).WithMany(p => p.InventoryDamageLogs)
                .HasForeignKey(d => d.InvDmgId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("invdmglog_inv_dmg_id_fk");

            entity.HasOne(d => d.MoveLocation).WithMany(p => p.InventoryDamageLogs)
                .HasForeignKey(d => d.MoveLocationId)
                .HasConstraintName("invdmglog_loc_id_fk");
        });

        modelBuilder.Entity<InventoryMoveLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("inventory_move_log_pkey");

            entity.ToTable("inventory_move_log");

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
            entity.Property(e => e.InvId)
                .HasColumnType("character varying")
                .HasColumnName("inv_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.ItemQty)
                .HasPrecision(10, 2)
                .HasColumnName("item_qty");
            entity.Property(e => e.NewLocationId)
                .HasColumnType("character varying")
                .HasColumnName("new_location_id");
            entity.Property(e => e.OldLocationId)
                .HasColumnType("character varying")
                .HasColumnName("old_location_id");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");

            entity.HasOne(d => d.Inv).WithMany(p => p.InventoryMoveLogs)
                .HasForeignKey(d => d.InvId)
                .HasConstraintName("invmovelog_inv_id_fk");

            entity.HasOne(d => d.NewLocation).WithMany(p => p.InventoryMoveLogNewLocations)
                .HasForeignKey(d => d.NewLocationId)
                .HasConstraintName("invmovelog_newloc_id_fk");

            entity.HasOne(d => d.OldLocation).WithMany(p => p.InventoryMoveLogOldLocations)
                .HasForeignKey(d => d.OldLocationId)
                .HasConstraintName("invmovelog_oldloc_id_fk");
        });

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

        modelBuilder.Entity<Refreshtoken>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("refreshtoken");

            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.ExpiryDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expiry_date");
            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.IsRevoked).HasColumnName("is_revoked");
            entity.Property(e => e.ReplacedToken)
                .HasColumnType("character varying")
                .HasColumnName("replaced_token");
            entity.Property(e => e.RevokedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("revoked_date");
            entity.Property(e => e.Token)
                .HasColumnType("character varying")
                .HasColumnName("token");
            entity.Property(e => e.UserId)
                .HasColumnType("character varying")
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("token_user_id_fk");
        });

        modelBuilder.Entity<SysMenu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sys_menu_pkey");

            entity.ToTable("sys_menu");

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
            entity.Property(e => e.ParentId)
                .HasColumnType("character varying")
                .HasColumnName("parent_id");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("sys_menu_parent_id_fk");
        });

        modelBuilder.Entity<SysMenuUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sys_menu_user_id_pk");

            entity.ToTable("sys_menu_user");

            entity.HasIndex(e => e.MenuId, "sys_menu_user_menuid_unique").IsUnique();

            entity.HasIndex(e => e.UserId, "sys_menu_user_userid_unique").IsUnique();

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
            entity.Property(e => e.MenuId)
                .HasColumnType("character varying")
                .HasColumnName("menu_id");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");
            entity.Property(e => e.UserId)
                .HasColumnType("character varying")
                .HasColumnName("user_id");

            entity.HasOne(d => d.Menu).WithOne(p => p.SysMenuUser)
                .HasForeignKey<SysMenuUser>(d => d.MenuId)
                .HasConstraintName("sys_menu_user_menuid_fk");

            entity.HasOne(d => d.User).WithOne(p => p.SysMenuUser)
                .HasForeignKey<SysMenuUser>(d => d.UserId)
                .HasConstraintName("sys_menu_user_userid_fk");
        });

        modelBuilder.Entity<SysRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sys_role_pkey");

            entity.ToTable("sys_role");

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
            entity.Property(e => e.IsAdmin)
                .HasDefaultValue(false)
                .HasColumnName("is_admin");
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
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");
        });

        modelBuilder.Entity<SysRoleUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sys_role_menu_id_pk");

            entity.ToTable("sys_role_user");

            entity.HasIndex(e => e.RoleId, "sys_role_user_unique").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.CanApprove)
                .HasDefaultValue(false)
                .HasColumnName("can_approve");
            entity.Property(e => e.CanCreate)
                .HasDefaultValue(false)
                .HasColumnName("can_create");
            entity.Property(e => e.CanView)
                .HasDefaultValue(false)
                .HasColumnName("can_view");
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
            entity.Property(e => e.RoleId)
                .HasColumnType("character varying")
                .HasColumnName("role_id");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");
            entity.Property(e => e.UserId)
                .HasColumnType("character varying")
                .HasColumnName("user_id");

            entity.HasOne(d => d.Role).WithOne(p => p.SysRoleUser)
                .HasForeignKey<SysRoleUser>(d => d.RoleId)
                .HasConstraintName("sys_role_user_sys_role_fk");

            entity.HasOne(d => d.User).WithMany(p => p.SysRoleUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("sys_role_menu_user_id_fk");
        });

        modelBuilder.Entity<UserAuthen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_authen_pkey");

            entity.ToTable("user_authen");

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
            entity.Property(e => e.Password)
                .HasColumnType("character varying")
                .HasColumnName("password");
            entity.Property(e => e.Plaintext)
                .HasColumnType("character varying")
                .HasColumnName("plaintext");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");
            entity.Property(e => e.UserId)
                .HasColumnType("character varying")
                .HasColumnName("user_id");
            entity.Property(e => e.Username)
                .HasColumnType("character varying")
                .HasColumnName("username");

            entity.HasOne(d => d.User).WithMany(p => p.UserAuthens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_auth_user_id_fk");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_profile_pkey");

            entity.ToTable("user_profile");

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.CreateBy)
                .HasColumnType("character varying")
                .HasColumnName("create_by");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.FName)
                .HasColumnType("character varying")
                .HasColumnName("f_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.LName)
                .HasColumnType("character varying")
                .HasColumnName("l_name");
            entity.Property(e => e.Tel)
                .HasColumnType("character varying")
                .HasColumnName("tel");
            entity.Property(e => e.UpdateBy)
                .HasColumnType("character varying")
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_date");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
