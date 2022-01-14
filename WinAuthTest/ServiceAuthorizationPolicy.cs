using CoreWCF.IdentityModel.Claims;
using CoreWCF.IdentityModel.Policy;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace WinAuthTest
{
    public class ServiceAuthorizationPolicy : IAuthorizationPolicy
    {
        private string _id;
        public ClaimSet Issuer => ClaimSet.System;

        public string Id => _id;

        public ServiceAuthorizationPolicy()
        {
            _id = Guid.NewGuid().ToString();
        }

        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            object primaryIdentity;
            if (false == evaluationContext.Properties.TryGetValue("Identities", out primaryIdentity))
                return false;
 
            IList<IIdentity> listIDs = primaryIdentity as IList<IIdentity>;
            if (null == listIDs || listIDs.Count < 1)
                return false;

            var identity = listIDs[0];
            if (!identity.IsAuthenticated)
                return false;

            if (identity is GenericIdentity)
            {
                //identity is a GenericIdentiy when something like basic auth is used
            }
            else if (identity is WindowsIdentity winIdentity)
            {
                //identity is a WindowsIdentity when windows auth is used
            }
            return true;
        }
    }
}
