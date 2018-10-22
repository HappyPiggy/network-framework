
public enum ErrorType
{
    None,//无错误
    ArgError, //结构参数错误
    CallbackError,//请求返回一个error结构
    ConnectError,
    ParseProtoBufError,//解析buf出错
    UnKnownCmd,//未知协议号
}