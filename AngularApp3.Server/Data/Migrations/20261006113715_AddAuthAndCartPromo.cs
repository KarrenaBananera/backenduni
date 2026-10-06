using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AngularApp3.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthAndCartPromo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cart_items_carts_cart_id",
                table: "cart_items");

            migrationBuilder.DropForeignKey(
                name: "FK_cart_items_products_product_id",
                table: "cart_items");

            migrationBuilder.DropForeignKey(
                name: "FK_carts_users_user_id",
                table: "carts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_products",
                table: "products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_carts",
                table: "carts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_promo_codes",
                table: "promo_codes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cart_items",
                table: "cart_items");

            migrationBuilder.RenameTable(
                name: "products",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "carts",
                newName: "Carts");

            migrationBuilder.RenameTable(
                name: "promo_codes",
                newName: "PromoCodes");

            migrationBuilder.RenameTable(
                name: "cart_items",
                newName: "CartItems");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "users",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Products",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "rating",
                table: "Products",
                newName: "Rating");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "Products",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "images",
                table: "Products",
                newName: "Images");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Products",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Products",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "stock_quantity",
                table: "Products",
                newName: "StockQuantity");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Products",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Carts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Carts",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Carts",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_carts_user_id",
                table: "Carts",
                newName: "IX_Carts_UserId");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "PromoCodes",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "PromoCodes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "PromoCodes",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "discount_percentage",
                table: "PromoCodes",
                newName: "DiscountPercentage");

            migrationBuilder.RenameIndex(
                name: "IX_promo_codes_code",
                table: "PromoCodes",
                newName: "IX_PromoCodes_Code");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "CartItems",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "CartItems",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "CartItems",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "cart_id",
                table: "CartItems",
                newName: "CartId");

            migrationBuilder.RenameIndex(
                name: "IX_cart_items_product_id",
                table: "CartItems",
                newName: "IX_CartItems_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_cart_items_cart_id_product_id",
                table: "CartItems",
                newName: "IX_CartItems_CartId_ProductId");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Products",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "AppliedPromoCode",
                table: "Carts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "PromoCodes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountPercentage",
                table: "PromoCodes",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "CartItems",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Carts",
                table: "Carts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PromoCodes",
                table: "PromoCodes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartItems",
                table: "CartItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Carts_CartId",
                table: "CartItems",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_users_UserId",
                table: "Carts",
                column: "UserId",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Carts_CartId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_users_UserId",
                table: "Carts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Carts",
                table: "Carts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PromoCodes",
                table: "PromoCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartItems",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "AppliedPromoCode",
                table: "Carts");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "products");

            migrationBuilder.RenameTable(
                name: "Carts",
                newName: "carts");

            migrationBuilder.RenameTable(
                name: "PromoCodes",
                newName: "promo_codes");

            migrationBuilder.RenameTable(
                name: "CartItems",
                newName: "cart_items");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "users",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "users",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "products",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "products",
                newName: "rating");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "products",
                newName: "price");

            migrationBuilder.RenameColumn(
                name: "Images",
                table: "products",
                newName: "images");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "products",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "products",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "StockQuantity",
                table: "products",
                newName: "stock_quantity");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "products",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "carts",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "carts",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "carts",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_Carts_UserId",
                table: "carts",
                newName: "IX_carts_user_id");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "promo_codes",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "promo_codes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "promo_codes",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "DiscountPercentage",
                table: "promo_codes",
                newName: "discount_percentage");

            migrationBuilder.RenameIndex(
                name: "IX_PromoCodes_Code",
                table: "promo_codes",
                newName: "IX_promo_codes_code");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "cart_items",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "cart_items",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "cart_items",
                newName: "product_id");

            migrationBuilder.RenameColumn(
                name: "CartId",
                table: "cart_items",
                newName: "cart_id");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_ProductId",
                table: "cart_items",
                newName: "IX_cart_items_product_id");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_CartId_ProductId",
                table: "cart_items",
                newName: "IX_cart_items_cart_id_product_id");

            migrationBuilder.AlterColumn<decimal>(
                name: "price",
                table: "products",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "products",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "promo_codes",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "discount_percentage",
                table: "promo_codes",
                type: "numeric(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<Guid>(
                name: "product_id",
                table: "cart_items",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddPrimaryKey(
                name: "PK_products",
                table: "products",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_carts",
                table: "carts",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_promo_codes",
                table: "promo_codes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_cart_items",
                table: "cart_items",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_cart_items_carts_cart_id",
                table: "cart_items",
                column: "cart_id",
                principalTable: "carts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cart_items_products_product_id",
                table: "cart_items",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_carts_users_user_id",
                table: "carts",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
