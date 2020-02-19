// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Client.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Collplex.Models {

  /// <summary>Holder for reflection information generated from Client.proto</summary>
  public static partial class ClientReflection {

    #region Descriptor
    /// <summary>File descriptor for Client.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ClientReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgxDbGllbnQucHJvdG8SD0NvbGxwbGV4Lk1vZGVscyK8AwoGQ2xpZW50EgwK",
            "BG5hbWUYASABKAkSFAoMY2xpZW50U2VjcmV0GAIgASgJEhMKC21heFNlcnZp",
            "Y2VzGAMgASgFEhoKEnJlZ0ludGVydmFsU2Vjb25kcxgEIAEoBRIPCgd0aW1l",
            "b3V0GAUgASgFElUKGmxvYWRCYWxhbmNlckNvbmZpZ3VyYXRpb25zGAYgAygL",
            "MjEuQ29sbHBsZXguTW9kZWxzLkNsaWVudC5Mb2FkQmFsYW5jZXJDb25maWd1",
            "cmF0aW9uGvQBChlMb2FkQmFsYW5jZXJDb25maWd1cmF0aW9uEgsKA2tleRgB",
            "IAEoCRJPCgR0eXBlGAIgASgOMkEuQ29sbHBsZXguTW9kZWxzLkNsaWVudC5M",
            "b2FkQmFsYW5jZXJDb25maWd1cmF0aW9uLkxvYWRCYWxhbmNlVHlwZSJ5Cg9M",
            "b2FkQmFsYW5jZVR5cGUSEwoPTk9fTE9BRF9CQUxBTkNFEAASHQoZU01PT1RI",
            "X1dFSUdIVF9ST1VORF9ST0JJThABEhIKDkxFQVNUX1JFUVVFU1RTEAISCgoG",
            "UkFORE9NEAMSEgoOU09VUkNFX0lQX0hBU0gQBGIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Collplex.Models.Client), global::Collplex.Models.Client.Parser, new[]{ "Name", "ClientSecret", "MaxServices", "RegIntervalSeconds", "Timeout", "LoadBalancerConfigurations" }, null, null, null, new pbr::GeneratedClrTypeInfo[] { new pbr::GeneratedClrTypeInfo(typeof(global::Collplex.Models.Client.Types.LoadBalancerConfiguration), global::Collplex.Models.Client.Types.LoadBalancerConfiguration.Parser, new[]{ "Key", "Type" }, null, new[]{ typeof(global::Collplex.Models.Client.Types.LoadBalancerConfiguration.Types.LoadBalanceType) }, null, null)})
          }));
    }
    #endregion

  }
  #region Messages
  /// <summary>
  /// 客户 
  /// </summary>
  public sealed partial class Client : pb::IMessage<Client> {
    private static readonly pb::MessageParser<Client> _parser = new pb::MessageParser<Client>(() => new Client());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Client> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Collplex.Models.ClientReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Client() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Client(Client other) : this() {
      name_ = other.name_;
      clientSecret_ = other.clientSecret_;
      maxServices_ = other.maxServices_;
      regIntervalSeconds_ = other.regIntervalSeconds_;
      timeout_ = other.timeout_;
      loadBalancerConfigurations_ = other.loadBalancerConfigurations_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Client Clone() {
      return new Client(this);
    }

    /// <summary>Field number for the "name" field.</summary>
    public const int NameFieldNumber = 1;
    private string name_ = "";
    /// <summary>
    /// 客户名称 
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "clientSecret" field.</summary>
    public const int ClientSecretFieldNumber = 2;
    private string clientSecret_ = "";
    /// <summary>
    /// 客户密钥，用于数据加解密 
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string ClientSecret {
      get { return clientSecret_; }
      set {
        clientSecret_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "maxServices" field.</summary>
    public const int MaxServicesFieldNumber = 3;
    private int maxServices_;
    /// <summary>
    /// 客户子节点总共最多可以注册多少业务 
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int MaxServices {
      get { return maxServices_; }
      set {
        maxServices_ = value;
      }
    }

    /// <summary>Field number for the "regIntervalSeconds" field.</summary>
    public const int RegIntervalSecondsFieldNumber = 4;
    private int regIntervalSeconds_;
    /// <summary>
    /// 客户子节点注册周期，单位秒，客户子节点必须在这个时间以内至少重复注册一次以报告存活，否则本中心节点会认为子节点业务宕机 
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int RegIntervalSeconds {
      get { return regIntervalSeconds_; }
      set {
        regIntervalSeconds_ = value;
      }
    }

    /// <summary>Field number for the "timeout" field.</summary>
    public const int TimeoutFieldNumber = 5;
    private int timeout_;
    /// <summary>
    /// 允许中心节点请求子节点业务时的最大等待时间 
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Timeout {
      get { return timeout_; }
      set {
        timeout_ = value;
      }
    }

    /// <summary>Field number for the "loadBalancerConfigurations" field.</summary>
    public const int LoadBalancerConfigurationsFieldNumber = 6;
    private static readonly pb::FieldCodec<global::Collplex.Models.Client.Types.LoadBalancerConfiguration> _repeated_loadBalancerConfigurations_codec
        = pb::FieldCodec.ForMessage(50, global::Collplex.Models.Client.Types.LoadBalancerConfiguration.Parser);
    private readonly pbc::RepeatedField<global::Collplex.Models.Client.Types.LoadBalancerConfiguration> loadBalancerConfigurations_ = new pbc::RepeatedField<global::Collplex.Models.Client.Types.LoadBalancerConfiguration>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Collplex.Models.Client.Types.LoadBalancerConfiguration> LoadBalancerConfigurations {
      get { return loadBalancerConfigurations_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Client);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Client other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Name != other.Name) return false;
      if (ClientSecret != other.ClientSecret) return false;
      if (MaxServices != other.MaxServices) return false;
      if (RegIntervalSeconds != other.RegIntervalSeconds) return false;
      if (Timeout != other.Timeout) return false;
      if(!loadBalancerConfigurations_.Equals(other.loadBalancerConfigurations_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      if (ClientSecret.Length != 0) hash ^= ClientSecret.GetHashCode();
      if (MaxServices != 0) hash ^= MaxServices.GetHashCode();
      if (RegIntervalSeconds != 0) hash ^= RegIntervalSeconds.GetHashCode();
      if (Timeout != 0) hash ^= Timeout.GetHashCode();
      hash ^= loadBalancerConfigurations_.GetHashCode();
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
      if (Name.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Name);
      }
      if (ClientSecret.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(ClientSecret);
      }
      if (MaxServices != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(MaxServices);
      }
      if (RegIntervalSeconds != 0) {
        output.WriteRawTag(32);
        output.WriteInt32(RegIntervalSeconds);
      }
      if (Timeout != 0) {
        output.WriteRawTag(40);
        output.WriteInt32(Timeout);
      }
      loadBalancerConfigurations_.WriteTo(output, _repeated_loadBalancerConfigurations_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      if (ClientSecret.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(ClientSecret);
      }
      if (MaxServices != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(MaxServices);
      }
      if (RegIntervalSeconds != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(RegIntervalSeconds);
      }
      if (Timeout != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Timeout);
      }
      size += loadBalancerConfigurations_.CalculateSize(_repeated_loadBalancerConfigurations_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Client other) {
      if (other == null) {
        return;
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      if (other.ClientSecret.Length != 0) {
        ClientSecret = other.ClientSecret;
      }
      if (other.MaxServices != 0) {
        MaxServices = other.MaxServices;
      }
      if (other.RegIntervalSeconds != 0) {
        RegIntervalSeconds = other.RegIntervalSeconds;
      }
      if (other.Timeout != 0) {
        Timeout = other.Timeout;
      }
      loadBalancerConfigurations_.Add(other.loadBalancerConfigurations_);
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
            Name = input.ReadString();
            break;
          }
          case 18: {
            ClientSecret = input.ReadString();
            break;
          }
          case 24: {
            MaxServices = input.ReadInt32();
            break;
          }
          case 32: {
            RegIntervalSeconds = input.ReadInt32();
            break;
          }
          case 40: {
            Timeout = input.ReadInt32();
            break;
          }
          case 50: {
            loadBalancerConfigurations_.AddEntriesFrom(input, _repeated_loadBalancerConfigurations_codec);
            break;
          }
        }
      }
    }

    #region Nested types
    /// <summary>Container for nested types declared in the Client message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static partial class Types {
      public sealed partial class LoadBalancerConfiguration : pb::IMessage<LoadBalancerConfiguration> {
        private static readonly pb::MessageParser<LoadBalancerConfiguration> _parser = new pb::MessageParser<LoadBalancerConfiguration>(() => new LoadBalancerConfiguration());
        private pb::UnknownFieldSet _unknownFields;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pb::MessageParser<LoadBalancerConfiguration> Parser { get { return _parser; } }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pbr::MessageDescriptor Descriptor {
          get { return global::Collplex.Models.Client.Descriptor.NestedTypes[0]; }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        pbr::MessageDescriptor pb::IMessage.Descriptor {
          get { return Descriptor; }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public LoadBalancerConfiguration() {
          OnConstruction();
        }

        partial void OnConstruction();

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public LoadBalancerConfiguration(LoadBalancerConfiguration other) : this() {
          key_ = other.key_;
          type_ = other.type_;
          _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public LoadBalancerConfiguration Clone() {
          return new LoadBalancerConfiguration(this);
        }

        /// <summary>Field number for the "key" field.</summary>
        public const int KeyFieldNumber = 1;
        private string key_ = "";
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public string Key {
          get { return key_; }
          set {
            key_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
          }
        }

        /// <summary>Field number for the "type" field.</summary>
        public const int TypeFieldNumber = 2;
        private global::Collplex.Models.Client.Types.LoadBalancerConfiguration.Types.LoadBalanceType type_ = global::Collplex.Models.Client.Types.LoadBalancerConfiguration.Types.LoadBalanceType.NoLoadBalance;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public global::Collplex.Models.Client.Types.LoadBalancerConfiguration.Types.LoadBalanceType Type {
          get { return type_; }
          set {
            type_ = value;
          }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override bool Equals(object other) {
          return Equals(other as LoadBalancerConfiguration);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool Equals(LoadBalancerConfiguration other) {
          if (ReferenceEquals(other, null)) {
            return false;
          }
          if (ReferenceEquals(other, this)) {
            return true;
          }
          if (Key != other.Key) return false;
          if (Type != other.Type) return false;
          return Equals(_unknownFields, other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override int GetHashCode() {
          int hash = 1;
          if (Key.Length != 0) hash ^= Key.GetHashCode();
          if (Type != global::Collplex.Models.Client.Types.LoadBalancerConfiguration.Types.LoadBalanceType.NoLoadBalance) hash ^= Type.GetHashCode();
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
          if (Type != global::Collplex.Models.Client.Types.LoadBalancerConfiguration.Types.LoadBalanceType.NoLoadBalance) {
            output.WriteRawTag(16);
            output.WriteEnum((int) Type);
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
          if (Type != global::Collplex.Models.Client.Types.LoadBalancerConfiguration.Types.LoadBalanceType.NoLoadBalance) {
            size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Type);
          }
          if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
          }
          return size;
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public void MergeFrom(LoadBalancerConfiguration other) {
          if (other == null) {
            return;
          }
          if (other.Key.Length != 0) {
            Key = other.Key;
          }
          if (other.Type != global::Collplex.Models.Client.Types.LoadBalancerConfiguration.Types.LoadBalanceType.NoLoadBalance) {
            Type = other.Type;
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
                Type = (global::Collplex.Models.Client.Types.LoadBalancerConfiguration.Types.LoadBalanceType) input.ReadEnum();
                break;
              }
            }
          }
        }

        #region Nested types
        /// <summary>Container for nested types declared in the LoadBalancerConfiguration message type.</summary>
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static partial class Types {
          public enum LoadBalanceType {
            [pbr::OriginalName("NO_LOAD_BALANCE")] NoLoadBalance = 0,
            [pbr::OriginalName("SMOOTH_WEIGHT_ROUND_ROBIN")] SmoothWeightRoundRobin = 1,
            [pbr::OriginalName("LEAST_REQUESTS")] LeastRequests = 2,
            [pbr::OriginalName("RANDOM")] Random = 3,
            [pbr::OriginalName("SOURCE_IP_HASH")] SourceIpHash = 4,
          }

        }
        #endregion

      }

    }
    #endregion

  }

  #endregion

}

#endregion Designer generated code
