using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Posts.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPostLikesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "post_likes",
                schema: "posts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp(0) with time zone", precision: 0, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post_likes", x => x.id);
                    table.ForeignKey(
                        name: "fk_post_likes_posts_post_id",
                        column: x => x.post_id,
                        principalSchema: "posts",
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_post_likes_post_id_author_id",
                schema: "posts",
                table: "post_likes",
                columns: new[] { "post_id", "author_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "post_likes",
                schema: "posts");
        }
    }
}
