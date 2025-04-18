using EXE_FAIEnglishTutor.Common;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Repositories.Interface;
using EXE_FAIEnglishTutor.Services.Interface;
using EXE_FAIEnglishTutor.Utils;

namespace EXE_FAIEnglishTutor.Services.Implementaion
{
    public class VerificationTokenServiceImpl : IVerificationTokenService
    {
        private readonly IVerificationTokenRepository _repository;

        public VerificationTokenServiceImpl(IVerificationTokenRepository repository)
        {
            _repository = repository;
        }
        public void createVertificationToken(User user, string token)
        {
            VerificationToken verificationToken = new VerificationToken
            {
                TokenCode = token,
                UserId = user.UserId,
                ExpiryDate = DateTimeUtils.calculateExpiryDate(Constants.EXPIRATION)
            };
            _repository.save(verificationToken);
        }

        public async Task<VerificationToken> getVerificationToken(string token)
        {
            return await _repository.findByToken(token);
        }


        public async Task updateVerificationToken(User user, string newToken, int expireTime)
        {
            VerificationToken existingToken = await _repository.findByUserId(user.UserId);
            if (existingToken == null) {
                throw new Exception("NotFoundTokenByUserId");
            } 
                existingToken.TokenCode = newToken;
                existingToken.ExpiryDate = DateTime.Now.AddMinutes(expireTime);
                await _repository.UpdateToken(existingToken);
        }


    }
}
