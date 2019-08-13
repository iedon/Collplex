// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Node.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Collplex.Models.Node {

  /// <summary>Holder for reflection information generated from Node.proto</summary>
  public static partial class NodeReflection {

    #region Descriptor
    /// <summary>File descriptor for Node.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static NodeReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgpOb2RlLnByb3RvEhRDb2xsUGxleC5Nb2RlbHMuTm9kZSLqAgoETm9kZRI0",
            "CghzZXJ2aWNlcxgBIAMoCzIiLkNvbGxQbGV4Lk1vZGVscy5Ob2RlLk5vZGUu",
            "U2VydmljZRIxCgZjb25maWcYAiABKAsyIS5Db2xsUGxleC5Nb2RlbHMuTm9k",
            "ZS5Ob2RlLkNvbmZpZxrIAQoHU2VydmljZRILCgNrZXkYASABKAkSPAoEdHlw",
            "ZRgCIAEoDjIuLkNvbGxQbGV4Lk1vZGVscy5Ob2RlLk5vZGUuU2VydmljZS5T",
            "ZXJ2aWNlVHlwZRIMCgRuYW1lGAMgASgJEg8KB25vZGVVcmwYBCABKAkSFAoM",
            "cmVnVGltZXN0YW1wGAUgASgDEhcKD2V4cGlyZVRpbWVzdGFtcBgGIAEoAyIk",
            "CgtTZXJ2aWNlVHlwZRIJCgVCQVNJQxAAEgoKBkNVU1RPTRABGi4KBkNvbmZp",
            "ZxITCgtyZWdJbnRlcnZhbBgBIAEoBRIPCgd0aW1lb3V0GAIgASgFYgZwcm90",
            "bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Collplex.Models.Node.Node), global::Collplex.Models.Node.Node.Parser, new[]{ "Services", "Config" }, null, null, null, new pbr::GeneratedClrTypeInfo[] { new pbr::GeneratedClrTypeInfo(typeof(global::Collplex.Models.Node.Node.Types.Service), global::Collplex.Models.Node.Node.Types.Service.Parser, new[]{ "Key", "Type", "Name", "NodeUrl", "RegTimestamp", "ExpireTimestamp" }, null, new[]{ typeof(global::Collplex.Models.Node.Node.Types.Service.Types.ServiceType) }, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Collplex.Models.Node.Node.Types.Config), global::Collplex.Models.Node.Node.Types.Config.Parser, new[]{ "RegInterval", "Timeout" }, null, null, null, null)})
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class Node : pb::IMessage<Node> {
    private static readonly pb::MessageParser<Node> _parser = new pb::MessageParser<Node>(() => new Node());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Node> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Collplex.Models.Node.NodeReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Node() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Node(Node other) : this() {
      services_ = other.services_.Clone();
      config_ = other.config_ != null ? other.config_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Node Clone() {
      return new Node(this);
    }

    /// <summary>Field number for the "services" field.</summary>
    public const int ServicesFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Collplex.Models.Node.Node.Types.Service> _repeated_services_codec
        = pb::FieldCodec.ForMessage(10, global::Collplex.Models.Node.Node.Types.Service.Parser);
    private readonly pbc::RepeatedField<global::Collplex.Models.Node.Node.Types.Service> services_ = new pbc::RepeatedField<global::Collplex.Models.Node.Node.Types.Service>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Collplex.Models.Node.Node.Types.Service> Services {
      get { return services_; }
    }

    /// <summary>Field number for the "config" field.</summary>
    public const int ConfigFieldNumber = 2;
    private global::Collplex.Models.Node.Node.Types.Config config_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Collplex.Models.Node.Node.Types.Config Config {
      get { return config_; }
      set {
        config_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Node);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Node other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!services_.Equals(other.services_)) return false;
      if (!object.Equals(Config, other.Config)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= services_.GetHashCode();
      if (config_ != null) hash ^= Config.GetHashCode();
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
      services_.WriteTo(output, _repeated_services_codec);
      if (config_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Config);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += services_.CalculateSize(_repeated_services_codec);
      if (config_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Config);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Node other) {
      if (other == null) {
        return;
      }
      services_.Add(other.services_);
      if (other.config_ != null) {
        if (config_ == null) {
          Config = new global::Collplex.Models.Node.Node.Types.Config();
        }
        Config.MergeFrom(other.Config);
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
          case 10: {
            services_.AddEntriesFrom(input, _repeated_services_codec);
            break;
          }
          case 18: {
            if (config_ == null) {
              Config = new global::Collplex.Models.Node.Node.Types.Config();
            }
            input.ReadMessage(Config);
            break;
          }
        }
      }
    }

    #region Nested types
    /// <summary>Container for nested types declared in the Node message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static partial class Types {
      /// <summary>
      /// 子节点业务元素 
      /// </summary>
      public sealed partial class Service : pb::IMessage<Service> {
        private static readonly pb::MessageParser<Service> _parser = new pb::MessageParser<Service>(() => new Service());
        private pb::UnknownFieldSet _unknownFields;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pb::MessageParser<Service> Parser { get { return _parser; } }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pbr::MessageDescriptor Descriptor {
          get { return global::Collplex.Models.Node.Node.Descriptor.NestedTypes[0]; }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        pbr::MessageDescriptor pb::IMessage.Descriptor {
          get { return Descriptor; }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Service() {
          OnConstruction();
        }

        partial void OnConstruction();

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Service(Service other) : this() {
          key_ = other.key_;
          type_ = other.type_;
          name_ = other.name_;
          nodeUrl_ = other.nodeUrl_;
          regTimestamp_ = other.regTimestamp_;
          expireTimestamp_ = other.expireTimestamp_;
          _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Service Clone() {
          return new Service(this);
        }

        /// <summary>Field number for the "key" field.</summary>
        public const int KeyFieldNumber = 1;
        private string key_ = "";
        /// <summary>
        /// 子节点业务标识 Key 
        /// </summary>
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public string Key {
          get { return key_; }
          set {
            key_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
          }
        }

        /// <summary>Field number for the "type" field.</summary>
        public const int TypeFieldNumber = 2;
        private global::Collplex.Models.Node.Node.Types.Service.Types.ServiceType type_ = global::Collplex.Models.Node.Node.Types.Service.Types.ServiceType.Basic;
        /// <summary>
        /// 子节点业务类型 
        /// </summary>
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public global::Collplex.Models.Node.Node.Types.Service.Types.ServiceType Type {
          get { return type_; }
          set {
            type_ = value;
          }
        }

        /// <summary>Field number for the "name" field.</summary>
        public const int NameFieldNumber = 3;
        private string name_ = "";
        /// <summary>
        /// 子节点友好名称 
        /// </summary>
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public string Name {
          get { return name_; }
          set {
            name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
          }
        }

        /// <summary>Field number for the "nodeUrl" field.</summary>
        public const int NodeUrlFieldNumber = 4;
        private string nodeUrl_ = "";
        /// <summary>
        /// 子节点所对应的该业务的完整URL 
        /// </summary>
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public string NodeUrl {
          get { return nodeUrl_; }
          set {
            nodeUrl_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
          }
        }

        /// <summary>Field number for the "regTimestamp" field.</summary>
        public const int RegTimestampFieldNumber = 5;
        private long regTimestamp_;
        /// <summary>
        /// UNIX 时间戳 首次注册的时间 
        /// </summary>
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public long RegTimestamp {
          get { return regTimestamp_; }
          set {
            regTimestamp_ = value;
          }
        }

        /// <summary>Field number for the "expireTimestamp" field.</summary>
        public const int ExpireTimestampFieldNumber = 6;
        private long expireTimestamp_;
        /// <summary>
        /// UNIX 时间戳 过期时间。如果子节点超过规定的时间(Colleges/reg_interval)没有报告自己存活，即此业务暂时不能使用。 
        /// </summary>
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public long ExpireTimestamp {
          get { return expireTimestamp_; }
          set {
            expireTimestamp_ = value;
          }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override bool Equals(object other) {
          return Equals(other as Service);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool Equals(Service other) {
          if (ReferenceEquals(other, null)) {
            return false;
          }
          if (ReferenceEquals(other, this)) {
            return true;
          }
          if (Key != other.Key) return false;
          if (Type != other.Type) return false;
          if (Name != other.Name) return false;
          if (NodeUrl != other.NodeUrl) return false;
          if (RegTimestamp != other.RegTimestamp) return false;
          if (ExpireTimestamp != other.ExpireTimestamp) return false;
          return Equals(_unknownFields, other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override int GetHashCode() {
          int hash = 1;
          if (Key.Length != 0) hash ^= Key.GetHashCode();
          if (Type != global::Collplex.Models.Node.Node.Types.Service.Types.ServiceType.Basic) hash ^= Type.GetHashCode();
          if (Name.Length != 0) hash ^= Name.GetHashCode();
          if (NodeUrl.Length != 0) hash ^= NodeUrl.GetHashCode();
          if (RegTimestamp != 0L) hash ^= RegTimestamp.GetHashCode();
          if (ExpireTimestamp != 0L) hash ^= ExpireTimestamp.GetHashCode();
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
          if (Key.Length != 0) {
            output.WriteRawTag(10);
            output.WriteString(Key);
          }
          if (Type != global::Collplex.Models.Node.Node.Types.Service.Types.ServiceType.Basic) {
            output.WriteRawTag(16);
            output.WriteEnum((int) Type);
          }
          if (Name.Length != 0) {
            output.WriteRawTag(26);
            output.WriteString(Name);
          }
          if (NodeUrl.Length != 0) {
            output.WriteRawTag(34);
            output.WriteString(NodeUrl);
          }
          if (RegTimestamp != 0L) {
            output.WriteRawTag(40);
            output.WriteInt64(RegTimestamp);
          }
          if (ExpireTimestamp != 0L) {
            output.WriteRawTag(48);
            output.WriteInt64(ExpireTimestamp);
          }
          if (_unknownFields != null) {
            _unknownFields.WriteTo(output);
          }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public int CalculateSize() {
          int size = 0;
          if (Key.Length != 0) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(Key);
          }
          if (Type != global::Collplex.Models.Node.Node.Types.Service.Types.ServiceType.Basic) {
            size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Type);
          }
          if (Name.Length != 0) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
          }
          if (NodeUrl.Length != 0) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(NodeUrl);
          }
          if (RegTimestamp != 0L) {
            size += 1 + pb::CodedOutputStream.ComputeInt64Size(RegTimestamp);
          }
          if (ExpireTimestamp != 0L) {
            size += 1 + pb::CodedOutputStream.ComputeInt64Size(ExpireTimestamp);
          }
          if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
          }
          return size;
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public void MergeFrom(Service other) {
          if (other == null) {
            return;
          }
          if (other.Key.Length != 0) {
            Key = other.Key;
          }
          if (other.Type != global::Collplex.Models.Node.Node.Types.Service.Types.ServiceType.Basic) {
            Type = other.Type;
          }
          if (other.Name.Length != 0) {
            Name = other.Name;
          }
          if (other.NodeUrl.Length != 0) {
            NodeUrl = other.NodeUrl;
          }
          if (other.RegTimestamp != 0L) {
            RegTimestamp = other.RegTimestamp;
          }
          if (other.ExpireTimestamp != 0L) {
            ExpireTimestamp = other.ExpireTimestamp;
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
              case 10: {
                Key = input.ReadString();
                break;
              }
              case 16: {
                Type = (global::Collplex.Models.Node.Node.Types.Service.Types.ServiceType) input.ReadEnum();
                break;
              }
              case 26: {
                Name = input.ReadString();
                break;
              }
              case 34: {
                NodeUrl = input.ReadString();
                break;
              }
              case 40: {
                RegTimestamp = input.ReadInt64();
                break;
              }
              case 48: {
                ExpireTimestamp = input.ReadInt64();
                break;
              }
            }
          }
        }

        #region Nested types
        /// <summary>Container for nested types declared in the Service message type.</summary>
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static partial class Types {
          public enum ServiceType {
            [pbr::OriginalName("BASIC")] Basic = 0,
            [pbr::OriginalName("CUSTOM")] Custom = 1,
          }

        }
        #endregion

      }

      /// <summary>
      /// (内部使用的信息，不会给API接口序列化发出去) 
      /// </summary>
      public sealed partial class Config : pb::IMessage<Config> {
        private static readonly pb::MessageParser<Config> _parser = new pb::MessageParser<Config>(() => new Config());
        private pb::UnknownFieldSet _unknownFields;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pb::MessageParser<Config> Parser { get { return _parser; } }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pbr::MessageDescriptor Descriptor {
          get { return global::Collplex.Models.Node.Node.Descriptor.NestedTypes[1]; }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        pbr::MessageDescriptor pb::IMessage.Descriptor {
          get { return Descriptor; }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Config() {
          OnConstruction();
        }

        partial void OnConstruction();

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Config(Config other) : this() {
          regInterval_ = other.regInterval_;
          timeout_ = other.timeout_;
          _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Config Clone() {
          return new Config(this);
        }

        /// <summary>Field number for the "regInterval" field.</summary>
        public const int RegIntervalFieldNumber = 1;
        private int regInterval_;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public int RegInterval {
          get { return regInterval_; }
          set {
            regInterval_ = value;
          }
        }

        /// <summary>Field number for the "timeout" field.</summary>
        public const int TimeoutFieldNumber = 2;
        private int timeout_;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public int Timeout {
          get { return timeout_; }
          set {
            timeout_ = value;
          }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override bool Equals(object other) {
          return Equals(other as Config);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool Equals(Config other) {
          if (ReferenceEquals(other, null)) {
            return false;
          }
          if (ReferenceEquals(other, this)) {
            return true;
          }
          if (RegInterval != other.RegInterval) return false;
          if (Timeout != other.Timeout) return false;
          return Equals(_unknownFields, other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override int GetHashCode() {
          int hash = 1;
          if (RegInterval != 0) hash ^= RegInterval.GetHashCode();
          if (Timeout != 0) hash ^= Timeout.GetHashCode();
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
          if (RegInterval != 0) {
            output.WriteRawTag(8);
            output.WriteInt32(RegInterval);
          }
          if (Timeout != 0) {
            output.WriteRawTag(16);
            output.WriteInt32(Timeout);
          }
          if (_unknownFields != null) {
            _unknownFields.WriteTo(output);
          }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public int CalculateSize() {
          int size = 0;
          if (RegInterval != 0) {
            size += 1 + pb::CodedOutputStream.ComputeInt32Size(RegInterval);
          }
          if (Timeout != 0) {
            size += 1 + pb::CodedOutputStream.ComputeInt32Size(Timeout);
          }
          if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
          }
          return size;
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public void MergeFrom(Config other) {
          if (other == null) {
            return;
          }
          if (other.RegInterval != 0) {
            RegInterval = other.RegInterval;
          }
          if (other.Timeout != 0) {
            Timeout = other.Timeout;
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
                RegInterval = input.ReadInt32();
                break;
              }
              case 16: {
                Timeout = input.ReadInt32();
                break;
              }
            }
          }
        }

      }

    }
    #endregion

  }

  #endregion

}

#endregion Designer generated code
