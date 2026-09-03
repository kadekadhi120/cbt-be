namespace cbt.be.Models.ResponseModels
{
    public class MainResponse<T>
    {
        public int Status { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

    }
}
