namespace FishSupplyChain.Types.CustomExceptionTypes
{
    public class NotFoundException(string message) : Exception(message){}
    public class UnauthorizedActionException(string message) : Exception(message){}
    public class BadRequestException(string message) : Exception(message){}
}
