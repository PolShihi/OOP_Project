using MyModel.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.Services
{
    public interface IEmailService
    {
        Task<ApiResponse<string>> SendEmailAsync(EmailSendRequestDTO emailSendRequest, CancellationToken cancellationToken = default);
    }
}
