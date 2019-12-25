using Microsoft.Owin.Security.Provider;

namespace Microsoft.Owin.Security.VanLang
{
    /// <summary>
    /// Provides context information to middleware providers.
    /// </summary>
    public class VanLangReturnEndpointContext : ReturnEndpointContext
    {
        /// <summary>
        /// Initializes a new <see cref="VanLangReturnEndpointContext"/>.
        /// </summary>
        /// <param name="context">OWIN environment</param>
        /// <param name="ticket">The authentication ticket</param>
        public VanLangReturnEndpointContext(
            IOwinContext context,
            AuthenticationTicket ticket)
            : base(context, ticket)
        {
        }
    }
}