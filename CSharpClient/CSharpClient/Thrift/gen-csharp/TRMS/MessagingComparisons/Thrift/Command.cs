/**
 * Autogenerated by Thrift Compiler (0.8.0)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using Thrift.Protocol;
using Thrift.Transport;
namespace TRMS.MessagingComparisons.Thrift
{

  [Serializable]
  public partial class Command : TBase
  {
    private string _commandName;
    private int _region;
    private string _videoFileName;
    private string _audioFileName0;
    private string _audioFileName1;
    private string _audioFileName2;
    private string _audioFileName3;
    private string _vbiFileName;
    private bool _useTDIR;
    private long _initialFrame;
    private double _initialRate;
    private bool _loop;

    public string CommandName
    {
      get
      {
        return _commandName;
      }
      set
      {
        __isset.commandName = true;
        this._commandName = value;
      }
    }

    public int Region
    {
      get
      {
        return _region;
      }
      set
      {
        __isset.region = true;
        this._region = value;
      }
    }

    public string VideoFileName
    {
      get
      {
        return _videoFileName;
      }
      set
      {
        __isset.videoFileName = true;
        this._videoFileName = value;
      }
    }

    public string AudioFileName0
    {
      get
      {
        return _audioFileName0;
      }
      set
      {
        __isset.audioFileName0 = true;
        this._audioFileName0 = value;
      }
    }

    public string AudioFileName1
    {
      get
      {
        return _audioFileName1;
      }
      set
      {
        __isset.audioFileName1 = true;
        this._audioFileName1 = value;
      }
    }

    public string AudioFileName2
    {
      get
      {
        return _audioFileName2;
      }
      set
      {
        __isset.audioFileName2 = true;
        this._audioFileName2 = value;
      }
    }

    public string AudioFileName3
    {
      get
      {
        return _audioFileName3;
      }
      set
      {
        __isset.audioFileName3 = true;
        this._audioFileName3 = value;
      }
    }

    public string VbiFileName
    {
      get
      {
        return _vbiFileName;
      }
      set
      {
        __isset.vbiFileName = true;
        this._vbiFileName = value;
      }
    }

    public bool UseTDIR
    {
      get
      {
        return _useTDIR;
      }
      set
      {
        __isset.useTDIR = true;
        this._useTDIR = value;
      }
    }

    public long InitialFrame
    {
      get
      {
        return _initialFrame;
      }
      set
      {
        __isset.initialFrame = true;
        this._initialFrame = value;
      }
    }

    public double InitialRate
    {
      get
      {
        return _initialRate;
      }
      set
      {
        __isset.initialRate = true;
        this._initialRate = value;
      }
    }

    public bool Loop
    {
      get
      {
        return _loop;
      }
      set
      {
        __isset.loop = true;
        this._loop = value;
      }
    }


    public Isset __isset;
    [Serializable]
    public struct Isset {
      public bool commandName;
      public bool region;
      public bool videoFileName;
      public bool audioFileName0;
      public bool audioFileName1;
      public bool audioFileName2;
      public bool audioFileName3;
      public bool vbiFileName;
      public bool useTDIR;
      public bool initialFrame;
      public bool initialRate;
      public bool loop;
    }

    public Command() {
    }

    public void Read (TProtocol iprot)
    {
      TField field;
      iprot.ReadStructBegin();
      while (true)
      {
        field = iprot.ReadFieldBegin();
        if (field.Type == TType.Stop) { 
          break;
        }
        switch (field.ID)
        {
          case 1:
            if (field.Type == TType.String) {
              CommandName = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I32) {
              Region = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.String) {
              VideoFileName = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.String) {
              AudioFileName0 = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.String) {
              AudioFileName1 = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 6:
            if (field.Type == TType.String) {
              AudioFileName2 = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 7:
            if (field.Type == TType.String) {
              AudioFileName3 = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 8:
            if (field.Type == TType.String) {
              VbiFileName = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 9:
            if (field.Type == TType.Bool) {
              UseTDIR = iprot.ReadBool();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 10:
            if (field.Type == TType.I64) {
              InitialFrame = iprot.ReadI64();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 11:
            if (field.Type == TType.Double) {
              InitialRate = iprot.ReadDouble();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 12:
            if (field.Type == TType.Bool) {
              Loop = iprot.ReadBool();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          default: 
            TProtocolUtil.Skip(iprot, field.Type);
            break;
        }
        iprot.ReadFieldEnd();
      }
      iprot.ReadStructEnd();
    }

    public void Write(TProtocol oprot) {
      TStruct struc = new TStruct("Command");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (CommandName != null && __isset.commandName) {
        field.Name = "commandName";
        field.Type = TType.String;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(CommandName);
        oprot.WriteFieldEnd();
      }
      if (__isset.region) {
        field.Name = "region";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(Region);
        oprot.WriteFieldEnd();
      }
      if (VideoFileName != null && __isset.videoFileName) {
        field.Name = "videoFileName";
        field.Type = TType.String;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(VideoFileName);
        oprot.WriteFieldEnd();
      }
      if (AudioFileName0 != null && __isset.audioFileName0) {
        field.Name = "audioFileName0";
        field.Type = TType.String;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(AudioFileName0);
        oprot.WriteFieldEnd();
      }
      if (AudioFileName1 != null && __isset.audioFileName1) {
        field.Name = "audioFileName1";
        field.Type = TType.String;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(AudioFileName1);
        oprot.WriteFieldEnd();
      }
      if (AudioFileName2 != null && __isset.audioFileName2) {
        field.Name = "audioFileName2";
        field.Type = TType.String;
        field.ID = 6;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(AudioFileName2);
        oprot.WriteFieldEnd();
      }
      if (AudioFileName3 != null && __isset.audioFileName3) {
        field.Name = "audioFileName3";
        field.Type = TType.String;
        field.ID = 7;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(AudioFileName3);
        oprot.WriteFieldEnd();
      }
      if (VbiFileName != null && __isset.vbiFileName) {
        field.Name = "vbiFileName";
        field.Type = TType.String;
        field.ID = 8;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(VbiFileName);
        oprot.WriteFieldEnd();
      }
      if (__isset.useTDIR) {
        field.Name = "useTDIR";
        field.Type = TType.Bool;
        field.ID = 9;
        oprot.WriteFieldBegin(field);
        oprot.WriteBool(UseTDIR);
        oprot.WriteFieldEnd();
      }
      if (__isset.initialFrame) {
        field.Name = "initialFrame";
        field.Type = TType.I64;
        field.ID = 10;
        oprot.WriteFieldBegin(field);
        oprot.WriteI64(InitialFrame);
        oprot.WriteFieldEnd();
      }
      if (__isset.initialRate) {
        field.Name = "initialRate";
        field.Type = TType.Double;
        field.ID = 11;
        oprot.WriteFieldBegin(field);
        oprot.WriteDouble(InitialRate);
        oprot.WriteFieldEnd();
      }
      if (__isset.loop) {
        field.Name = "loop";
        field.Type = TType.Bool;
        field.ID = 12;
        oprot.WriteFieldBegin(field);
        oprot.WriteBool(Loop);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("Command(");
      sb.Append("CommandName: ");
      sb.Append(CommandName);
      sb.Append(",Region: ");
      sb.Append(Region);
      sb.Append(",VideoFileName: ");
      sb.Append(VideoFileName);
      sb.Append(",AudioFileName0: ");
      sb.Append(AudioFileName0);
      sb.Append(",AudioFileName1: ");
      sb.Append(AudioFileName1);
      sb.Append(",AudioFileName2: ");
      sb.Append(AudioFileName2);
      sb.Append(",AudioFileName3: ");
      sb.Append(AudioFileName3);
      sb.Append(",VbiFileName: ");
      sb.Append(VbiFileName);
      sb.Append(",UseTDIR: ");
      sb.Append(UseTDIR);
      sb.Append(",InitialFrame: ");
      sb.Append(InitialFrame);
      sb.Append(",InitialRate: ");
      sb.Append(InitialRate);
      sb.Append(",Loop: ");
      sb.Append(Loop);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
