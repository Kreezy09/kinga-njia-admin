using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NjianiAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddClaimUserEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Convert User.Role from string to integer
            // Map old string values to enum integers
            migrationBuilder.Sql(@"
                ALTER TABLE ""Users"" 
                ALTER COLUMN ""Role"" TYPE integer 
                USING (
                    CASE 
                        WHEN ""Role"" = 'EndUser' THEN 0
                        WHEN ""Role"" = 'Staff' THEN 1
                        WHEN ""Role"" = 'Admin' THEN 2
                        ELSE 0  -- Default to EndUser
                    END
                );
            ");

            // Convert Claim.Status from string to integer
            migrationBuilder.Sql(@"
                ALTER TABLE ""Claims"" 
                ALTER COLUMN ""Status"" TYPE integer 
                USING (
                    CASE 
                        WHEN ""Status"" = 'Pending' THEN 0
                        WHEN ""Status"" = 'InProgress' THEN 1
                        WHEN ""Status"" = 'Resolved' THEN 2
                        WHEN ""Status"" = 'Rejected' THEN 3
                        ELSE 0  -- Default to Pending
                    END
                );
            ");

            // Convert Claim.Severity from string to integer (nullable)
            migrationBuilder.Sql(@"
                ALTER TABLE ""Claims"" 
                ALTER COLUMN ""Severity"" TYPE integer 
                USING (
                    CASE 
                        WHEN ""Severity"" = 'Low' THEN 0
                        WHEN ""Severity"" = 'Medium' THEN 1
                        WHEN ""Severity"" = 'High' THEN 2
                        WHEN ""Severity"" = 'Critical' THEN 3
                        WHEN ""Severity"" IS NULL THEN NULL
                        ELSE NULL
                    END
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Convert User.Role from integer back to string
            migrationBuilder.Sql(@"
                ALTER TABLE ""Users"" 
                ALTER COLUMN ""Role"" TYPE text 
                USING (
                    CASE 
                        WHEN ""Role"" = 0 THEN 'EndUser'
                        WHEN ""Role"" = 1 THEN 'Staff'
                        WHEN ""Role"" = 2 THEN 'Admin'
                        ELSE 'EndUser'
                    END
                );
            ");

            // Convert Claim.Status from integer back to string
            migrationBuilder.Sql(@"
                ALTER TABLE ""Claims"" 
                ALTER COLUMN ""Status"" TYPE text 
                USING (
                    CASE 
                        WHEN ""Status"" = 0 THEN 'Pending'
                        WHEN ""Status"" = 1 THEN 'InProgress'
                        WHEN ""Status"" = 2 THEN 'Resolved'
                        WHEN ""Status"" = 3 THEN 'Rejected'
                        ELSE 'Pending'
                    END
                );
            ");

            // Convert Claim.Severity from integer back to string
            migrationBuilder.Sql(@"
                ALTER TABLE ""Claims"" 
                ALTER COLUMN ""Severity"" TYPE text 
                USING (
                    CASE 
                        WHEN ""Severity"" = 0 THEN 'Low'
                        WHEN ""Severity"" = 1 THEN 'Medium'
                        WHEN ""Severity"" = 2 THEN 'High'
                        WHEN ""Severity"" = 3 THEN 'Critical'
                        WHEN ""Severity"" IS NULL THEN NULL
                        ELSE NULL
                    END
                );
            ");
        }
    }
}