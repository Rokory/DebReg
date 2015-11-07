using DebReg.Security;
using DebReg.Models;
using Microsoft.Owin.Security;
using Moq;
using System.Threading.Tasks;

namespace DebReg.Mocks
{
    public class SecurityMocks
    {
        private ISecurityManager _securityManager;

        public ISecurityManager SecurityManager
        {
            get
            {
                if (_securityManager == null)
                {
                    var mock = new Mock<ISecurityManager>();
                    mock.Setup(m =>
                        m.Logout()
                    )
                    .Callback(() =>
                        { }
                    );

                    mock.Setup(m =>
                        m.LoginAsync(It.IsAny<User>())
                    )
                    .Returns<User>(u =>
                        LoginAsync(u)
                    );

                    _securityManager = mock.Object;
                }
                return _securityManager;
            }
        }

        private async Task LoginAsync(User u)
        {
            return;
        }

        private IAuthenticationManager _authManager;
        public IAuthenticationManager AuthManager
        {
            get
            {
                if (_authManager == null)
                {
                    var mock = new Mock<IAuthenticationManager>();

                    _authManager = mock.Object;
                }
                return _authManager;
            }
        }
    }
}
