namespace API
{
    public sealed class Result<T>
    {
        #region Properties
        public T Value { get; }
        public Error Error { get; }
        public bool IsSuccess { get; private set; }
        #endregion
        #region Constructors
        private Result(T value)
        {
            Value = value;
            IsSuccess = true;
        }
        private Result(Error error)
        {
            Error = error;
            IsSuccess = false;
        }
        #endregion
        #region Factories
        public static Result<T> Success(T value) => new(value);
        public static Result<T> Failure(Error error) => new(error);
        #endregion

    }
    public record Error(ErrorType Type, string Description)
    {
        public static Error NoLineItems = new(ErrorType.Validation, "Line Items are empty");
        public static Error NotEnoughStock = new(ErrorType.Validation, "No Enough Stock for order");
        public static Error PaymentFailed = new(ErrorType.Failure, "Failed to process payment");
    }
    public enum ErrorType
    {
        Validation = 0,
        Failure = 1
    }
}
