namespace Dike.Identity.Core.Common
{
    public class IdentityException : Exception
    {
        public string InternalCode { get; }

        public IdentityException(string internalCode, string message) : base(message)
        {
            InternalCode = internalCode;
        }
    }
}
