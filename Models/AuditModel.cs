namespace Cont5.Models.Audit {
    public class GetAuditLogModel {
        public string Api { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string Exception { get; set; }
        public string ClientIp { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDt { get; set; }
    }
    public class GetAuditLogRequest {
        public string Endpoint { get; set; }
        public string RequestResponse { get; set; }
        public string CreatedBy { get; set; }
    }
}