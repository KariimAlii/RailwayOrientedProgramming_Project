namespace API
{
    public static class ResultExtensions
    {
        public static Result<Tout> Bind<Tin,Tout>(this Result<Tin> result, Func<Tin, Result<Tout>> fn)
        {
            return result.IsSuccess
                ? fn(result.Value)
                : Result<Tout>.Failure(result.Error);
        }
        public static Result<Tout> TryCatch<Tin,Tout>(this Result<Tin> result, Func<Tin, Tout> fn, Error error)
        {
            try
            {
                return result.IsSuccess
                ? Result<Tout>.Success(fn(result.Value))
                : Result<Tout>.Failure(result.Error);
            }
            catch (Exception)
            {

                return Result<Tout>.Failure(error);
            }
            
        }
        public static Result<Tin> Tap<Tin>(this Result<Tin> result, Action<Tin> action)
        {
            // 🚩🚩 we assume that action cannot fail , otherwise we should use TryCatch
            if (result.IsSuccess)
            {
                action(result.Value);
            }

            return result;
        }
        public static Tout Match<Tin, Tout>(this Result<Tin> result, Func<Tin, Tout> OnSuccess, Func<Error, Tout> OnFailure)
        {
            return result.IsSuccess
                ? OnSuccess(result.Value)
                : OnFailure(result.Error);
        }
    }
}
