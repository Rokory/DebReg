using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace DebReg.Mocks {
    public class HTTPMocks {
        private HttpContextBase _httpContextBase;

        private HttpContextBase HttpContextBase {
            get {
                if (_httpContextBase == null) {
                    var mockHttpContext = new Mock<HttpContextBase>();

                    mockHttpContext.SetupGet(m =>
                        m.User)
                        .Returns(() => {
                            if (UserName != null && UserId != null) {
                                List<Claim> claims = new List<Claim>{
                                    new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", UserId)
                                };
                                GenericIdentity identity = new GenericIdentity(UserName);
                                identity.AddClaims(claims);
                                GenericPrincipal principal = new GenericPrincipal(identity, new String[0]);
                                return principal;

                            }
                            return null;
                        });
                    _httpContextBase = mockHttpContext.Object;
                }
                return _httpContextBase;
            }
        }

        private ControllerContext _controllerContext;

        public ControllerContext ControllerContext {
            get {
                if (_controllerContext == null) {
                    var controllerContextMock = new Mock<ControllerContext>();
                    controllerContextMock.SetupGet(m =>
                        m.HttpContext)
                        .Returns(HttpContextBase);
                    _controllerContext = controllerContextMock.Object;
                }
                return _controllerContext;
            }
        }

        public String UserName { get; set; }
        public String UserId { get; set; }




    }
}
