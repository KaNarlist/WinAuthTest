using CoreWCF.IdentityModel.Selectors;
using System;

namespace WinAuthTest
{
    public class ServiceAuthenticator : UserNamePasswordValidator
    {

        public override void Validate(string userName, string password)
        {
            //is only called with basic auth, but not windows auth
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                throw new Exception("Invalid credentials!");
        }
    }
}
