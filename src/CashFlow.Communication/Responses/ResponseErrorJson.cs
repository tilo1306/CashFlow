namespace CashFlow.Communication.Responses;

public class ResponseErrorJson
{
    public List<string> ErrorMessages { get; set; }

    public ResponseErrorJson(string ErrorMessage)
    {
        ErrorMessages = [ErrorMessage];
    }

    public ResponseErrorJson(List<string> errorMessage)
    {
        ErrorMessages = errorMessage;
    }
}