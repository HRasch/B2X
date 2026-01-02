using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace B2Connect.Shared.Monitoring.Migrations
{
    /// <inheritdoc />
    public partial class InitialMonitoringSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "connected_services",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    endpoint = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    last_checked = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_successful = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    average_latency_ms = table.Column<double>(type: "double precision", nullable: false),
                    uptime_percent = table.Column<double>(type: "double precision", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_connected_services", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "scheduler_jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    progress_percent = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    metadata = table.Column<string>(type: "jsonb", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scheduler_jobs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "connection_test_results",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    executed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    success = table.Column<bool>(type: "boolean", nullable: false),
                    dns_resolution_ms = table.Column<double>(type: "double precision", nullable: false),
                    tls_handshake_ms = table.Column<double>(type: "double precision", nullable: false),
                    total_connection_ms = table.Column<double>(type: "double precision", nullable: false),
                    speed_rating = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    error_message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    recommendations = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_connection_test_results", x => x.id);
                    table.ForeignKey(
                        name: "FK_connection_test_results_connected_services_service_id",
                        column: x => x.service_id,
                        principalTable: "connected_services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "resource_alerts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    alert_type = table.Column<int>(type: "integer", nullable: false),
                    threshold_value = table.Column<double>(type: "double precision", nullable: false),
                    actual_value = table.Column<double>(type: "double precision", nullable: false),
                    triggered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    resolved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resource_alerts", x => x.id);
                    table.ForeignKey(
                        name: "FK_resource_alerts_connected_services_service_id",
                        column: x => x.service_id,
                        principalTable: "connected_services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "resource_metrics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cpu_percent = table.Column<double>(type: "double precision", nullable: false),
                    cpu_average = table.Column<double>(type: "double precision", nullable: false),
                    cpu_peak = table.Column<double>(type: "double precision", nullable: false),
                    memory_used_bytes = table.Column<long>(type: "bigint", nullable: false),
                    memory_total_bytes = table.Column<long>(type: "bigint", nullable: false),
                    memory_percent = table.Column<double>(type: "double precision", nullable: false),
                    memory_peak_bytes = table.Column<long>(type: "bigint", nullable: false),
                    connection_pool_active = table.Column<int>(type: "integer", nullable: false),
                    connection_pool_total = table.Column<int>(type: "integer", nullable: false),
                    thread_count = table.Column<int>(type: "integer", nullable: false),
                    queue_depth = table.Column<int>(type: "integer", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resource_metrics", x => x.id);
                    table.ForeignKey(
                        name: "FK_resource_metrics_connected_services_service_id",
                        column: x => x.service_id,
                        principalTable: "connected_services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "communication_errors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    http_method = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    request_url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    status_code = table.Column<int>(type: "integer", nullable: true),
                    error_type = table.Column<int>(type: "integer", nullable: false),
                    message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    request_headers = table.Column<string>(type: "jsonb", nullable: true),
                    response_headers = table.Column<string>(type: "jsonb", nullable: true),
                    request_body = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    response_body = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    dns_resolution_ms = table.Column<double>(type: "double precision", nullable: true),
                    tcp_connection_ms = table.Column<double>(type: "double precision", nullable: true),
                    tls_handshake_ms = table.Column<double>(type: "double precision", nullable: true),
                    time_to_first_byte_ms = table.Column<double>(type: "double precision", nullable: true),
                    total_response_time_ms = table.Column<double>(type: "double precision", nullable: true),
                    retry_count = table.Column<int>(type: "integer", nullable: false),
                    correlated_job_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_communication_errors", x => x.id);
                    table.ForeignKey(
                        name: "FK_communication_errors_connected_services_service_id",
                        column: x => x.service_id,
                        principalTable: "connected_services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_communication_errors_scheduler_jobs_correlated_job_id",
                        column: x => x.correlated_job_id,
                        principalTable: "scheduler_jobs",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "job_execution_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    message = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    stack_trace = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    context = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_execution_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_job_execution_logs_scheduler_jobs_job_id",
                        column: x => x.job_id,
                        principalTable: "scheduler_jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "service_errors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    error_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    stack_trace = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    correlated_job_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_errors", x => x.id);
                    table.ForeignKey(
                        name: "FK_service_errors_connected_services_service_id",
                        column: x => x.service_id,
                        principalTable: "connected_services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_service_errors_scheduler_jobs_correlated_job_id",
                        column: x => x.correlated_job_id,
                        principalTable: "scheduler_jobs",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_communication_errors_correlated_job_id",
                table: "communication_errors",
                column: "correlated_job_id");

            migrationBuilder.CreateIndex(
                name: "IX_communication_errors_service_id",
                table: "communication_errors",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_communication_errors_tenant_id",
                table: "communication_errors",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_communication_errors_tenant_id_service_id",
                table: "communication_errors",
                columns: new[] { "tenant_id", "service_id" });

            migrationBuilder.CreateIndex(
                name: "IX_connected_services_tenant_id",
                table: "connected_services",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_connected_services_tenant_id_status",
                table: "connected_services",
                columns: new[] { "tenant_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_connection_test_results_service_id",
                table: "connection_test_results",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_connection_test_results_tenant_id",
                table: "connection_test_results",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_connection_test_results_tenant_id_service_id",
                table: "connection_test_results",
                columns: new[] { "tenant_id", "service_id" });

            migrationBuilder.CreateIndex(
                name: "IX_job_execution_logs_job_id",
                table: "job_execution_logs",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_execution_logs_tenant_id",
                table: "job_execution_logs",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_execution_logs_tenant_id_job_id",
                table: "job_execution_logs",
                columns: new[] { "tenant_id", "job_id" });

            migrationBuilder.CreateIndex(
                name: "IX_resource_alerts_service_id",
                table: "resource_alerts",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_resource_alerts_tenant_id",
                table: "resource_alerts",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_resource_alerts_tenant_id_service_id",
                table: "resource_alerts",
                columns: new[] { "tenant_id", "service_id" });

            migrationBuilder.CreateIndex(
                name: "IX_resource_metrics_service_id",
                table: "resource_metrics",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_resource_metrics_tenant_id",
                table: "resource_metrics",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_resource_metrics_tenant_id_service_id",
                table: "resource_metrics",
                columns: new[] { "tenant_id", "service_id" });

            migrationBuilder.CreateIndex(
                name: "IX_scheduler_jobs_tenant_id",
                table: "scheduler_jobs",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_scheduler_jobs_tenant_id_status",
                table: "scheduler_jobs",
                columns: new[] { "tenant_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_service_errors_correlated_job_id",
                table: "service_errors",
                column: "correlated_job_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_errors_service_id",
                table: "service_errors",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_errors_tenant_id",
                table: "service_errors",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_errors_tenant_id_service_id",
                table: "service_errors",
                columns: new[] { "tenant_id", "service_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "communication_errors");

            migrationBuilder.DropTable(
                name: "connection_test_results");

            migrationBuilder.DropTable(
                name: "job_execution_logs");

            migrationBuilder.DropTable(
                name: "resource_alerts");

            migrationBuilder.DropTable(
                name: "resource_metrics");

            migrationBuilder.DropTable(
                name: "service_errors");

            migrationBuilder.DropTable(
                name: "connected_services");

            migrationBuilder.DropTable(
                name: "scheduler_jobs");
        }
    }
}
