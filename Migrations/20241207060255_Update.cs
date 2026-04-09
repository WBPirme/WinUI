using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OGAS.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "材料s",
                columns: table => new
                {
                    材料ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    材料类别 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    名称 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    描述 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    计量单位 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    每单位价格 = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_材料s", x => x.材料ID);
                });

            migrationBuilder.CreateTable(
                name: "产品s",
                columns: table => new
                {
                    产品ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    产品类别 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    名称 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    描述 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    计量单位 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    每单位价格 = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_产品s", x => x.产品ID);
                });

            migrationBuilder.CreateTable(
                name: "供应商s",
                columns: table => new
                {
                    供应商ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    名称 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    联系信息 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    位置 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    备注 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_供应商s", x => x.供应商ID);
                });

            migrationBuilder.CreateTable(
                name: "生产设备s",
                columns: table => new
                {
                    生产设备ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    名称 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    描述 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    容量 = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    效率 = table.Column<decimal>(type: "decimal(3,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_生产设备s", x => x.生产设备ID);
                });

            migrationBuilder.CreateTable(
                name: "用户",
                columns: table => new
                {
                    编号 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    密码 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_用户", x => x.编号);
                });

            migrationBuilder.CreateTable(
                name: "材料库存s",
                columns: table => new
                {
                    材料库存ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    材料ID = table.Column<int>(type: "int", nullable: true),
                    材料类型 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    数量 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    计量单位 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    最后更新时间 = table.Column<DateTime>(type: "datetime2", nullable: false),
                    备注 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_材料库存s", x => x.材料库存ID);
                    table.ForeignKey(
                        name: "FK_材料库存s_材料s_材料ID",
                        column: x => x.材料ID,
                        principalTable: "材料s",
                        principalColumn: "材料ID");
                });

            migrationBuilder.CreateTable(
                name: "产品库存s",
                columns: table => new
                {
                    产品库存ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    产品ID = table.Column<int>(type: "int", nullable: true),
                    产品类型 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    数量 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    计量单位 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    最后更新时间 = table.Column<DateTime>(type: "datetime2", nullable: false),
                    备注 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_产品库存s", x => x.产品库存ID);
                    table.ForeignKey(
                        name: "FK_产品库存s_产品s_产品ID",
                        column: x => x.产品ID,
                        principalTable: "产品s",
                        principalColumn: "产品ID");
                });

            migrationBuilder.CreateTable(
                name: "出口订单s",
                columns: table => new
                {
                    订单ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    订单日期 = table.Column<DateTime>(type: "datetime2", nullable: false),
                    产品ID = table.Column<int>(type: "int", nullable: false),
                    出口数量 = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    收益 = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_出口订单s", x => x.订单ID);
                    table.ForeignKey(
                        name: "FK_出口订单s_产品s_产品ID",
                        column: x => x.产品ID,
                        principalTable: "产品s",
                        principalColumn: "产品ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "采购记录",
                columns: table => new
                {
                    采购记录ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    材料ID = table.Column<int>(type: "int", nullable: false),
                    数量 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    单价 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    总价 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    供应商ID = table.Column<int>(type: "int", nullable: false),
                    采购日期 = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_采购记录", x => x.采购记录ID);
                    table.ForeignKey(
                        name: "FK_采购记录_供应商s_供应商ID",
                        column: x => x.供应商ID,
                        principalTable: "供应商s",
                        principalColumn: "供应商ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_采购记录_材料s_材料ID",
                        column: x => x.材料ID,
                        principalTable: "材料s",
                        principalColumn: "材料ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "供应商产品s",
                columns: table => new
                {
                    供应商产品ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    供应商ID = table.Column<int>(type: "int", nullable: false),
                    材料ID = table.Column<int>(type: "int", nullable: false),
                    单价 = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_供应商产品s", x => x.供应商产品ID);
                    table.ForeignKey(
                        name: "FK_供应商产品s_供应商s_供应商ID",
                        column: x => x.供应商ID,
                        principalTable: "供应商s",
                        principalColumn: "供应商ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_供应商产品s_材料s_材料ID",
                        column: x => x.材料ID,
                        principalTable: "材料s",
                        principalColumn: "材料ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "加工厂s",
                columns: table => new
                {
                    加工厂ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    名称 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    生产设备ID = table.Column<int>(type: "int", nullable: true),
                    设备数量 = table.Column<int>(type: "int", nullable: true),
                    位置 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    加工效率 = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    联系人信息 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    状态 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_加工厂s", x => x.加工厂ID);
                    table.ForeignKey(
                        name: "FK_加工厂s_生产设备s_生产设备ID",
                        column: x => x.生产设备ID,
                        principalTable: "生产设备s",
                        principalColumn: "生产设备ID");
                });

            migrationBuilder.CreateTable(
                name: "生产工艺s",
                columns: table => new
                {
                    工艺ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    产品ID = table.Column<int>(type: "int", nullable: false),
                    材料ID = table.Column<int>(type: "int", nullable: true),
                    生产设备ID = table.Column<int>(type: "int", nullable: false),
                    工艺名称 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    标准加工时间 = table.Column<int>(type: "int", nullable: false),
                    质量标准 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    安全指引 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    备注 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_生产工艺s", x => x.工艺ID);
                    table.ForeignKey(
                        name: "FK_生产工艺s_产品s_产品ID",
                        column: x => x.产品ID,
                        principalTable: "产品s",
                        principalColumn: "产品ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_生产工艺s_材料s_材料ID",
                        column: x => x.材料ID,
                        principalTable: "材料s",
                        principalColumn: "材料ID");
                    table.ForeignKey(
                        name: "FK_生产工艺s_生产设备s_生产设备ID",
                        column: x => x.生产设备ID,
                        principalTable: "生产设备s",
                        principalColumn: "生产设备ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "生产计划s",
                columns: table => new
                {
                    计划ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    计划类型 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    加工厂ID = table.Column<int>(type: "int", nullable: false),
                    工艺ID = table.Column<long>(type: "bigint", nullable: false),
                    计划开始日期 = table.Column<DateTime>(type: "datetime2", nullable: false),
                    计划数量 = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    状态 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    备注 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_生产计划s", x => x.计划ID);
                    table.ForeignKey(
                        name: "FK_生产计划s_加工厂s_加工厂ID",
                        column: x => x.加工厂ID,
                        principalTable: "加工厂s",
                        principalColumn: "加工厂ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_生产计划s_生产工艺s_工艺ID",
                        column: x => x.工艺ID,
                        principalTable: "生产工艺s",
                        principalColumn: "工艺ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "生产记录s",
                columns: table => new
                {
                    生产记录ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    记录类型 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    计划ID = table.Column<long>(type: "bigint", nullable: false),
                    材料来源 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    材料数量 = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    材料成本 = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    加工厂ID = table.Column<int>(type: "int", nullable: false),
                    工艺ID = table.Column<long>(type: "bigint", nullable: false),
                    实际开始日期 = table.Column<DateTime>(type: "datetime2", nullable: false),
                    实际结束日期 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    生产数量 = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    生产时间 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    质量指标 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    备注 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    生产设备ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_生产记录s", x => x.生产记录ID);
                    table.ForeignKey(
                        name: "FK_生产记录s_加工厂s_加工厂ID",
                        column: x => x.加工厂ID,
                        principalTable: "加工厂s",
                        principalColumn: "加工厂ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_生产记录s_生产工艺s_工艺ID",
                        column: x => x.工艺ID,
                        principalTable: "生产工艺s",
                        principalColumn: "工艺ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_生产记录s_生产计划s_计划ID",
                        column: x => x.计划ID,
                        principalTable: "生产计划s",
                        principalColumn: "计划ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_生产记录s_生产设备s_生产设备ID",
                        column: x => x.生产设备ID,
                        principalTable: "生产设备s",
                        principalColumn: "生产设备ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_材料库存s_材料ID",
                table: "材料库存s",
                column: "材料ID");

            migrationBuilder.CreateIndex(
                name: "IX_采购记录_材料ID",
                table: "采购记录",
                column: "材料ID");

            migrationBuilder.CreateIndex(
                name: "IX_采购记录_供应商ID",
                table: "采购记录",
                column: "供应商ID");

            migrationBuilder.CreateIndex(
                name: "IX_产品库存s_产品ID",
                table: "产品库存s",
                column: "产品ID");

            migrationBuilder.CreateIndex(
                name: "IX_出口订单s_产品ID",
                table: "出口订单s",
                column: "产品ID");

            migrationBuilder.CreateIndex(
                name: "IX_供应商产品s_材料ID",
                table: "供应商产品s",
                column: "材料ID");

            migrationBuilder.CreateIndex(
                name: "IX_供应商产品s_供应商ID",
                table: "供应商产品s",
                column: "供应商ID");

            migrationBuilder.CreateIndex(
                name: "IX_加工厂s_生产设备ID",
                table: "加工厂s",
                column: "生产设备ID");

            migrationBuilder.CreateIndex(
                name: "IX_生产工艺s_材料ID",
                table: "生产工艺s",
                column: "材料ID");

            migrationBuilder.CreateIndex(
                name: "IX_生产工艺s_产品ID",
                table: "生产工艺s",
                column: "产品ID");

            migrationBuilder.CreateIndex(
                name: "IX_生产工艺s_生产设备ID",
                table: "生产工艺s",
                column: "生产设备ID");

            migrationBuilder.CreateIndex(
                name: "IX_生产计划s_工艺ID",
                table: "生产计划s",
                column: "工艺ID");

            migrationBuilder.CreateIndex(
                name: "IX_生产计划s_加工厂ID",
                table: "生产计划s",
                column: "加工厂ID");

            migrationBuilder.CreateIndex(
                name: "IX_生产记录s_工艺ID",
                table: "生产记录s",
                column: "工艺ID");

            migrationBuilder.CreateIndex(
                name: "IX_生产记录s_计划ID",
                table: "生产记录s",
                column: "计划ID");

            migrationBuilder.CreateIndex(
                name: "IX_生产记录s_加工厂ID",
                table: "生产记录s",
                column: "加工厂ID");

            migrationBuilder.CreateIndex(
                name: "IX_生产记录s_生产设备ID",
                table: "生产记录s",
                column: "生产设备ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "材料库存s");

            migrationBuilder.DropTable(
                name: "采购记录");

            migrationBuilder.DropTable(
                name: "产品库存s");

            migrationBuilder.DropTable(
                name: "出口订单s");

            migrationBuilder.DropTable(
                name: "供应商产品s");

            migrationBuilder.DropTable(
                name: "生产记录s");

            migrationBuilder.DropTable(
                name: "用户");

            migrationBuilder.DropTable(
                name: "供应商s");

            migrationBuilder.DropTable(
                name: "生产计划s");

            migrationBuilder.DropTable(
                name: "加工厂s");

            migrationBuilder.DropTable(
                name: "生产工艺s");

            migrationBuilder.DropTable(
                name: "产品s");

            migrationBuilder.DropTable(
                name: "材料s");

            migrationBuilder.DropTable(
                name: "生产设备s");
        }
    }
}
