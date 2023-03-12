using System;
using System.Collections.Generic;
using System.Linq;
using Cont5.Models.Audit;
using Cont5.Helpers;

namespace Cont5.Services {
    public interface IAuditService {
        List<GetAuditLogModel> GetAuditLog(GetAuditLogRequest request);
    }

    public class AuditService : IAuditService {
        private readonly IUserAccountHelper userAccountHelper;
        private readonly IJsonHelper jsonHelper;

        public AuditService(IUserAccountHelper userAccountHelper, IJsonHelper jsonHelper) {
            this.userAccountHelper = userAccountHelper;
            this.jsonHelper = jsonHelper;
        }

        public List<GetAuditLogModel> GetAuditLog(GetAuditLogRequest request) {
            // TODO: Enhance to return exception
            var result = new List<GetAuditLogModel>();
            dynamic exception = null;
            var isFiltering = !(String.IsNullOrWhiteSpace(request.Endpoint) && String.IsNullOrWhiteSpace(request.RequestResponse) && String.IsNullOrWhiteSpace(request.CreatedBy));

            try {
                if (userAccountHelper.CurrentLogin.RoleId > 1 ) {
                    throw new SystemException($"Unauthorized Access");
                }

                var tempResult = jsonHelper.AuditList.OrderByDescending(o => o.CreatedDt).Select(s => new GetAuditLogModel{
                    Api = s.Api,
                    Request = s.Request,
                    Response = s.Response,
                    Exception = s.Exception,
                    ClientIp = s.ClientIp,
                    CreatedBy = userAccountHelper.AccountList.Where(w => w.UserId == s.CreatedBy).Select(s => s.UserName).FirstOrDefault(),
                    CreatedDt = s.CreatedDt.ToString("dd/MM/yyyy HH:mm:ss"),
                }).Where(w => ((w.Api ?? "").Contains(request.Endpoint, StringComparison.OrdinalIgnoreCase))
                                && (((w.Request ?? "").Contains(request.RequestResponse, StringComparison.OrdinalIgnoreCase)) || ((w.Response ?? "").Contains(request.RequestResponse, StringComparison.OrdinalIgnoreCase)))
                                && ((w.CreatedBy ?? "").Contains(request.CreatedBy, StringComparison.OrdinalIgnoreCase)) );

                result = tempResult.Take(isFiltering ? tempResult.Count() : 50).ToList();

            } catch (Exception ex) {
                exception = new {
                    Exception = ex.Message,
                };
            }

            return result;
        }
    }
}