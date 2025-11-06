using VerificationDLL.Models;

namespace VerificationDLL
{
    public interface IVerificationDLL
    {
        Task<Models.VerificationResults> VerifyAsync(int uploadId);
    }
}
