//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: player.proto
namespace usercmd
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RetErrorMsg")]
  public partial class RetErrorMsg : global::ProtoBuf.IExtensible
  {
    public RetErrorMsg() {}
    
    private uint _RetCode;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"RetCode", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint RetCode
    {
      get { return _RetCode; }
      set { _RetCode = value; }
    }
    private uint _Params = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"Params", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint Params
    {
      get { return _Params; }
      set { _Params = value; }
    }
    private string _JsonParam = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"JsonParam", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string JsonParam
    {
      get { return _JsonParam; }
      set { _JsonParam = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RetTeamAddr")]
  public partial class RetTeamAddr : global::ProtoBuf.IExtensible
  {
    public RetTeamAddr() {}
    
    private string _Address;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"Address", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string Address
    {
      get { return _Address; }
      set { _Address = value; }
    }
    private string _Key;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"Key", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string Key
    {
      get { return _Key; }
      set { _Key = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ObjNode")]
  public partial class ObjNode : global::ProtoBuf.IExtensible
  {
    public ObjNode() {}
    
    private uint _Id;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"Id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint Id
    {
      get { return _Id; }
      set { _Id = value; }
    }
    private uint _Num;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"Num", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint Num
    {
      get { return _Num; }
      set { _Num = value; }
    }
    private uint _Money = default(uint);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"Money", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint Money
    {
      get { return _Money; }
      set { _Money = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ActAward")]
  public partial class ActAward : global::ProtoBuf.IExtensible
  {
    public ActAward() {}
    
    private uint _TypeId;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"TypeId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint TypeId
    {
      get { return _TypeId; }
      set { _TypeId = value; }
    }
    private uint _Nums;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"Nums", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint Nums
    {
      get { return _Nums; }
      set { _Nums = value; }
    }
    private uint _State;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"State", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint State
    {
      get { return _State; }
      set { _State = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"MsgType")]
    public enum MsgType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"Login", Value=1)]
      Login = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ErrorMsg", Value=25)]
      ErrorMsg = 25,
            
      [global::ProtoBuf.ProtoEnum(Name=@"TeamAddr", Value=39)]
      TeamAddr = 39
    }
  
}