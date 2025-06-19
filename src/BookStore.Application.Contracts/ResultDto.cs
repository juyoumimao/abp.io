using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore
{
    public class ResultDto
    {
        public bool IsSuc { get; set; }

        public ResultCode code { get; set; }

        public string msg { get; set; }

        public ResultDto(bool IsSuc, ResultCode code, string msg)
        {
            this.IsSuc = IsSuc;
            this.code = code;
            this.msg = msg;
        }

        public static ResultDto Success(ResultCode code)
        {
            return new ResultDto(true, code, "操作成功");
        }
        public static ResultDto Fail(ResultCode code, string reason)
        {
            return new ResultDto(true, code, reason);
        }
    }
    public class ResultDto<T> : ResultDto
    {
        public T data { get; set; }

        public ResultDto(bool IsSuc, ResultCode code, string msg, T data) : base(IsSuc, code, msg)
        {
            this.data = data;
        }

        public static ResultDto<T> Success(ResultCode code, T data)
        {
            return new ResultDto<T>(true, code, "操作成功", data);
        }
        public static ResultDto<T> Fail(ResultCode code, string reason)
        {
            return new ResultDto<T>(true, code, reason, default!);
        }
    }

    public enum ResultCode
    {
        Ok = 200,
        Fail = 500
    }
}
