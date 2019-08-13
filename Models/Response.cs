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
        NODE_REG_CUSTOM_URL_LIMIT = 2002,
        NODE_LOCK_TIMEOUT = 2003,
        SVC_INVALID_CLIENT_ID = 3000
    }

    public struct ResponsePacket
    {
        public ResponseCodeType Code;
        public string Message;
        public object Data;
    }
}
