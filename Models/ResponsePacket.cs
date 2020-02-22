// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: ResponsePacket.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Collplex.Models {

  /// <summary>Holder for reflection information generated from ResponsePacket.proto</summary>
  public static partial class ResponsePacketReflection {

    #region Descriptor
    /// <summary>File descriptor for ResponsePacket.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ResponsePacketReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChRSZXNwb25zZVBhY2tldC5wcm90bxIPQ29sbHBsZXguTW9kZWxzIq0ECg5S",
            "ZXNwb25zZVBhY2tldBI+CgRjb2RlGAEgASgOMjAuQ29sbHBsZXguTW9kZWxz",
            "LlJlc3BvbnNlUGFja2V0LlJlc3BvbnNlQ29kZVR5cGUSDwoHbWVzc2FnZRgC",
            "IAEoCRIMCgRkYXRhGAMgASgJIrsDChBSZXNwb25zZUNvZGVUeXBlEgYKAk9L",
            "EAASFQoQU0VSVkVSX0VYQ0VQVElPThDoBxIOCglOT1RfRk9VTkQQ6QcSDgoJ",
            "Rk9SQklEREVOEOoHEhAKC0JBRF9HQVRFV0FZEOsHEhAKC0JBRF9SRVFVRVNU",
            "EOwHEhgKE1NFUlZJQ0VfVU5BVkFJTEFCTEUQ7QcSFwoSTUVUSE9EX05PVF9B",
            "TExPV0VEEO4HEhEKDElOVkFMSURfQk9EWRDvBxIbChZOT0RFX0lOVkFMSURf",
            "Q0xJRU5UX0lEENAPEhoKFU5PREVfT1BFUkFUSU9OX0ZBSUxFRBDRDxITCg5O",
            "T0RFX1JFR19MSU1JVBDSDxIWChFOT0RFX0xPQ0tfVElNRU9VVBDTDxIYChNO",
            "T0RFX1JFU1BPTlNFX0VSUk9SENQPEhsKFk5PREVfUkVTUE9OU0VfVElNRURP",
            "VVQQ1Q8SGwoWTk9ERV9ORVRXT1JLX0VYQ0VQVElPThDWDxIaChVTVkNfSU5W",
            "QUxJRF9DTElFTlRfSUQQuBcSEgoNU1ZDX05PVF9GT1VORBC5FxIUCg9TVkNf",
            "VU5BVkFJTEFCTEUQuhdiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Collplex.Models.ResponsePacket), global::Collplex.Models.ResponsePacket.Parser, new[]{ "Code", "Message", "Data" }, null, new[]{ typeof(global::Collplex.Models.ResponsePacket.Types.ResponseCodeType) }, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  /// <summary>
  /// 响应 
  /// </summary>
  public sealed partial class ResponsePacket : pb::IMessage<ResponsePacket> {
    private static readonly pb::MessageParser<ResponsePacket> _parser = new pb::MessageParser<ResponsePacket>(() => new ResponsePacket());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ResponsePacket> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Collplex.Models.ResponsePacketReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ResponsePacket() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ResponsePacket(ResponsePacket other) : this() {
      code_ = other.code_;
      message_ = other.message_;
      data_ = other.data_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ResponsePacket Clone() {
      return new ResponsePacket(this);
    }

    /// <summary>Field number for the "code" field.</summary>
    public const int CodeFieldNumber = 1;
    private global::Collplex.Models.ResponsePacket.Types.ResponseCodeType code_ = global::Collplex.Models.ResponsePacket.Types.ResponseCodeType.Ok;
    /// <summary>
    /// 响应状态码 
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Collplex.Models.ResponsePacket.Types.ResponseCodeType Code {
      get { return code_; }
      set {
        code_ = value;
      }
    }

    /// <summary>Field number for the "message" field.</summary>
    public const int MessageFieldNumber = 2;
    private string message_ = "";
    /// <summary>
    /// 响应状态码的消息 
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Message {
      get { return message_; }
      set {
        message_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "data" field.</summary>
    public const int DataFieldNumber = 3;
    private string data_ = "";
    /// <summary>
    /// JSON 数据 
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Data {
      get { return data_; }
      set {
        data_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as ResponsePacket);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(ResponsePacket other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Code != other.Code) return false;
      if (Message != other.Message) return false;
      if (Data != other.Data) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Code != global::Collplex.Models.ResponsePacket.Types.ResponseCodeType.Ok) hash ^= Code.GetHashCode();
      if (Message.Length != 0) hash ^= Message.GetHashCode();
      if (Data.Length != 0) hash ^= Data.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Code != global::Collplex.Models.ResponsePacket.Types.ResponseCodeType.Ok) {
        output.WriteRawTag(8);
        output.WriteEnum((int) Code);
      }
      if (Message.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Message);
      }
      if (Data.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(Data);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Code != global::Collplex.Models.ResponsePacket.Types.ResponseCodeType.Ok) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Code);
      }
      if (Message.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Message);
      }
      if (Data.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Data);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(ResponsePacket other) {
      if (other == null) {
        return;
      }
      if (other.Code != global::Collplex.Models.ResponsePacket.Types.ResponseCodeType.Ok) {
        Code = other.Code;
      }
      if (other.Message.Length != 0) {
        Message = other.Message;
      }
      if (other.Data.Length != 0) {
        Data = other.Data;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Code = (global::Collplex.Models.ResponsePacket.Types.ResponseCodeType) input.ReadEnum();
            break;
          }
          case 18: {
            Message = input.ReadString();
            break;
          }
          case 26: {
            Data = input.ReadString();
            break;
          }
        }
      }
    }

    #region Nested types
    /// <summary>Container for nested types declared in the ResponsePacket message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static partial class Types {
      public enum ResponseCodeType {
        [pbr::OriginalName("OK")] Ok = 0,
        [pbr::OriginalName("SERVER_EXCEPTION")] ServerException = 1000,
        [pbr::OriginalName("NOT_FOUND")] NotFound = 1001,
        [pbr::OriginalName("FORBIDDEN")] Forbidden = 1002,
        [pbr::OriginalName("BAD_GATEWAY")] BadGateway = 1003,
        [pbr::OriginalName("BAD_REQUEST")] BadRequest = 1004,
        [pbr::OriginalName("SERVICE_UNAVAILABLE")] ServiceUnavailable = 1005,
        [pbr::OriginalName("METHOD_NOT_ALLOWED")] MethodNotAllowed = 1006,
        [pbr::OriginalName("INVALID_BODY")] InvalidBody = 1007,
        [pbr::OriginalName("NODE_INVALID_CLIENT_ID")] NodeInvalidClientId = 2000,
        [pbr::OriginalName("NODE_OPERATION_FAILED")] NodeOperationFailed = 2001,
        [pbr::OriginalName("NODE_REG_LIMIT")] NodeRegLimit = 2002,
        [pbr::OriginalName("NODE_LOCK_TIMEOUT")] NodeLockTimeout = 2003,
        [pbr::OriginalName("NODE_RESPONSE_ERROR")] NodeResponseError = 2004,
        [pbr::OriginalName("NODE_RESPONSE_TIMEDOUT")] NodeResponseTimedout = 2005,
        [pbr::OriginalName("NODE_NETWORK_EXCEPTION")] NodeNetworkException = 2006,
        [pbr::OriginalName("SVC_INVALID_CLIENT_ID")] SvcInvalidClientId = 3000,
        [pbr::OriginalName("SVC_NOT_FOUND")] SvcNotFound = 3001,
        [pbr::OriginalName("SVC_UNAVAILABLE")] SvcUnavailable = 3002,
      }

    }
    #endregion

  }

  #endregion

}

#endregion Designer generated code