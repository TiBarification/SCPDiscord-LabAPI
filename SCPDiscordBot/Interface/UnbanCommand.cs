// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: BotToPlugin/UnbanCommand.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace SCPDiscord.Interface {

  /// <summary>Holder for reflection information generated from BotToPlugin/UnbanCommand.proto</summary>
  public static partial class UnbanCommandReflection {

    #region Descriptor
    /// <summary>File descriptor for BotToPlugin/UnbanCommand.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static UnbanCommandReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Ch5Cb3RUb1BsdWdpbi9VbmJhbkNvbW1hbmQucHJvdG8SFFNDUERpc2NvcmQu",
            "SW50ZXJmYWNlIpkBCgxVbmJhbkNvbW1hbmQSEQoJY2hhbm5lbElEGAEgASgE",
            "EhMKC3N0ZWFtSURPcklQGAIgASgJEhUKDWludGVyYWN0aW9uSUQYAyABKAQS",
            "GgoSZGlzY29yZERpc3BsYXlOYW1lGAQgASgJEhcKD2Rpc2NvcmRVc2VybmFt",
            "ZRgFIAEoCRIVCg1kaXNjb3JkVXNlcklEGAYgASgEYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::SCPDiscord.Interface.UnbanCommand), global::SCPDiscord.Interface.UnbanCommand.Parser, new[]{ "ChannelID", "SteamIDOrIP", "InteractionID", "DiscordDisplayName", "DiscordUsername", "DiscordUserID" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  [global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
  public sealed partial class UnbanCommand : pb::IMessage<UnbanCommand>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<UnbanCommand> _parser = new pb::MessageParser<UnbanCommand>(() => new UnbanCommand());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<UnbanCommand> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::SCPDiscord.Interface.UnbanCommandReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public UnbanCommand() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public UnbanCommand(UnbanCommand other) : this() {
      channelID_ = other.channelID_;
      steamIDOrIP_ = other.steamIDOrIP_;
      interactionID_ = other.interactionID_;
      discordDisplayName_ = other.discordDisplayName_;
      discordUsername_ = other.discordUsername_;
      discordUserID_ = other.discordUserID_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public UnbanCommand Clone() {
      return new UnbanCommand(this);
    }

    /// <summary>Field number for the "channelID" field.</summary>
    public const int ChannelIDFieldNumber = 1;
    private ulong channelID_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong ChannelID {
      get { return channelID_; }
      set {
        channelID_ = value;
      }
    }

    /// <summary>Field number for the "steamIDOrIP" field.</summary>
    public const int SteamIDOrIPFieldNumber = 2;
    private string steamIDOrIP_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string SteamIDOrIP {
      get { return steamIDOrIP_; }
      set {
        steamIDOrIP_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "interactionID" field.</summary>
    public const int InteractionIDFieldNumber = 3;
    private ulong interactionID_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong InteractionID {
      get { return interactionID_; }
      set {
        interactionID_ = value;
      }
    }

    /// <summary>Field number for the "discordDisplayName" field.</summary>
    public const int DiscordDisplayNameFieldNumber = 4;
    private string discordDisplayName_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string DiscordDisplayName {
      get { return discordDisplayName_; }
      set {
        discordDisplayName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "discordUsername" field.</summary>
    public const int DiscordUsernameFieldNumber = 5;
    private string discordUsername_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string DiscordUsername {
      get { return discordUsername_; }
      set {
        discordUsername_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "discordUserID" field.</summary>
    public const int DiscordUserIDFieldNumber = 6;
    private ulong discordUserID_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong DiscordUserID {
      get { return discordUserID_; }
      set {
        discordUserID_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as UnbanCommand);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(UnbanCommand other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (ChannelID != other.ChannelID) return false;
      if (SteamIDOrIP != other.SteamIDOrIP) return false;
      if (InteractionID != other.InteractionID) return false;
      if (DiscordDisplayName != other.DiscordDisplayName) return false;
      if (DiscordUsername != other.DiscordUsername) return false;
      if (DiscordUserID != other.DiscordUserID) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (ChannelID != 0UL) hash ^= ChannelID.GetHashCode();
      if (SteamIDOrIP.Length != 0) hash ^= SteamIDOrIP.GetHashCode();
      if (InteractionID != 0UL) hash ^= InteractionID.GetHashCode();
      if (DiscordDisplayName.Length != 0) hash ^= DiscordDisplayName.GetHashCode();
      if (DiscordUsername.Length != 0) hash ^= DiscordUsername.GetHashCode();
      if (DiscordUserID != 0UL) hash ^= DiscordUserID.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (ChannelID != 0UL) {
        output.WriteRawTag(8);
        output.WriteUInt64(ChannelID);
      }
      if (SteamIDOrIP.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(SteamIDOrIP);
      }
      if (InteractionID != 0UL) {
        output.WriteRawTag(24);
        output.WriteUInt64(InteractionID);
      }
      if (DiscordDisplayName.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(DiscordDisplayName);
      }
      if (DiscordUsername.Length != 0) {
        output.WriteRawTag(42);
        output.WriteString(DiscordUsername);
      }
      if (DiscordUserID != 0UL) {
        output.WriteRawTag(48);
        output.WriteUInt64(DiscordUserID);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (ChannelID != 0UL) {
        output.WriteRawTag(8);
        output.WriteUInt64(ChannelID);
      }
      if (SteamIDOrIP.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(SteamIDOrIP);
      }
      if (InteractionID != 0UL) {
        output.WriteRawTag(24);
        output.WriteUInt64(InteractionID);
      }
      if (DiscordDisplayName.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(DiscordDisplayName);
      }
      if (DiscordUsername.Length != 0) {
        output.WriteRawTag(42);
        output.WriteString(DiscordUsername);
      }
      if (DiscordUserID != 0UL) {
        output.WriteRawTag(48);
        output.WriteUInt64(DiscordUserID);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (ChannelID != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(ChannelID);
      }
      if (SteamIDOrIP.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(SteamIDOrIP);
      }
      if (InteractionID != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(InteractionID);
      }
      if (DiscordDisplayName.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(DiscordDisplayName);
      }
      if (DiscordUsername.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(DiscordUsername);
      }
      if (DiscordUserID != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(DiscordUserID);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(UnbanCommand other) {
      if (other == null) {
        return;
      }
      if (other.ChannelID != 0UL) {
        ChannelID = other.ChannelID;
      }
      if (other.SteamIDOrIP.Length != 0) {
        SteamIDOrIP = other.SteamIDOrIP;
      }
      if (other.InteractionID != 0UL) {
        InteractionID = other.InteractionID;
      }
      if (other.DiscordDisplayName.Length != 0) {
        DiscordDisplayName = other.DiscordDisplayName;
      }
      if (other.DiscordUsername.Length != 0) {
        DiscordUsername = other.DiscordUsername;
      }
      if (other.DiscordUserID != 0UL) {
        DiscordUserID = other.DiscordUserID;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
      if ((tag & 7) == 4) {
        // Abort on any end group tag.
        return;
      }
      switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            ChannelID = input.ReadUInt64();
            break;
          }
          case 18: {
            SteamIDOrIP = input.ReadString();
            break;
          }
          case 24: {
            InteractionID = input.ReadUInt64();
            break;
          }
          case 34: {
            DiscordDisplayName = input.ReadString();
            break;
          }
          case 42: {
            DiscordUsername = input.ReadString();
            break;
          }
          case 48: {
            DiscordUserID = input.ReadUInt64();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
      if ((tag & 7) == 4) {
        // Abort on any end group tag.
        return;
      }
      switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 8: {
            ChannelID = input.ReadUInt64();
            break;
          }
          case 18: {
            SteamIDOrIP = input.ReadString();
            break;
          }
          case 24: {
            InteractionID = input.ReadUInt64();
            break;
          }
          case 34: {
            DiscordDisplayName = input.ReadString();
            break;
          }
          case 42: {
            DiscordUsername = input.ReadString();
            break;
          }
          case 48: {
            DiscordUserID = input.ReadUInt64();
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code
