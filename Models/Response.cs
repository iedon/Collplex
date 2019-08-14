using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collplex.Models
{
    public enum ResponseCodeType : ushort
    {
        OK = 0,
        SERVER_EXCEPTION = 1000,
        NOT_FOUND = 1001,
        FORBIDDEN = 1002,
        BAD_GATEWAY = 1003,
        BAD_REQUEST = 1004,
        SERVICE_UNAVAILABLE = 1005,
        METHOD_NOT_ALLOWED = 1006,
        INVALID_BODY = 1007,
        NODE_INVALID_CLIENT_ID_OR_SECRET = 2000,
        NODE_OPERATION_FAILED = 2001,
        NODE_REG_CUSTOM_SVC_LIMIT = 2002,
        NODE_LOCK_TIMEOUT = 2003,
        NODE_RESPONSE_ERROR = 2004,
        NODE_RESPONSE_TIMEDOUT = 2005,
        SVC_INVALID_CLIENT_ID = 3000,
        SVC_NOT_FOUND = 3001
    }

    public class ResponsePacket
    {
        public ResponseCodeType Code;
        public string Message;
        public object Data;
    }
}
