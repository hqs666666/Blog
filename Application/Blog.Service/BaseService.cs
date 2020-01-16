
using Blog.Dto;
using Blog.IService;
 
namespace Blog.Service
{
    public class BaseService : IBaseService
    {
        protected ResultMsg CreateResultMsg(string message, int rows, object data)
        {
            return new ResultMsg
            {
                Message = message,
                Result = rows >= 0,
                Data = data
            };
        }
    }
}
