﻿// <auto-generated />
using AspNetCoreWithAppRolesAndFineGrained.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AspNetCoreWithAppRolesAndFineGrained.Migrations
{
    [DbContext(typeof(AspNetCoreWithAppRolesAndFineGrainedDbContext))]
    partial class AspNetCoreWithAppRolesAndFineGrainedDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.8");

            modelBuilder.Entity("AspNetCoreWithAppRolesAndFineGrained.Models.Branch", b =>
                {
                    b.Property<int>("BranchID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("RegionalManagerAADGroupId")
                        .HasColumnType("TEXT");

                    b.Property<string>("SalespersonAADGroupId")
                        .HasColumnType("TEXT");

                    b.HasKey("BranchID");

                    b.ToTable("Branch");
                });

            modelBuilder.Entity("AspNetCoreWithAppRolesAndFineGrained.Models.Employee", b =>
                {
                    b.Property<int>("EmployeeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BranchID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstName")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserPrincipalName")
                        .HasColumnType("TEXT");

                    b.HasKey("EmployeeID");

                    b.HasIndex("BranchID");

                    b.ToTable("Employee");
                });

            modelBuilder.Entity("AspNetCoreWithAppRolesAndFineGrained.Models.Salary", b =>
                {
                    b.Property<int>("SalaryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("EmployeeID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("SalaryID");

                    b.HasIndex("EmployeeID")
                        .IsUnique();

                    b.ToTable("Salary");
                });

            modelBuilder.Entity("AspNetCoreWithAppRolesAndFineGrained.Models.Sale", b =>
                {
                    b.Property<int>("SaleID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("EmployeeID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("SaleID");

                    b.HasIndex("EmployeeID");

                    b.ToTable("Sale");
                });

            modelBuilder.Entity("AspNetCoreWithAppRolesAndFineGrained.Models.Employee", b =>
                {
                    b.HasOne("AspNetCoreWithAppRolesAndFineGrained.Models.Branch", "Branch")
                        .WithMany("Employees")
                        .HasForeignKey("BranchID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Branch");
                });

            modelBuilder.Entity("AspNetCoreWithAppRolesAndFineGrained.Models.Salary", b =>
                {
                    b.HasOne("AspNetCoreWithAppRolesAndFineGrained.Models.Employee", "Employee")
                        .WithOne("Salary")
                        .HasForeignKey("AspNetCoreWithAppRolesAndFineGrained.Models.Salary", "EmployeeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("AspNetCoreWithAppRolesAndFineGrained.Models.Sale", b =>
                {
                    b.HasOne("AspNetCoreWithAppRolesAndFineGrained.Models.Employee", "Employee")
                        .WithMany("Sales")
                        .HasForeignKey("EmployeeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("AspNetCoreWithAppRolesAndFineGrained.Models.Branch", b =>
                {
                    b.Navigation("Employees");
                });

            modelBuilder.Entity("AspNetCoreWithAppRolesAndFineGrained.Models.Employee", b =>
                {
                    b.Navigation("Salary");

                    b.Navigation("Sales");
                });
#pragma warning restore 612, 618
        }
    }
}
