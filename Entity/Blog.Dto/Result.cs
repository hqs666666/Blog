
namespace Blog.Dto
{
    public class ResultMsg
    {
        public string Message { get; set; }
        public bool Result { get; set; }
        public object Data { get; set; }
        public int ErrorCode { get; set; }
    }
}
