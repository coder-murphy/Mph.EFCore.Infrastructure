using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Mph.EFCore.Infrastructure.Common
{
    public static class SQLBuilder
    {
        /// <summary>
        /// 初始化审计日志(这里使用pgSQL)
        /// </summary>
        /// <param name="database"></param>
        public static void InitAuditSql(DatabaseFacade database)
        {
            database.ExecuteSqlRaw(AuditTriggerFuncSQL);

            // Eg.
            RegistAuditTrigger(database, "Your Table");
        }

        private static void RegistAuditTrigger(DatabaseFacade database,string tableName)
        {
            var sql = $@"
DROP TRIGGER IF EXISTS trg_{tableName}_audit ON ""{tableName}"";";
            database.ExecuteSqlRaw(sql);

            sql = $@"
CREATE TRIGGER trg_{tableName}_audit
AFTER INSERT OR UPDATE OR DELETE
ON ""{tableName}""
FOR EACH ROW
EXECUTE FUNCTION fn_audit_log_trigger();";
            database.ExecuteSqlRaw(sql);
        }

        private const string AuditTriggerFuncSQL = @$"
CREATE OR REPLACE FUNCTION ""public"".""fn_audit_log_trigger""()
  RETURNS ""pg_catalog"".""trigger"" AS $BODY$
DECLARE
    v_op_id BIGINT;
    v_op_name text := current_setting('app.operator_name', true);
    v_op_ip text := current_setting('app.operator_ip', true);
    v_op_record_id BIGINT;
    v_operate_at timestamptz;
BEGIN
--  只有值不为空时才转换，否则直接设为 NULL
    IF current_setting('app.operator_id', true) <> '' THEN
        v_op_id := current_setting('app.operator_id', true)::bigint;
    ELSE
        v_op_id := 0;
    END IF;

    IF current_setting('app.operator_record_id', true) <> '' THEN
        v_op_record_id := current_setting('app.operator_record_id', true)::bigint;
    ELSE
        v_op_record_id := 0;
    END IF;

    IF current_setting('app.operated_at', true) <> '' THEN
        v_operate_at := current_setting('app.operated_at', true)::timestamptz;
    ELSE
        v_operate_at := now(); -- 空就用数据库当前时间;
    END IF;

    IF TG_OP = 'INSERT' THEN
        INSERT INTO ""AuditLogs""
        (""TableName"", ""RecordId"", ""Action"", ""OperatorId"", ""NewValues"", ""OperatorName"", ""OperatorIp"",""OperatedAt"")
        VALUES
        (TG_TABLE_NAME,v_op_record_id, 0, v_op_id, to_jsonb(NEW), v_op_name, v_op_ip,v_operate_at);
        RETURN NEW;
    ELSIF TG_OP = 'UPDATE' THEN
         INSERT INTO ""AuditLogs""
        (""TableName"", ""RecordId"", ""Action"", ""OperatorId"", ""OldValues"", ""NewValues"", ""OperatorName"", ""OperatorIp"",""OperatedAt"")
        VALUES
        (TG_TABLE_NAME,v_op_record_id, 1, v_op_id, to_jsonb(OLD), to_jsonb(NEW), v_op_name, v_op_ip,v_operate_at);
        RETURN NEW;
    ELSIF TG_OP = 'DELETE' THEN
         INSERT INTO ""AuditLogs""
        (""TableName"", ""RecordId"", ""Action"", ""OperatorId"", ""OldValues"", ""OperatorName"", ""OperatorIp"",""OperatedAt"")
        VALUES
        (TG_TABLE_NAME,v_op_record_id, 2, v_op_id, to_jsonb(OLD), v_op_name, v_op_ip,v_operate_at);
        RETURN OLD;
    END IF;
END;
$BODY$
  LANGUAGE plpgsql";
    }
}
