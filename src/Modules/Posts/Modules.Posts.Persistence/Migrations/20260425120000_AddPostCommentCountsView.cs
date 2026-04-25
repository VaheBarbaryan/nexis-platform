using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Posts.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPostCommentCountsView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE MATERIALIZED VIEW posts.post_comment_counts AS
                SELECT post_id, COUNT(*) AS count
                FROM posts.comments
                WHERE deleted_at IS NULL
                GROUP BY post_id;

                CREATE UNIQUE INDEX idx_post_comment_counts_post_id
                ON posts.post_comment_counts (post_id);
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP MATERIALIZED VIEW IF EXISTS posts.post_comment_counts;");
        }
    }
}
