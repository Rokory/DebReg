using DebRegCommunication;
using DebRegCommunication.Models;
using Moq;
using System;

namespace DebReg.Mocks {
    public class DebRegCommunicationMocks {
        private IEMailService _mailService;
        public IEMailService EMailService {
            get {
                if (_mailService == null) {
                    var mockMailService = new Mock<IEMailService>();
                    mockMailService.Setup(m => m.Send(It.IsAny<EMailMessage>(), It.IsAny<Object>()))
                        .Callback<EMailMessage, Object>((message, token) => { });
                    _mailService = mockMailService.Object;
                }
                return _mailService;
            }
        }
    }
}
