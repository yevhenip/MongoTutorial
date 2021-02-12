namespace MongoTutorial.Core.Common
{
    public class Result<T>
    {
        public bool Succeeded { get; }
        public string Field { get; }
        public string Message { get; }
        public T Data { get; }

        private Result(T data, bool succeeded, string field, string message)
        {
            Data = data;
            Succeeded = succeeded;
            Field = field;
            Message = message;
        }


        public static Result<T> Success(T data = default)
        {
            return new(data, true, string.Empty, string.Empty);
        }

        public static Result<T> Failure(string field = "", string message = "")
        {
            return new(default, false, field, message);
        }
    }
}